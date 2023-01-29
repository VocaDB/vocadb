using System.Diagnostics.CodeAnalysis;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.ExtLinks;

public class WebLink : IWebLink, IEntryWithIntId
{
	public static CollectionDiffWithValue<T, T> Sync<T>(
		IList<T> oldLinks,
		IEnumerable<IWebLinkContract> newLinks,
		IWebLinkFactory<T> webLinkFactory
	)
		where T : WebLink
	{
		ParamIs.NotNull(() => oldLinks);
		ParamIs.NotNull(() => newLinks);

		var validLinks = newLinks.Where(w => !string.IsNullOrWhiteSpace(w.Url)).ToArray();

		T Create(IWebLinkContract newItem)
		{
			return webLinkFactory.CreateWebLink(
				newItem.Description,
				newItem.Url,
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
				oldItem.Url = newItem.Url;
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
		IList<T> oldLinks,
		IEnumerable<ArchivedWebLinkContract> newLinks,
		IWebLinkFactory<T> webLinkFactory
	)
		where T : WebLink
	{
		T Create(ArchivedWebLinkContract newItem)
		{
			return webLinkFactory.CreateWebLink(
				newItem.Description,
				newItem.Url,
				newItem.Category,
				newItem.Disabled
			);
		}

		void Remove(T oldItem)
		{
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

#nullable disable
	public WebLink() { }
#nullable enable

	public WebLink(string description, string url, WebLinkCategory category, bool disabled)
	{
		ParamIs.NotNull(() => description);
		ParamIs.NotNullOrWhiteSpace(() => url);

		Description = description;
		Url = url;
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

	// TODO: Make non-nullable.
	public virtual WebAddress? Address { get; set; }

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
