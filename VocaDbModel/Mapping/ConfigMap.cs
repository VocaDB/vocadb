#nullable disable

using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Mapping;

public class ConfigMap : ClassMap<Config>
{
	public ConfigMap()
	{
		Cache.ReadWrite();
		Id(m => m.Id);

		Map(m => m.Type).Not.Nullable().Length(50).Unique();
		Map(m => m.Value).Length(int.MaxValue);
		Map(m => m.Updated).Not.Nullable();
	}
}
