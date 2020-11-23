using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Mapping.ReleaseEvents
{

	public class PVForEventMap : ClassMap<PVForEvent>
	{

		public PVForEventMap()
		{

			Table("PVsForEvents");
			Id(m => m.Id);

			Map(m => m.Author).Length(100).Not.Nullable();
			Map(m => m.Name).Length(200).Not.Nullable();
			Map(m => m.PublishDate).Nullable();
			Map(m => m.PVId).Length(255).Not.Nullable();
			Map(m => m.PVType).Not.Nullable();
			Map(m => m.Service).Not.Nullable();

			References(m => m.Entry).Column("[Event]").Not.Nullable();

			Component(m => m.ExtendedMetadata, c =>
			{
				c.Map(m => m.Json, "ExtendedMetadataJson").Nullable();
			});

		}

	}

}
