using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Discussions;

namespace VocaDb.Model.DataContracts.Discussions {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class DiscussionTopicContract {

		public DiscussionTopicContract() { }

		public DiscussionTopicContract(DiscussionTopic topic, IUserIconFactory userIconFactory) {
			
			ParamIs.NotNull(() => topic);

			Author = new UserWithIconContract(topic.Author, userIconFactory);
			CreateDate = topic.CreateDate;
			Id = topic.Id;
			Name = topic.Name;

			// TODO
			Comments = topic.Comments.Select(c => new CommentContract(c)).ToArray();
			CommentCount = topic.Comments.Count;
			Content = topic.Content;
			LastCommentDate = topic.Comments.Any() ? (DateTime?)topic.Comments.Max(t => t.Created) : null; 

		}

		[DataMember]
		public UserWithIconContract Author { get; set; }

		[DataMember]
		public int CommentCount { get; set; }

		[DataMember]
		public CommentContract[] Comments { get; set; }

		[DataMember]
		public string Content { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public DateTime? LastCommentDate { get; set; }

		[DataMember]
		public string Name { get; set; }

	}

}
