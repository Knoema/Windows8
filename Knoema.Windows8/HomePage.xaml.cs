using Knoema.Windows8.Data;

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
using Windows.ApplicationModel.Search;
using Knoema.Windows8.Data.Search;

// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace Knoema.Windows8
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HomePage : Knoema.Windows8.Common.LayoutAwarePage
    {
        public HomePage()
        {
            this.InitializeComponent();

			SearchPane.GetForCurrentView().SuggestionsRequested += SearchPage_SuggestionsRequested;
			SearchPane.GetForCurrentView().QuerySubmitted += SearchPage_QuerySubmitted;
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
            var homeTags = await AppModel.GetHomeTags((String)navigationParameter);
            this.DefaultViewModel["Groups"] = homeTags;
			this.progressRing.IsActive = false;
        }

        /// <summary>
        /// Invoked when a group header is clicked.
        /// </summary>
        /// <param name="sender">The Button used as a group header for the selected group.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Header_Click(object sender, RoutedEventArgs e)
        {
            // Determine what group the Button instance represents
            var group = (sender as FrameworkElement).DataContext;

            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            this.Frame.Navigate(typeof(TagPage), ((TagItem)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((ResourceItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemDetailPage), itemId);
        }

		async void SearchPage_SuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs args)
		{
			var homeTags = await AppModel.GetHomeTags("AllGroups");

			var suggestedItems = new List<string>();
			suggestedItems.AddRange(homeTags.Select(x => x.Title));

			foreach (var tag in homeTags)
				suggestedItems.AddRange(tag.Items.Select(x => x.Title));

			suggestedItems = suggestedItems.Where(x => x.ToLower().Contains(args.QueryText.ToLower())).Distinct().ToList();
			args.Request.SearchSuggestionCollection.AppendQuerySuggestions(suggestedItems);
		}

		async void SearchPage_QuerySubmitted(SearchPane sender, SearchPaneQuerySubmittedEventArgs args)
		{
			var homeTags = await AppModel.GetHomeTags("AllGroups");

			TagItem tag = null;
			ResourceItem res = null;

			tag = homeTags.FirstOrDefault(x => x.Title == args.QueryText);
			if (tag == null)
			{
				foreach (var item in homeTags)
				{
					res = item.Items.FirstOrDefault(x => x.Title == args.QueryText);
					if (res != null)
						break;
				}
			}

			if (tag != null)
				this.Frame.Navigate(typeof(TagPage), tag.UniqueId);

			else if (res != null)
				this.Frame.Navigate(typeof(ItemDetailPage), res.UniqueId);

			else
			{
				this.Frame.Navigate(typeof(SearchPage), args.QueryText);
			}

		}
    }
}
