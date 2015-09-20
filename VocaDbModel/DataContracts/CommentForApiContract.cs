using System;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Comments;

namespace VocaDb.Model.DataContracts {

	[DataContract]
	public class CommentForApiContract : IComment {

		public CommentForApiContract() { }

		public CommentForApiContract(Comment comment, IUserIconFactory iconFactory, bool includeMessage = true) {
			
			ParamIs.NotNull(() => comment);

			Author = comment.Author != null ? new UserWithIconContract(comment.Author, iconFactory) : null;
			AuthorName = comment.AuthorName;
			Created = comment.Created.ToUniversalTime();
			Id = comment.Id;
			Message = (includeMessage ? comment.Message : null);

		}

		[DataMember]
		public UserWithIconContract Author { get; set; }

		[DataMember]
		public string AuthorName { get; set; }

		/// <summary>
		/// Comment creation date in UTC.
		/// </summary>
		[DataMember]
		public DateTime Created { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Message { get; set; }

	}

}
