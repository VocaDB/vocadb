using FluentNHibernate.Mapping;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Mapping
{
	public sealed class WebhookMap : ClassMap<Webhook>
	{
		public WebhookMap()
		{
			Id(m => m.Id);

			Map(m => m.Url).Not.Nullable();
			Map(m => m.WebhookEvents).CustomType<WebhookEvents>().Not.Nullable();
		}
	}
}
