﻿using Knoema.Windows8.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;

// The Item Detail Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234232

namespace Knoema.Windows8
{
    /// <summary>
    /// A page that displays details for a single item within a group while allowing gestures to
    /// flip through other items belonging to the same group.
    /// </summary>
    public sealed partial class ItemDetailPage : Knoema.Windows8.Common.LayoutAwarePage
	{
		private static TypedEventHandler<DataTransferManager, DataRequestedEventArgs> _handler;
        public ItemDetailPage()
        {
			if (_handler != null)
				DataTransferManager.GetForCurrentView().DataRequested -= _handler;

			_handler = ShareLinkHandler;
			DataTransferManager.GetForCurrentView().DataRequested += _handler;

            this.InitializeComponent();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // Allow saved page state to override the initial item to display
			if (pageState != null && pageState.ContainsKey("Item"))
            {
				navigationParameter = pageState["Item"];
            }

            // TODO: Create an appropriate data model for your problem domain to replace the sample data
			var item = await AppModel.GetResource((String)navigationParameter);
            this.DefaultViewModel["Item"] = item;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
			if (this.DefaultViewModel.ContainsKey("item"))
				pageState["Item"] = this.DefaultViewModel["Item"];
        }

		private T FindFirstElementInVisualTree<T>(DependencyObject parentElement) where T : DependencyObject
		{
			var count = VisualTreeHelper.GetChildrenCount(parentElement);
			if (count == 0)
				return null;

			for (int i = 0; i < count; i++)
			{
				var child = VisualTreeHelper.GetChild(parentElement, i);

				if (child != null && child is T)
				{
					return (T)child;
				}
				else
				{
					var result = FindFirstElementInVisualTree<T>(child);
					if (result != null)
						return result;
				}
			}
			return null;
		}

		private void flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			FlipView flipView = (FlipView) sender;

			var container = flipView.ItemContainerGenerator.ContainerFromItem(flipView.SelectedIndex);
			if (container != null)
				FindFirstElementInVisualTree<WebView>(container).Navigate(new Uri((flipView.SelectedItem as ResourceItem).Content));
		}

		private void ShareLinkHandler(DataTransferManager sender, DataRequestedEventArgs e)
		{
			var selectedItem = (ResourceItem)this.DefaultViewModel["Item"];

			DataRequest request = e.Request;
			request.Data.Properties.Title = selectedItem.Title;
			request.Data.Properties.Description = selectedItem.Description ?? string.Empty;
			request.Data.SetUri(new Uri("http://knoema.com/" + selectedItem.UniqueId));
		}
    }
}
