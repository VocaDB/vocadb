namespace VocaDb.Model.Domain.Albums {

	public class ReleaseEventSeriesAlias {

		private string name;
		private ReleaseEventSeries series;

		public ReleaseEventSeriesAlias() { }

		public ReleaseEventSeriesAlias(ReleaseEventSeries series, string name) {

			Series = series;
			Name = name;

		}

		public virtual int Id { get; set; }

		public virtual string Name {
			get { return name; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				name = value; 
			}
		}

		public virtual ReleaseEventSeries Series {
			get { return series; }
			set {
				ParamIs.NotNull(() => value);
				series = value; 
			}
		}

		public virtual bool Equals(ReleaseEventSeriesAlias another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as ReleaseEventSeriesAlias);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

	}
}
