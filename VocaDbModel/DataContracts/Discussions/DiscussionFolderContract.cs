using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.Discussions {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class DiscussionFolderContract {

		public DiscussionFolderContract() { }

		public DiscussionFolderContract(DiscussionFolder folder, DiscussionFolderOptionalFields fields,
			IUserIconFactory userIconFactory) {

			ParamIs.NotNull(() => folder);

			this.Description = folder.Description;
			this.Id = folder.Id;
			this.Name = folder.Name;

			if (fields.HasFlag(DiscussionFolderOptionalFields.LastTopic) && folder.Topics.Any()) {

				var lastTopic = folder.Topics.ToArray().MaxItem(t => t.Created);

				LastTopicAuthor = new UserWithIconContract(lastTopic.Author, lastTopic.AuthorName, userIconFactory);
				LastTopicDate = folder.Topics.Max(t => t.Created);

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
		public UserWithIconContract LastTopicAuthor { get; set; }

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
		LastTopic		= 1,
		TopicCount		= 2

	}

}
