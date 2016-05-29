using System;

namespace VocaDb.Model.Domain.Users {

	public class OldUsername : IEntryWithIntId {

		public OldUsername() {
			Date = DateTime.Now;
		}

		public OldUsername(User user, string oldName) : this() {
			User = user;
			OldName = oldName;
		}

		public virtual DateTime Date { get; set; }

		public virtual int Id { get; set; }

		public virtual string OldName { get; set; }

		public virtual User User { get; set; }

	}

}
