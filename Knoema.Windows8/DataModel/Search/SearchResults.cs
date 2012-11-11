using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knoema.Windows8.Data.Search
{
	public enum ResultType
	{
		TimeSeries,
		Resource,
		Dataset,
		Crosschart,
		Tag,
		DataPoints,
		Atlas
	}

	public class ResultItem
	{
		public ResultType Type { get; set; }
		public ResourceItem.Serial Resource { get; set; }
		public string Tag { get; set; }
	}

	public class SearchResults
	{
		public SearchResults()
		{
			Items = new List<ResultItem>();
		}

		public List<ResultItem> Items { get; set; }
	}	
}
