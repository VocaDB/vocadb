#nullable disable

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

		public virtual int Id { get; init; }

		public virtual int AlbumId { get; init; }

		public virtual DateTime Date { get; init; }

		public virtual string LanguageCode { get; init; }

		public virtual string Text { get; init; }

		/// <summary>
		/// See https://stackoverflow.com/a/9323209
		/// </summary>
		[DisplayFormat(ConvertEmptyStringToNull = false)]
		public virtual string Title { get; init; }

		public virtual UserForApiContract User { get; init; }
	}
}
