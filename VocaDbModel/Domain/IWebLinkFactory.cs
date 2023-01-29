using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Domain;

public interface IWebLinkFactory<out T> where T : WebLink
{
	T CreateWebLink(string description, WebAddress address, WebLinkCategory category, bool disabled);
}
