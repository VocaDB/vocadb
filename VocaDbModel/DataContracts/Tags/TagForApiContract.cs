using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagForApiContract {

		public TagForApiContract() { }

		public TagForApiContract(Tag tag, 
			IEntryImagePersisterOld thumbPersister,
			bool ssl,			
			TagOptionalFields optionalFields) {
			
			AliasedToName = tag.AliasedTo != null ? tag.AliasedTo.Name : null;
			CategoryName = tag.CategoryName;
			Id = tag.Id;
			Name = tag.Name;
			ParentName = tag.Parent != null ? tag.Parent.Name : null;
			Status = tag.Status;
			Version = tag.Version;

			if (optionalFields.HasFlag(TagOptionalFields.Description)) {
				Description = tag.Description;
			}

			if (optionalFields.HasFlag(TagOptionalFields.MainPicture) && tag.Thumb != null) {
				MainPicture = new EntryThumbForApiContract(tag.Thumb, thumbPersister, ssl);
			}

		}

		[DataMember]
		public string AliasedToName { get; set; }

		[DataMember]
		public string CategoryName { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string ParentName { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryStatus Status { get; set; }

		[DataMember]
		public int Version { get; set; }

	}

	[Flags]
	public enum TagOptionalFields {

		None		= 0,
		Description = 1,
		MainPicture = 2,

	}

}
