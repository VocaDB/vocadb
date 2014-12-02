using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Controllers {

    public class CommentController : ControllerBase {

		class EntryComparer : IEqualityComparer<EntryRefWithNameContract> {

			public bool Equals(EntryRefWithNameContract x, EntryRefWithNameContract y) {
				return x.EntryType == y.EntryType && x.Id == y.Id;
			}

			public int GetHashCode(EntryRefWithNameContract obj) {
				return obj.Id;
			}

		}

        //
        // GET: /Comment/

        public ActionResult Index()
        {

			var comments = Services.Other.GetRecentComments();
			var grouped = comments.GroupBy(c => c.Entry, new EntryComparer());

			return View(grouped);
        }

    }
}
