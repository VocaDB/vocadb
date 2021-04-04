#nullable disable

using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagContract : TagBaseContract
	{
		public TagContract()
		{
		}

#nullable enable
		public TagContract(Tag tag, ContentLanguagePreference languagePreference, bool includeAdditionalNames = false)
			: base(tag, languagePreference, includeAdditionalNames, true)
		{
			ParamIs.NotNull(() => tag);

			CreateDate = tag.CreateDate;
			Deleted = tag.Deleted;
			Description = tag.Description.GetBestMatch(languagePreference);
			HideFromSuggestions = tag.HideFromSuggestions;
			Parent = tag.Parent != null ? new TagBaseContract(tag.Parent, languagePreference) : null;
			Status = tag.Status;
			Targets = tag.Targets;
			Version = tag.Version;
		}
#nullable disable

		/// <summary>
		/// Date this entry was created.
		/// </summary>
		[DataMember]
		public DateTime CreateDate { get; init; }

		public bool Deleted { get; init; }

		[DataMember]
		public string Description { get; init; }

		[DataMember]
		public bool HideFromSuggestions { get; init; }

		[DataMember]
		public TagBaseContract Parent { get; set; }

		[DataMember]
		public EntryStatus Status { get; init; }

		[DataMember]
		public TagTargetTypes Targets { get; init; }

		[DataMember]
		public int Version { get; init; }
	}
}
