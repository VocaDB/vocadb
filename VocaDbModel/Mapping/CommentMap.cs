using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Comments;

namespace VocaDb.Model.Mapping {

	public abstract class CommentMap<TComment, TEntry> : ClassMap<TComment> where TComment : GenericComment<TEntry> where TEntry : class, IEntryWithNames {

		protected CommentMap() {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Created).Not.Nullable();
			Map(m => m.Message).Length(4000).Not.Nullable();

			References(m => m.Author).Not.Nullable();

		}

	}

}
