using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Html;

namespace Knoema.Windows8.Data
{
	/// <summary>
	/// Generic item data model.
	/// </summary>
	public class ResourceItem : ItemBase
	{
		public class Serial
		{
			public string Id { get; set; }
			public string Title { get; set; }
			public string Description { get; set; }
			public string ResourceUrl { get; set; }
			public string ThumbnailUrl { get; set; }
			public string EmbedUrl { get; set; }
			public string Type { get; set; }
		}
		
		public ResourceItem(String uniqueId, String title, String subtitle, String imagePath, String description, TagItem group)
			: base(uniqueId, title, subtitle, imagePath, description)
		{
			this._group = group;
		}

		public ResourceItem(Serial item, TagItem group) : base (item.Id, item.Title, string.Empty, item.ThumbnailUrl.Replace(".png", "-x2.png"), item.Description)
		{
			this._content = item.EmbedUrl + "?noheader=1";
			this._group = group;
		}

		private string _content = string.Empty;
		public string Content
		{
			get { return this._content; }
			set { this.SetProperty(ref this._content, value); }
		}

		private TagItem _group;
		public TagItem Group
		{
			get { return this._group; }
			set { this.SetProperty(ref this._group, value); }
		}

		public string RawDescription
		{
			get
			{
				return HtmlUtilities.ConvertToText(Description ?? string.Empty);
			}
		}
	}
}
