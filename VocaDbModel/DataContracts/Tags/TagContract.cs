using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagContract : TagBaseContract {

		public TagContract() {
			AliasedTo = null;
		}

		public TagContract(Tag tag, ContentLanguagePreference languagePreference, bool includeAdditionalNames = false)
			: base(tag, languagePreference, includeAdditionalNames) {

			ParamIs.NotNull(() => tag);

			AliasedTo = tag.AliasedTo != null ? new TagBaseContract(tag.AliasedTo, languagePreference) : null;
			CategoryName = tag.CategoryName;
			CreateDate = tag.CreateDate;
			Deleted = tag.Deleted;
			Description = tag.Description.GetBestMatch(languagePreference);
			Parent = tag.Parent != null ? new TagBaseContract(tag.Parent, languagePreference) : null;
			Status = tag.Status;
			Version = tag.Version;

		}

		[DataMember]
		[Obsolete("Tag aliases are now just names")]
		public TagBaseContract AliasedTo { get; set; }

		[DataMember]
		public string CategoryName { get; set; }

		/// <summary>
		/// Date this entry was created.
		/// </summary>
		[DataMember]
		public DateTime CreateDate { get; set; }

		public bool Deleted { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public TagBaseContract Parent { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember]
		public int Version { get; set; }

	}
}
