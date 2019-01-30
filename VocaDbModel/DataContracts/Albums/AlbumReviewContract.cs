using System;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums {

	public class AlbumReviewContract {

		public AlbumReviewContract() { }

		public AlbumReviewContract(AlbumReview review, IUserIconFactory userIconFactory) {

			if (review == null)
				throw new ArgumentNullException(nameof(review));

			Id = review.Id;
			AlbumId = review.Album.Id;
			Date = review.Date;
			LanguageCode = review.LanguageCode;
			Text = review.Text;
			Title = review.Title;
			User = new UserForApiContract(review.User, userIconFactory, UserOptionalFields.MainPicture);

		}

		public virtual int Id { get; set; }

		public virtual int AlbumId { get; set; }

		public virtual DateTime Date { get; set; }

		public virtual string LanguageCode { get; set; }

		public virtual string Text { get; set; }

		public virtual string Title { get; set; }

		public virtual UserForApiContract User { get; set; }

	}

}
