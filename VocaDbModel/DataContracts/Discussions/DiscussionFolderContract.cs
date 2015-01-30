using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Discussions;

namespace VocaDb.Model.DataContracts.Discussions {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class DiscussionFolderContract {

		public DiscussionFolderContract() { }

		public DiscussionFolderContract(DiscussionFolder folder, DiscussionFolderOptionalFields fields) {

			ParamIs.NotNull(() => folder);

			this.Description = folder.Description;
			this.Id = folder.Id;
			this.Name = folder.Name;

			if (fields.HasFlag(DiscussionFolderOptionalFields.LastTopicDate)) {
				this.LastTopicDate = folder.Topics.Any() ? (DateTime?)folder.Topics.Max(t => t.Created) : null; 				
			}

			if (fields.HasFlag(DiscussionFolderOptionalFields.TopicCount)) {
				this.TopicCount = folder.Topics.Count();				
			}

		}

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public DateTime? LastTopicDate { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public int TopicCount { get; set; }

	}

	[Flags]
	public enum DiscussionFolderOptionalFields {
		
		None			= 0,
		LastTopicDate	= 1,
		TopicCount		= 2

	}

}
