namespace VocaDb.Model.Domain.Tags {

	public class TagMapping {

		public TagMapping() { }

		public TagMapping(Tag tag, string sourceTag) {
			Tag = tag;
			SourceTag = sourceTag;
		}

		private Tag tag;
		private string sourceTag;

		public int Id { get; set; }

		public string SourceTag {
			get { return sourceTag; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				sourceTag = value;
			}
		}

		public Tag Tag {
			get { return tag; }
			set {
				ParamIs.NotNull(() => value);
				tag = value;
			}
		}

	}

}
