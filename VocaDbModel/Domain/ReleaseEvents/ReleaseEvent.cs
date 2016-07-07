using System;
using System.Collections.Generic;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class ReleaseEvent : IEntryWithNames {

		string IEntryBase.DefaultName {
			get { return Name; }
		}

		bool IDeletableEntry.Deleted {
			get { return false; }
		}

		INameManager IEntryWithNames.Names {
			get { 
				return new SingleNameManager(Name); 
			}
		}

		private IList<Album> albums = new List<Album>();
		private ArchivedVersionManager<ArchivedReleaseEventVersion, ReleaseEventEditableFields> archivedVersions
			= new ArchivedVersionManager<ArchivedReleaseEventVersion, ReleaseEventEditableFields>();
		private string description;
		private string name;
		private ReleaseEventSeries series;
		private string seriesSuffix;

		public ReleaseEvent() {
			Description = SeriesSuffix = string.Empty;
		}

		public ReleaseEvent(string description, DateTime? date, string name)
			: this() {

			ParamIs.NotNullOrEmpty(() => name);

			Description = description;
			Date = date;
			Name = name;

		}

		public ReleaseEvent(string description, DateTime? date, ReleaseEventSeries series, int seriesNumber, string seriesSuffix,
			string name, bool customName)
			: this() {

			ParamIs.NotNull(() => series);

			Description = description;
			Date = date;
			Series = series;
			SeriesNumber = seriesNumber;
			SeriesSuffix = seriesSuffix;
			CustomName = customName;

			if (!string.IsNullOrWhiteSpace(name)) {
				Name = name;
			}

			UpdateNameFromSeries();

		}

		public virtual IList<Album> Albums {
			get { return albums; }
			set {
				ParamIs.NotNull(() => value);
				albums = value; 
			}
		}

		public virtual ArchivedVersionManager<ArchivedReleaseEventVersion, ReleaseEventEditableFields> ArchivedVersionsManager {
			get { return archivedVersions; }
			set {
				ParamIs.NotNull(() => value);
				archivedVersions = value;
			}
		}

		public virtual bool CustomName { get; set; }

		public virtual Date Date { get; set; }

		public virtual string Description {
			get { return description; }
			set {
				ParamIs.NotNull(() => value);
				description = value; 
			}
		}

		public virtual EntryType EntryType {
			get { return EntryType.ReleaseEvent; }
		}

		public virtual int Id { get; set; }

		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNullOrWhiteSpace(() => value);
				name = value; 
			}
		}

		public virtual ReleaseEventSeries Series {
			get { return series; }
			set { series = value; }
		}

		public virtual int SeriesNumber { get; set; }

		public virtual string SeriesSuffix {
			get { return seriesSuffix; }
			set {
				ParamIs.NotNull(() => value);
				seriesSuffix = value;
			}
		}

		public virtual int Version { get; set; }

		public virtual ArchivedReleaseEventVersion CreateArchivedVersion(ReleaseEventDiff diff, AgentLoginData author, EntryEditEvent reason) {

			var archived = new ArchivedReleaseEventVersion(this, diff, author, reason);
			ArchivedVersionsManager.Add(archived);
			Version++;

			return archived;

		}

		public virtual bool Equals(ReleaseEvent another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ReleaseEvent);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override string ToString() {
			return string.Format("Release event '{0}' [{1}]", Name, Id);
		}

		public virtual void UpdateNameFromSeries() {

			if (Series != null && !CustomName) {
				Name = Series.GetEventName(SeriesNumber, SeriesSuffix);				
			}
			
		}

	}
}
