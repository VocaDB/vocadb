#nullable disable

using VocaDb.Model;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service;

namespace VocaDb.Web.Models.Album
{
	/// <summary>
	/// Parameter collection given to index action.
	/// </summary>
	public class IndexRouteParams
	{
		public IndexRouteParams() { }

		public IndexRouteParams(IndexRouteParams source, int? page)
		{
			ParamIs.NotNull(() => source);

			discType = (source.discType != DiscType.Unknown ? source.discType : null);
			draftsOnly = (source.draftsOnly == true ? source.draftsOnly : null);
			filter = source.filter;
			matchMode = source.matchMode;
			sort = source.sort;
			view = source.view;
			this.page = page;
		}

		public DiscType? discType { get; set; }
		public bool? draftsOnly { get; set; }
		public string filter { get; set; }
		public NameMatchMode? matchMode { get; set; }
		public int? page { get; set; }
		public AlbumSortRule? sort { get; set; }
		public EntryViewMode? view { get; set; }
	}
}