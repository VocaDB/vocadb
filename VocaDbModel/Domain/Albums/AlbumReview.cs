using System;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Albums
{

	public class AlbumReview : IAlbumLink, IEntryWithIntId
	{

		public AlbumReview() { }

		public AlbumReview(Album album, User user, string title, string text, string languageCode)
		{
			Album = album ?? throw new ArgumentNullException(nameof(album));
			User = user ?? throw new ArgumentNullException(nameof(user));
			Title = title ?? string.Empty;
			Text = text ?? throw new ArgumentNullException(nameof(text));
			LanguageCode = languageCode ?? string.Empty;
			Date = DateTime.UtcNow;
		}

		public virtual int Id { get; set; }

		public virtual Album Album { get; set; }

		public virtual DateTime Date { get; set; }

		public virtual string LanguageCode { get; set; }

		public virtual string Text { get; set; }

		public virtual string Title { get; set; }

		public virtual User User { get; set; }

	}

}
