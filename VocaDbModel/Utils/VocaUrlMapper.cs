using VocaDb.Model.Utils.Search;

namespace VocaDb.Model.Utils {

	public class VocaUrlMapper {

		private readonly bool ssl;

		public VocaUrlMapper(bool ssl) {
			this.ssl = ssl;
		}

		/// <summary>
		/// Host address including scheme, for example http://vocadb.net.
		/// Does not include the trailing slash.
		/// </summary>
		public string HostAddress {
			get {
				return VocaUriBuilder.HostAddress(ssl);
			}
		}

		public string FullAbsolute(string relative) {
			return VocaUriBuilder.Absolute(relative, ssl);
		}

		public SearchRouteParamsFactory Search {
			get {
				return new SearchRouteParamsFactory();
			}
		}

	}

}
