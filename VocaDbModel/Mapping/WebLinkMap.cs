using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Mapping {

	public class WebLinkMap<TLink, TEntry> : ClassMap<TLink> where TLink : GenericWebLink<TEntry> where TEntry : class {

		public WebLinkMap(string foreignKeyColumn = null) {

			Cache.ReadWrite();
			Id(m => m.Id);

			Map(m => m.Description).Length(512).Not.Nullable();
			Map(m => m.Url).Length(512).Not.Nullable();

			References(m => m.Entry).Column(foreignKeyColumn ?? typeof(TEntry).Name).Not.Nullable();

		}

	}

}
