using System;
using System.Collections.Generic;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class ReleaseEventSeries : IEntryBase, IEquatable<ReleaseEventSeries> {

		string IEntryBase.DefaultName {
			get { return Name; }
		}

		bool IDeletableEntry.Deleted {
			get { return false; }
		}

		int IEntryBase.Version {
			get { return 0; }
		}

		public static ImageSizes ImageSizes = ImageSizes.Original | ImageSizes.SmallThumb | ImageSizes.TinyThumb;

		private IList<ReleaseEventSeriesAlias> aliases = new List<ReleaseEventSeriesAlias>();
		private string description;
		private IList<ReleaseEvent> events = new List<ReleaseEvent>();
		private string name;

		public ReleaseEventSeries() {
			Description = string.Empty;
		}

		public ReleaseEventSeries(string name, string description, IEnumerable<string> aliases)
			: this() {

			ParamIs.NotNull(() => aliases);

			Name = name;
			Description = description;

			foreach (var a in aliases)
				CreateAlias(a);

		}

		public virtual IList<ReleaseEventSeriesAlias> Aliases {
			get { return aliases; }
			set {
				ParamIs.NotNull(() => value);
				aliases = value; 
			}
		}

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value; 
			}
		}

		public virtual EntryType EntryType {
			get { return EntryType.ReleaseEventSeries; }
		}

		public virtual IList<ReleaseEvent> Events {
			get { return events; }
			set {
				ParamIs.NotNull(() => value);
				events = value; 
			}
		}

		public virtual int Id { get; set; }

		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				name = value; 
			}
		}

		public virtual string PictureMime { get; set; }

		public virtual ReleaseEventSeriesAlias CreateAlias(string alias) {

			var a = new ReleaseEventSeriesAlias(this, alias);
			Aliases.Add(a);

			return a;

		}

		public virtual bool Equals(ReleaseEventSeries another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ReleaseEventSeries);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual string GetEventName(int number, string suffix) {

			if (string.IsNullOrEmpty(suffix)) {
				return string.Format("{0} {1}", Name, number);
			} else {
				return string.Format("{0} {1} {2}", Name, number, suffix);
			}

		}

		public override string ToString() {
			return string.Format("release event series '{0}' [{1}]", Name, Id);
		}

		public virtual void UpdateAliases(IEnumerable<string> aliases) {

			ParamIs.NotNull(() => aliases);

			var diff = CollectionHelper.Diff(Aliases, aliases, (a1, a2) => a1.Name.Equals(a2));

			foreach (var added in diff.Added)
				CreateAlias(added);

			foreach (var removed in diff.Removed)
				Aliases.Remove(removed);

		}

	}

}
