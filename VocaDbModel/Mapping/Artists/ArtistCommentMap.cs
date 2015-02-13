using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Mapping.Artists {

	public class ArtistCommentMap : ClassMap<ArtistComment> {

		public ArtistCommentMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Message).Length(4000).Not.Nullable();

			References(m => m.Artist).Not.Nullable();
			References(m => m.Author).Not.Nullable();

		}

	}

}
