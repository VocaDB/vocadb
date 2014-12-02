using System;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.DataContracts.Activityfeed {

	public class NewsEntryContract {

		public NewsEntryContract() { }

		public NewsEntryContract(NewsEntry newsEntry) {

			ParamIs.NotNull(() => newsEntry);

			Anonymous = newsEntry.Anonymous;
			Author = new UserContract(newsEntry.Author);
			CreateDate = newsEntry.CreateDate;
			Id = newsEntry.Id;
			Important = newsEntry.Important;
			Stickied = newsEntry.Stickied;
			Text = newsEntry.Text;

		}

		public bool Anonymous { get; set; }

		public UserContract Author { get; set; }

		public DateTime CreateDate { get; set; }

		public int Id { get; set; }

		public bool Important { get; set; }

		public bool Stickied { get; set; }

		public string Text { get; set; }

	}

}
