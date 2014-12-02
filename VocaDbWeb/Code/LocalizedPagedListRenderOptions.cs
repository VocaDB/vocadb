using PagedList.Mvc;
using VocaDb.Web.Resources.Other;

namespace VocaDb.Web.Code {

	public static class LocalizedPagedListRenderOptions {

		public static PagedListRenderOptions Instance {
			get {
				return new PagedListRenderOptions {
					LinkToFirstPageFormat = "<< " + PagedListStrings.First,
					LinkToLastPageFormat = PagedListStrings.Last + " >>",
					LinkToPreviousPageFormat = "< " + PagedListStrings.Previous,
					LinkToNextPageFormat = PagedListStrings.Next + " >",
					ContainerDivClasses = new[] { "pagination" },
					UlElementClasses = new string[0]
				};
			}
		}

	}
}