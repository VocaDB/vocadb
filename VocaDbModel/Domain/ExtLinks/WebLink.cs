using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.ExtLinks
{

	public class WebLink : IWebLink, IEntryWithIntId
	{

		public static CollectionDiffWithValue<T, T> Sync<T>(IList<T> oldLinks, IEnumerable<WebLinkContract> newLinks, IWebLinkFactory<T> webLinkFactory)
			where T : WebLink
		{

			ParamIs.NotNull(() => oldLinks);
			ParamIs.NotNull(() => newLinks);

			var validLinks = newLinks.Where(w => !string.IsNullOrWhiteSpace(w.Url)).ToArray();
			var diff = CollectionHelper.Diff(oldLinks, validLinks, (n1, n2) => n1.Id == n2.Id);
			var created = new List<T>();
			var edited = new List<T>();

			foreach (var n in diff.Removed)
			{
				oldLinks.Remove(n);
			}

			foreach (var linkEntry in validLinks)
			{

				var entry = linkEntry;
				var old = (entry.Id != 0 ? oldLinks.FirstOrDefault(n => n.Id == entry.Id) : null);

				if (old != null)
				{

					if (!old.ContentEquals(linkEntry))
					{
						old.Category = linkEntry.Category;
						old.Description = linkEntry.Description;
						old.Url = linkEntry.Url;
						edited.Add(old);
					}

				}
				else
				{

					var n = webLinkFactory.CreateWebLink(linkEntry.Description, linkEntry.Url, linkEntry.Category);
					created.Add(n);

				}

			}

			return new CollectionDiffWithValue<T, T>(created, diff.Removed, diff.Unchanged, edited);

		}

		public static CollectionDiff<T, T> SyncByValue<T>(IList<T> oldLinks, IEnumerable<ArchivedWebLinkContract> newLinks, IWebLinkFactory<T> webLinkFactory)
			where T : WebLink
		{

			var diff = CollectionHelper.Diff(oldLinks, newLinks, (n1, n2) => n1.ContentEquals(n2));
			var created = new List<T>();

			foreach (var n in diff.Removed)
			{
				oldLinks.Remove(n);
			}

			foreach (var linkEntry in diff.Added)
			{

				var n = webLinkFactory.CreateWebLink(linkEntry.Description, linkEntry.Url, linkEntry.Category);
				created.Add(n);

			}

			return new CollectionDiff<T, T>(created, diff.Removed, diff.Unchanged);

		}

		private string description;
		private string url;

		public WebLink() { }

		public WebLink(string description, string url, WebLinkCategory category)
		{

			ParamIs.NotNull(() => description);
			ParamIs.NotNullOrWhiteSpace(() => url);

			Description = description;
			Url = url;
			Category = category;

		}

		public WebLink(WebLinkContract contract)
		{

			ParamIs.NotNull(() => contract);

			Category = contract.Category;
			Description = contract.Description;
			Url = contract.Url;

		}

		public virtual WebLinkCategory Category { get; set; }

		/// <summary>
		/// User-visible link description. Cannot be null.
		/// </summary>
		public virtual string Description
		{
			get { return description; }
			set
			{
				ParamIs.NotNull(() => value);
				description = value;
			}
		}

		/// <summary>
		/// Link description if the description is not empty. Otherwise URL.
		/// </summary>
		public virtual string DescriptionOrUrl
		{
			get
			{
				return !string.IsNullOrEmpty(Description) ? Description : Url;
			}
		}

		public virtual int Id { get; set; }

		/// <summary>
		/// Link URL. Cannot be null or empty.
		/// </summary>
		public virtual string Url
		{
			get { return url; }
			set
			{
				ParamIs.NotNullOrWhiteSpace(() => value);
				url = value;
			}
		}

		public virtual bool ContentEquals(IWebLink another)
		{

			if (another == null)
				return false;

			return (Url == another.Url && Description == another.Description && Category == another.Category);

		}

		public override string ToString()
		{
			return string.Format("web link '{0}'", Url);
		}

	}

	public interface IWebLink
	{

		WebLinkCategory Category { get; set; }

		string Description { get; set; }

		string Url { get; set; }

	}

}
