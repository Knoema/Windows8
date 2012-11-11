using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Knoema.Windows8.Data;
using Knoema.Windows8.Data.Search;
using Knoema.Windows8.DataModel.Search;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Knoema.Windows8
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class SearchPage : Knoema.Windows8.Common.LayoutAwarePage
	{
		public SearchPage()
		{
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
			var query = (string)navigationParameter;			
			pageTitle.Text = string.Format("Search result for \"{0}\"", query);

			var searchResults = await AppModel.Search(query);

			var atlasResult = searchResults.Items.Where(x => x.Type == ResultType.Atlas && x.Indicator == null).ToList();
			if (atlasResult.Any())
			{
				var client = new System.Net.Http.HttpClient();
				var response = await client.GetAsync(atlasResult.First().EmbedUrl);
				ParseAtlasPage(await response.Content.ReadAsStringAsync());
			}

			//this.DefaultViewModel["Atlas"] = atlasResult;
			//this.DefaultViewModel["Tags"] = searchResults.Items.Where(x => x.Type == ResultType.Tag).ToList();

			var tag = new TagItem(string.Empty, "Dashboards", string.Empty, string.Empty, string.Empty, string.Empty);

			var resources = searchResults.Items
					.Where(x => x.Type == Data.Search.ResultType.Resource)
					.Select(x => x.Resource).Where(x => x.Type == "Page")
					.Select(x => new ResourceItem(x, null, "#059bda"))
					.ToList();

			foreach (var item in resources)
				tag.Items.Add(item);

			this.DefaultViewModel["Resources"] = new List<TagItem>() { tag };

			this.progressRing.IsActive = false;	
		}

		/// <summary>
		/// Preserves state associated with this page in case the application is suspended or the
		/// page is discarded from the navigation cache.  Values must conform to the serialization
		/// requirements of <see cref="SuspensionManager.SessionState"/>.
		/// </summary>
		/// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
		protected override void SaveState(Dictionary<String, Object> pageState)
		{
		}

		private AtlasViewModel ParseAtlasPage(string result)
		{
			XDocument doc = XDocument.Parse(result, LoadOptions.None);
			var titleNode = doc.Descendants("h3").FirstOrDefault();
			var relatedPages = doc.Descendants("a").Where(a => a.Attribute("class").Value == "page")
				.Select(a => new ResourceItem(a.Attribute("data-id").Value, a.Attribute("title").Value, "", string.Format("http://th.knoema.com/production/{0}.png", a.Attribute("data-id").Value), "", null, "")).ToList();

			var model = new AtlasViewModel
			{
				ImageSource = doc.Descendants("img").Where(node => node.Attribute("class").Value == "flag").Select(img => img.Attribute("src").Value).FirstOrDefault(),
				Title = titleNode.Value,
				Parameters = (titleNode.NextNode as XElement).Element("ul").Elements("li").Select(el => el.Value.ToString()).ToList()
			};

			return model;
		}

		void ItemView_ItemClick(object sender, ItemClickEventArgs e)
		{
			// Navigate to the appropriate destination page, configuring the new page
			// by passing required information as a navigation parameter
			var itemId = ((ResourceItem)e.ClickedItem).UniqueId;
			this.Frame.Navigate(typeof(ItemDetailPage), itemId);
		}
	}
}
