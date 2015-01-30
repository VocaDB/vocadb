using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Discussions;

namespace VocaDb.Model.DataContracts.Discussions {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class DiscussionTopicContract {

		public DiscussionTopicContract() { }

		public DiscussionTopicContract(DiscussionTopic topic, IUserIconFactory userIconFactory, DiscussionTopicOptionalFields fields) {
			
			ParamIs.NotNull(() => topic);

			Author = new UserWithIconContract(topic.Author, userIconFactory);
			Created = topic.Created;
			FolderId = topic.Folder.Id;
			Id = topic.Id;
			Name = topic.Name;

			if (fields.HasFlag(DiscussionTopicOptionalFields.Comments)) {
				Comments = topic.Comments.Select(c => new CommentForApiContract(c, userIconFactory)).ToArray();				
			}

			if (fields.HasFlag(DiscussionTopicOptionalFields.CommentCount)) {
				CommentCount = topic.Comments.Count;				
			}

			if (fields.HasFlag(DiscussionTopicOptionalFields.Content)) {
				Content = topic.Content;				
			}

			if (fields.HasFlag(DiscussionTopicOptionalFields.LastCommentDate)) {
				LastCommentDate = topic.Comments.Any() ? (DateTime?)topic.Comments.Max(t => t.Created) : null; 				
			}

		}

		[DataMember]
		public UserWithIconContract Author { get; set; }

		[DataMember]
		public int CommentCount { get; set; }

		[DataMember]
		public CommentForApiContract[] Comments { get; set; }

		[DataMember]
		public string Content { get; set; }

		[DataMember]
		public DateTime Created { get; set; }

		[DataMember]
		public int FolderId { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public DateTime? LastCommentDate { get; set; }

		[DataMember]
		public string Name { get; set; }

	}

	[Flags]
	public enum DiscussionTopicOptionalFields {
		
		None			= 0,

		Comments		= 1,
		CommentCount	= 2,
		Content			= 4,
		LastCommentDate = 8,

		All = (Comments | CommentCount | Content | LastCommentDate)

	}

}
