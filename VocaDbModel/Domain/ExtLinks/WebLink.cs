using System.Diagnostics.CodeAnalysis;
using NHibernate;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.ExtLinks;

public class WebLink : IWebLink, IEntryWithIntId
{
	private static WebAddressHost GetOrCreateWebAddressHost(IDatabaseContext ctx, Uri uri, User actor)
	{
		var host = ctx.Query<WebAddressHost>().FirstOrDefault(host => host.Hostname == uri.Host);
		if (host is not null)
		{
			return host;
		}
		else
		{
			host = new WebAddressHost(uri.Host, actor);
			ctx.Save(host);
			return host;
		}
	}

	// TODO: remove
	[Obsolete]
	private static WebAddressHost GetOrCreateWebAddressHost(ISession ctx, Uri uri, User actor)
	{
		var host = ctx.Query<WebAddressHost>().FirstOrDefault(host => host.Hostname == uri.Host);
		if (host is not null)
		{
			return host;
		}
		else
		{
			host = new WebAddressHost(uri.Host, actor);
			ctx.Save(host);
			return host;
		}
	}

	public static WebAddress GetOrCreateWebAddress(IDatabaseContext ctx, Uri uri, User actor)
	{
		var host = GetOrCreateWebAddressHost(ctx, uri, actor);
		var address = ctx.Query<WebAddress>().FirstOrDefault(address => address.Url == uri.ToString());
		if (address is not null)
		{
			return address;
		}
		else
		{
			address = new WebAddress(uri, host, actor);
			ctx.Save(address);
			return address;
		}
	}

	// TODO: remove
	[Obsolete]
	public static WebAddress GetOrCreateWebAddress(ISession ctx, Uri uri, User actor)
	{
		var host = GetOrCreateWebAddressHost(ctx, uri, actor);
		var address = ctx.Query<WebAddress>().FirstOrDefault(address => address.Url == uri.ToString());
		if (address is not null)
		{
			return address;
		}
		else
		{
			address = new WebAddress(uri, host, actor);
			ctx.Save(address);
			return address;
		}
	}

	public static CollectionDiffWithValue<T, T> Sync<T>(
		IDatabaseContext ctx,
		IList<T> oldLinks,
		IEnumerable<IWebLinkContract> newLinks,
		IWebLinkFactory<T> webLinkFactory,
		User actor
	)
		where T : WebLink
	{
		ParamIs.NotNull(() => oldLinks);
		ParamIs.NotNull(() => newLinks);

		var validLinks = newLinks.Where(w => !string.IsNullOrWhiteSpace(w.Url)).ToArray();

		T Create(IWebLinkContract newItem)
		{
			var address = GetOrCreateWebAddress(ctx, new Uri(newItem.Url), actor);
			address.IncrementReferenceCount();

			return webLinkFactory.CreateWebLink(
				newItem.Description,
				address,
				newItem.Category,
				newItem.Disabled
			);
		}

		bool Update(T oldItem, IWebLinkContract newItem)
		{
			if (!oldItem.ContentEquals(newItem))
			{
				oldItem.Category = newItem.Category;
				oldItem.Description = newItem.Description;
				oldItem.Disabled = newItem.Disabled;
				var address = GetOrCreateWebAddress(ctx, new Uri(newItem.Url), actor);
				oldItem.SetAddress(address);
				return true;
			}

			return false;
		}

		void Remove(T oldItem)
		{
			oldLinks.Remove(oldItem);
		}

		return CollectionHelper.SyncWithContent(
			oldLinks,
			validLinks,
			identityEquality: (n1, n2) => n1.Id == n2.Id,
			Create,
			Update,
			Remove
		);
	}

	public static CollectionDiff<T, T> SyncByValue<T>(
		IDatabaseContext ctx,
		IList<T> oldLinks,
		IEnumerable<ArchivedWebLinkContract> newLinks,
		IWebLinkFactory<T> webLinkFactory,
		User actor
	)
		where T : WebLink
	{
		T Create(ArchivedWebLinkContract newItem)
		{
			return webLinkFactory.CreateWebLink(
				newItem.Description,
				address: GetOrCreateWebAddress(ctx, new Uri(newItem.Url), actor),
				newItem.Category,
				newItem.Disabled
			);
		}

		void Remove(T oldItem)
		{
			oldItem.Address.DecrementReferenceCount();

			oldLinks.Remove(oldItem);
		}

		return CollectionHelper.Sync(
			oldLinks,
			newLinks,
			equality: (n1, n2) => n1.ContentEquals(n2),
			Create,
			Remove
		);
	}

	private string _description;
	private string _url;
	private WebAddress _address;

#nullable disable
	public WebLink() { }
#nullable enable

	public WebLink(string description, WebAddress address, WebLinkCategory category, bool disabled)
	{
		ParamIs.NotNull(() => description);

		Description = description;
		Address = address;
		Category = category;
		Disabled = disabled;
	}

	public WebLink(WebLinkContract contract)
	{
		ParamIs.NotNull(() => contract);

		Category = contract.Category;
		Description = contract.Description;
		Url = contract.Url;
		Disabled = contract.Disabled;
	}

	public virtual WebLinkCategory Category { get; set; }

	public virtual bool Disabled { get; set; }

	/// <summary>
	/// User-visible link description. Cannot be null.
	/// </summary>
	public virtual string Description
	{
		get => _description;
		[MemberNotNull(nameof(_description))]
		set
		{
			ParamIs.NotNull(() => value);
			_description = value;
		}
	}

	/// <summary>
	/// Link description if the description is not empty. Otherwise URL.
	/// </summary>
	public virtual string DescriptionOrUrl => !string.IsNullOrEmpty(Description) ? Description : Url;

	public virtual int Id { get; set; }

	/// <summary>
	/// Link URL. Cannot be null or empty.
	/// </summary>
	public virtual string Url
	{
		get => _url;
		[MemberNotNull(nameof(_url))]
		set
		{
			ParamIs.NotNullOrWhiteSpace(() => value);
			_url = value;
		}
	}

	public virtual WebAddress Address
	{
		get => _address;
		[MemberNotNull(nameof(_address), nameof(_url))]
		set
		{
			_address = value;
			Url = value.Url;
		}
	}

	public virtual bool ContentEquals(IWebLink? other)
	{
		if (other == null)
			return false;

		return Url == other.Url && Description == other.Description && Category == other.Category && Disabled == other.Disabled;
	}

	public override string ToString()
	{
		return $"web link '{Url}'";
	}

	public virtual void SetAddress(WebAddress address)
	{
		Address.DecrementReferenceCount();

		Address = address;

		Address.IncrementReferenceCount();
	}
}

public interface IWebLink
{
	WebLinkCategory Category { get; set; }

	string Description { get; set; }

	string Url { get; set; }

	bool Disabled { get; set; }
}

public interface IWebLinkWithDescriptionOrUrl : IWebLink
{
	string? DescriptionOrUrl { get; }
}
