using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace Knoema.Windows8.Data
{



    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class AppModel
    {
        private static AppModel _instance = null;
		
		private bool _loaded = false;
		private ObservableCollection<TagItem> _homeTags = new ObservableCollection<TagItem>();

		private static AppModel Instance
		{
			get
			{
				if (_instance == null)
					lock(typeof(AppModel))
						if (_instance == null)
						{
							_instance = new AppModel();
						}

				return _instance;
			}
		}

		private async Task<AppModel> EnsureLoaded()
		{
			if (!_loaded)
				{
					await LoadTopics();
					_loaded = true;
				}

			return this;
		}

		private ObservableCollection<TagItem> HomeTags
		{
			get
			{
				return _homeTags;
			}
		}
        
		
        public static async Task<IEnumerable<TagItem>> GetHomeTags(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            
            var instance = await Instance.EnsureLoaded();
			
			return instance.HomeTags;
        }

        public static async Task<TagItem> GetTag(string uniqueId)
        {
			var instance = await Instance.EnsureLoaded();

			// Simple linear search is acceptable for small data sets
			var matches = instance.HomeTags.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<ResourceItem> GetItem(string uniqueId)
        {
			var instance = await Instance.EnsureLoaded();

			// Simple linear search is acceptable for small data sets
			var matches = instance.HomeTags.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            return matches.FirstOrDefault();
        }

		private async Task LoadTopics()
		{
			var client = new System.Net.Http.HttpClient();
			var tags = new string[]{
									"Demographics",
									"Energy",
									"Economics",
									"Commodities",
									"Africa",
									"Agriculture",
									"Commodity Passport",
									"World Data Maps"};

			var colors = new string[] { "#146714", "#671414", "#674b14", "#14674b", "#144b67", "#141467", "#4b1467", "#66144b" };
		
			foreach (var topic in tags)
			{
				var color = colors[tags.ToList().IndexOf(topic)];

				var group = new TagItem(topic, topic, string.Empty, "Assets/DarkGray.png", string.Empty, color);

				var tagResp = await client.GetAsync(string.Format("http://dev.knoema.org/api/1.0/frontend/tags?tag={0}&client_id=EZj54KGFo3rzIvnLczrElvAitEyU28DGw9R73tif", topic));

				var resources = JsonConvert.DeserializeObject<IEnumerable<ResourceItem.Serial>>(await tagResp.Content.ReadAsStringAsync());
				foreach (var resource in resources.Where(res => res.Type != "Dataset"))
					group.Items.Add(new ResourceItem(resource, group, color));

				this.HomeTags.Add(group);
			}
		}
    }
}
