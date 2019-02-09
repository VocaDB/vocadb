using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Mapping.Albums {

	public class AlbumReviewMap : ClassMap<AlbumReview> {
		public AlbumReviewMap() {

			Table("AlbumReviews");
			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Date).Not.Nullable();
			Map(m => m.LanguageCode).Not.Nullable().UniqueKey("UX_AlbumReviews");
			Map(m => m.Text).Not.Nullable();
			Map(m => m.Title).Not.Nullable();

			References(m => m.Album).Not.Nullable().UniqueKey("UX_AlbumReviews");
			References(m => m.User).Not.Nullable().UniqueKey("UX_AlbumReviews");

		}

	}
}
