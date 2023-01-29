using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.ExtLinks;

public class WebLinkManager<T> where T : WebLink
{
	private IList<T> _links = new List<T>();

	public virtual bool HasLink(string url)
	{
		return Links.Any(l => l.Url == url);
	}

	public virtual IList<T> Links
	{
		get => _links;
		set
		{
			ParamIs.NotNull(() => value);
			_links = value;
		}
	}

	public CollectionDiffWithValue<T, T> Sync(IDatabaseContext ctx, IEnumerable<IWebLinkContract> newLinks, IWebLinkFactory<T> webLinkFactory, User actor)
	{
		return WebLink.Sync(ctx, Links, newLinks, webLinkFactory, actor);
	}

	public CollectionDiff<T, T> SyncByValue(IDatabaseContext ctx, IEnumerable<ArchivedWebLinkContract> newLinks, IWebLinkFactory<T> webLinkFactory, User actor)
	{
		return WebLink.SyncByValue(ctx, oldLinks: Links, newLinks, webLinkFactory, actor);
	}
}
