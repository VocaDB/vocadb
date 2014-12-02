using System;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Activityfeed {

	public class NewsEntry {

		private string text;
		private User author;

		public NewsEntry() {
			Anonymous = true;
			CreateDate = DateTime.Now;
			Stickied = false;
		}

		public NewsEntry(string text, User author, bool anonymous, bool important, bool stickied)
			: this() {

			Text = text;
			Author = author;
			Anonymous = anonymous;
			Important = important;
			Stickied = stickied;

		}

		public virtual bool Anonymous { get; set; }

		public virtual User Author {
			get { return author; }
			set {
				ParamIs.NotNull(() => value);
				author = value;
			}
		}

		public virtual DateTime CreateDate { get; set; }

		public virtual int Id { get; set; }

		public virtual bool Important { get; set; }

		public virtual bool Stickied { get; set; }

		public virtual string Text {
			get { return text; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				text = value;
			}
		}

		public override string ToString() {
			return string.Format("news entry [{0}]", Id);
		}

	}
}
