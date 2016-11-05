namespace VocaDb.Model.Domain.Tags {

	public class TagMapping {

		public TagMapping() { }

		public TagMapping(Tag tag, string sourceTag) {
			Tag = tag;
			SourceTag = sourceTag;
		}

		private Tag tag;
		private string sourceTag;

		public virtual int Id { get; set; }

		public virtual string SourceTag {
			get { return sourceTag; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				sourceTag = value;
			}
		}

		public virtual Tag Tag {
			get { return tag; }
			set {
				ParamIs.NotNull(() => value);
				tag = value;
			}
		}

	}

}
