using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocaDb.Model.DataContracts.Comments {

	public class EntryWithCommentsContract {

		public UnifiedCommentContract[] Comments { get; set; }

		public EntryWithImageContract Entry { get; set; }

	}

}
