using System;

namespace VocaDb.Model.Domain.Tags {

	/// <summary>
	/// Defines tag mapping from an external source system such as NicoNicoDouga to VocaDB.
	/// </summary>
	public class TagMapping {

		public TagMapping() {
			CreateDate = DateTime.Now;
		}

		public TagMapping(Tag tag, string sourceTag) : this() {

			ParamIs.NotNull(() => tag);
			ParamIs.NotNullOrEmpty(() => sourceTag);

			Tag = tag;
			SourceTag = sourceTag;

		}

		private Tag tag;
		private string sourceTag;

		public virtual DateTime CreateDate { get; set; }

		public virtual int Id { get; set; }

		/// <summary>
		/// Tag name in the source system.
		/// For example "VOCAROCK".
		/// </summary>
		public virtual string SourceTag {
			get => sourceTag;
			set {
				ParamIs.NotNullOrEmpty(() => value);
				sourceTag = value;
			}
		}

		/// <summary>
		/// VocaDB tag. Cannot be null.
		/// </summary>
		public virtual Tag Tag {
			get => tag;
			set {
				ParamIs.NotNull(() => value);
				tag = value;
			}
		}

	}

}
