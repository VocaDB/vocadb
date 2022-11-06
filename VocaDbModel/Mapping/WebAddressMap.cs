using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Mapping;

public class WebAddressHostMap : ClassMap<WebAddressHost>
{
	public WebAddressHostMap()
	{
		Cache.ReadWrite();
		Id(m => m.Id);

		Map(m => m.Hostname).Not.Nullable();
		Map(m => m.ReferenceCount).Not.Nullable();

		References(m => m.Actor).Not.Nullable();
	}
}

public class WebAddressMap : ClassMap<WebAddress>
{
	public WebAddressMap()
	{
		Cache.ReadWrite();
		Id(m => m.Id);

		Map(m => m.Url).Length(512).Not.Nullable();
		Map(m => m.Scheme).Not.Nullable();
		Map(m => m.Port).Not.Nullable();
		Map(m => m.Path).Length(512).Not.Nullable();
		Map(m => m.Query).Length(512).Not.Nullable();
		Map(m => m.Fragment).Length(512).Not.Nullable();
		Map(m => m.ReferenceCount).Not.Nullable();

		References(m => m.Host).Not.Nullable();
		References(m => m.Actor).Not.Nullable();
	}
}
