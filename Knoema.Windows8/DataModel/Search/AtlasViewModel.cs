using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knoema.Windows8.Data;

namespace Knoema.Windows8.DataModel.Search
{
	public class AtlasViewModel
	{
		public string Title { get; set; }
		public string ImageSource { get; set; }
		public List<string> Parameters { get; set; }

		public List<TagItem> RelatedPages { get; set; }

	}
}
