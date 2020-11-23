using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Mapping.ReleaseEvents
{

	public class ArchivedReleaseEventSeriesVersionMap : ClassMap<ArchivedReleaseEventSeriesVersion>
	{

		public ArchivedReleaseEventSeriesVersionMap()
		{

			Id(m => m.Id);
			Table("ArchivedEventSeriesVersions");

			Map(m => m.CommonEditEvent).Length(30).Not.Nullable();
			Map(m => m.Created).Not.Nullable();
			Map(m => m.Data).Not.Nullable();
			Map(m => m.Hidden).Not.Nullable();
			Map(m => m.Notes).Length(200).Not.Nullable();
			Map(m => m.Version).Not.Nullable();

			References(m => m.Author).Not.Nullable();
			References(m => m.Entry).Column("[Series]").Not.Nullable();

			Component(m => m.Diff, c =>
			{
				c.Map(m => m.ChangedFieldsString, "ChangedFields").Length(100).Not.Nullable();
			});

		}

	}

}
