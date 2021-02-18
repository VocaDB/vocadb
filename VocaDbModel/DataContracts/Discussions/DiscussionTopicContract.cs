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
	public class DiscussionTopicContract
	{
		public DiscussionTopicContract() { }

		public DiscussionTopicContract(DiscussionTopic topic, IUserIconFactory userIconFactory, DiscussionTopicOptionalFields fields)
		{
			ParamIs.NotNull(() => topic);

			Author = new UserForApiContract(topic.Author, userIconFactory, UserOptionalFields.MainPicture);
			Created = topic.Created.ToUniversalTime();
			FolderId = topic.Folder.Id;
			Id = topic.Id;
			Locked = topic.Locked;
			Name = topic.Name;

			if (fields.HasFlag(DiscussionTopicOptionalFields.Comments))
			{
				Comments = topic.Comments.Select(c => new CommentForApiContract(c, userIconFactory)).ToArray();
			}

			if (fields.HasFlag(DiscussionTopicOptionalFields.CommentCount))
			{
				CommentCount = topic.Comments.Count();
			}

			if (fields.HasFlag(DiscussionTopicOptionalFields.Content))
			{
				Content = topic.Content;
			}

			if (fields.HasFlag(DiscussionTopicOptionalFields.LastComment) && topic.Comments.Any())
			{
				LastComment = new CommentForApiContract(topic.Comments.ToArray().MaxItem(c => c.Created),
					userIconFactory, includeMessage: false);
			}
		}

		[DataMember]
		public UserForApiContract Author { get; set; }

		[DataMember]
		public int CommentCount { get; set; }

		[DataMember]
		public CommentForApiContract[] Comments { get; set; }

		[DataMember]
		public string Content { get; set; }

		/// <summary>
		/// Date and time when this topic was posted, in UTC.
		/// </summary>
		[DataMember]
		public DateTime Created { get; set; }

		[DataMember]
		public int FolderId { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public CommentForApiContract LastComment { get; set; }

		[DataMember]
		public bool Locked { get; set; }

		[DataMember]
		public string Name { get; set; }

		public override string ToString()
		{
			return $"{Name} [{Id}] at {Created}";
		}
	}

	[Flags]
	public enum DiscussionTopicOptionalFields
	{
		None = 0,

		Comments = 1 << 0,
		CommentCount = 1 << 1,
		Content = 1 << 2,
		LastComment = 1 << 3,

		All = Comments | CommentCount | Content | LastComment,
	}
}
