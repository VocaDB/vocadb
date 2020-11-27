using System;
using System.ComponentModel.DataAnnotations;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Comments;

namespace VocaDb.Model.DataContracts.Albums
{
	public class AlbumReviewContract
	{
		public AlbumReviewContract()
		{
			Title = string.Empty;
		}

		public AlbumReviewContract(AlbumReview review, IUserIconFactory userIconFactory) : this()
		{
			if (review == null)
				throw new ArgumentNullException(nameof(review));

			Id = review.Id;
			AlbumId = review.EntryForComment.Id;
			Date = review.Created;
			LanguageCode = review.LanguageCode;
			Text = review.Message;
			Title = review.Title;
			User = new UserForApiContract(review.Author, userIconFactory, UserOptionalFields.MainPicture);
		}

		public virtual int Id { get; set; }

		public virtual int AlbumId { get; set; }

		public virtual DateTime Date { get; set; }

		public virtual string LanguageCode { get; set; }

		public virtual string Text { get; set; }

		/// <summary>
		/// See https://stackoverflow.com/a/9323209
		/// </summary>
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public virtual string Title { get; set; }

		public virtual UserForApiContract User { get; set; }
	}
}
