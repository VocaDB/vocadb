#nullable disable

using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.Discussions
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class DiscussionFolderContract
	{
		public DiscussionFolderContract() { }

		public DiscussionFolderContract(DiscussionFolder folder, DiscussionFolderOptionalFields fields,
			IUserIconFactory userIconFactory)
		{
			ParamIs.NotNull(() => folder);

			Description = folder.Description;
			Id = folder.Id;
			Name = folder.Name;

			if (fields.HasFlag(DiscussionFolderOptionalFields.LastTopic) && folder.Topics.Any())
			{
				var lastTopic = folder.Topics.ToArray().MaxItem(t => t.Created);

				LastTopicAuthor = new UserForApiContract(lastTopic.Author, lastTopic.AuthorName, userIconFactory, UserOptionalFields.MainPicture);
				LastTopicDate = folder.Topics.Max(t => t.Created).ToUniversalTime();
			}

			if (fields.HasFlag(DiscussionFolderOptionalFields.TopicCount))
			{
				TopicCount = folder.Topics.Count();
			}
		}

		[DataMember]
		public string Description { get; init; }

		[DataMember]
		public int Id { get; init; }

		[DataMember]
		public UserForApiContract LastTopicAuthor { get; init; }

		/// <summary>
		/// Date of the latest topic posted, in UTC.
		/// </summary>
		[DataMember]
		public DateTime? LastTopicDate { get; init; }

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		public int TopicCount { get; init; }
	}

	[Flags]
	public enum DiscussionFolderOptionalFields
	{
		None = 0,
		LastTopic = 1 << 0,
		TopicCount = 1 << 1,
	}
}
