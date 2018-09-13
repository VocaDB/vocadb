using VocaDb.Model.Utils.Search;

namespace VocaDb.Model.Utils {

	public class VocaUrlMapper {

		/// <summary>
		/// Host address including scheme, for example http://vocadb.net.
		/// Does not include the trailing slash.
		/// </summary>
		public string HostAddress => VocaUriBuilder.HostAddress;

		public string FullAbsolute(string relative) => VocaUriBuilder.Absolute(relative);

		public SearchRouteParamsFactory Search => new SearchRouteParamsFactory();

	}

}
