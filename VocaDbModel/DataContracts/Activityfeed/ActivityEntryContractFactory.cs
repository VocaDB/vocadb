using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.DataContracts.Activityfeed {

	/*public class ActivityEntryContractFactory : IActivityEntryVisitor {

		private ActivityEntryContract contract;
		private readonly Func<EntryRef, EntryName> entryNameGetter;

		void IActivityEntryVisitor.Visit(AlbumActivityEntry entry) {
			var name = entryNameGetter(entry.EntryRef);
			contract = new ActivityEntryContract(entry, name);
		}

		void IActivityEntryVisitor.Visit(PlaintextEntry entry) {
			contract = new ActivityEntryContract(entry);
		}

		public ActivityEntryContractFactory(Func<EntryRef, EntryName> entryNameGetter) {
			this.entryNameGetter = entryNameGetter;
		}

		public ActivityEntryContract Create(ActivityEntry entry) {
			entry.Accept(this);
			return contract;
		}*/

	}

	public class EntryName {

		public static EntryName Empty {
			get {
				return new EntryName(string.Empty);
			}
		}

		public EntryName(string displayName, string additionalNames) {
			DisplayName = displayName;
			AdditionalNames = additionalNames;
		}

		public EntryName(string displayName)
			: this(displayName, string.Empty) {}

		public string AdditionalNames { get; set; }

		public string DisplayName { get; set; }

	}

}
