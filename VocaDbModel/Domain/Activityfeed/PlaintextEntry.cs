using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Activityfeed {

	public class PlaintextEntry : ActivityEntry {

		private string text;

		public PlaintextEntry() {}

		public PlaintextEntry(User author, bool sticky, string text)
			: base(author, sticky) {

			Text = text;

		}

		public override ActivityEntryType EntryType {
			get { 
				return ActivityEntryType.Plaintext; 
			}
		}

		public virtual string Text {
			get { return text; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				text = value;
			}
		}

		public override void Accept(IActivityEntryVisitor visitor) {
			visitor.Visit(this);
		}

	}

}
