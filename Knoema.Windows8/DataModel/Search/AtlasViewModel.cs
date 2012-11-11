using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knoema.Windows8.Data;

namespace Knoema.Windows8.DataModel.Search
{
	public class AtlasViewModel
	{
		public AtlasViewModel()
		{
			RelatedPages = new ObservableCollection<ResourceItem>();
		}
		public string Title { get; set; }
		public string ImageSource { get; set; }
		public List<string> Parameters { get; set; }

		public ObservableCollection <ResourceItem> RelatedPages { get; set; }

	}
}
