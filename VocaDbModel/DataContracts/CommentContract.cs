using System;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts {

	/// <summary>
	/// Data contract for <see cref="Comment"/>.
	/// SECURITY NOTE: User contract contains email, which is sensitive information.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class CommentContract : IComment {

		public CommentContract() { }

		public CommentContract(Comment comment) {

			ParamIs.NotNull(() => comment);

			Author = (comment.Author != null ? new UserWithEmailContract(comment.Author) : null);
			AuthorName = comment.AuthorName;
			Created = comment.Created;
			Id = comment.Id;
			Message = comment.Message;

		}

		[DataMember]
		public UserWithEmailContract Author { get; set; }

		[DataMember]
		public string AuthorName { get; set; }

		[DataMember]
		public DateTime Created { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Message { get; set; }

	}

}
