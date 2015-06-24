
using System.Linq;

namespace VocaDb.Model.Service.Search {

	public struct EntryIdsCollection {

		public static EntryIdsCollection CreateWithFallback(int[] ids, int fallbackId) {
			return new EntryIdsCollection(ids != null && ids.Any() ? ids : new [] { fallbackId });
		}

		public EntryIdsCollection(int[] ids) : this() {
			Ids = ids;
		}

		public bool HasAny {
			get {
				return Ids != null && Ids.Any();
			}
		}

		public bool HasMultiple {
			get {
				return Ids != null && Ids.Length > 1;
			}
		}

		public bool HasSingleId {
			get {
				return Ids != null && Ids.Length == 1;
			}
		}

		public int[] Ids { get; private set; }

		public int Primary {
			get {
				return HasSingleId ? Ids[0] : 0;				
			}
		}

	}

}
