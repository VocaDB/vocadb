#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using ViewRes.Tag;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Translations;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Search;

namespace VocaDb.Web.Controllers;

public class TagController : ControllerBase
{
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
	private readonly IEnumTranslations _enumTranslations;
	private readonly IEntryLinkFactory _entryLinkFactory;
	private readonly MarkdownParser _markdownParser;
	private readonly TagQueries _queries;
	private readonly IAggregatedEntryImageUrlFactory _entryThumbPersister;

	public TagController(
		TagQueries queries,
		IEntryLinkFactory entryLinkFactory,
		IEnumTranslations enumTranslations,
		MarkdownParser markdownParser,
		IAggregatedEntryImageUrlFactory entryThumbPersister
	)
	{
		_queries = queries;
		_entryLinkFactory = entryLinkFactory;
		_enumTranslations = enumTranslations;
		_markdownParser = markdownParser;
		_entryThumbPersister = entryThumbPersister;
	}

	public ActionResult ArchivedVersionXml(int id)
	{
		var doc = _queries.GetVersionXml<ArchivedTagVersion>(id);
		var content = doc != null ? XmlHelper.SerializeToUTF8XmlString(doc) : string.Empty;

		return Xml(content);
	}

	public ActionResult Restore(int id)
	{
		_queries.Restore(id);

		return RedirectToAction("DetailsById", new { id });
	}

	private ActionResult RenderDetails(TagDetailsForApiContract contract)
	{
		if (contract == null)
			return NotFound();

		PageProperties.GlobalSearchType = EntryType.Tag;
		PageProperties.PageTitle = contract.Name;
		PageProperties.Title = contract.Name;
		PageProperties.Subtitle = DetailsStrings.Tag;
		PageProperties.CanonicalUrl = _entryLinkFactory.GetFullEntryUrl(EntryType.Tag, contract.Id, contract.UrlSlug);
		PageProperties.OpenGraph.ShowTwitterCard = true;
		PageProperties.Robots = contract.Deleted ? PagePropertiesData.Robots_Noindex_Follow : string.Empty;

		return ReactIndex.File(PageProperties);
	}

	// Kept for now since there's external references.
	[Obsolete]
	public ActionResult Details(string id)
	{
		if (string.IsNullOrEmpty(id))
			return NoId();

		var tagId = _queries.GetTagByName(id, t => t.Id, InvalidId);

		if (tagId == InvalidId)
		{
			s_log.Info($"Tag not found: {id.Replace(Environment.NewLine, "")}, referrer {Request.GetTypedHeaders().Referer}");
			return NotFound();
		}

		return RedirectToActionPermanent("DetailsById", new { id = tagId, slug = id });
	}

	/// <summary>
	/// Redirects to entry type tag based on entry type and possible sub-type.
	/// As fallback, redirects to tags index if no tag is found.
	/// </summary>
	public ActionResult DetailsByEntryType(EntryType entryType, string subType = "")
	{
		var tag = _queries.FindTagForEntryType(new EntryTypeAndSubType(entryType, subType), (tag, lang) => new TagBaseContract(tag, lang));

		if (tag != null)
		{
			return RedirectToAction("DetailsById", new { id = tag.Id, slug = tag.UrlSlug });
		}
		else
		{
			return RedirectToAction("Index");
		}
	}

	public async Task<ActionResult> DetailsById(int id = InvalidId, string slug = null)
	{
		if (id == InvalidId)
			return NoId();

		// TODO: write test for null slug
		slug ??= string.Empty;

		var tagName = await _queries.LoadTagAsync(id, t => t.UrlSlug ?? string.Empty);

		if (slug != tagName)
		{
			return RedirectToActionPermanent("DetailsById", new { id, slug = tagName });
		}

		var contract = await _queries.GetDetailsAsync(id);

		var prop = PageProperties;

		var thumbUrl = Url.ImageThumb(contract.MainPicture, ImageSize.Original);
		if (!string.IsNullOrEmpty(thumbUrl))
		{
			PageProperties.OpenGraph.Image = thumbUrl;
		}

		prop.Description = _markdownParser.GetPlainText(contract.Description.EnglishOrOriginal);

		return RenderDetails(contract);
	}


#nullable enable
	[Authorize]
	public ActionResult Edit(int id)
	{
		return ReactIndex.File(PageProperties);
	}
#nullable disable

	public ActionResult Index(string filter = null)
	{
		if (!string.IsNullOrEmpty(filter))
		{
			var tag = _queries.GetTagByName(filter, t => new { t.Id, t.UrlSlug });

			if (tag != null)
			{
				return RedirectToAction("DetailsById", new { id = tag.Id, slug = tag.UrlSlug });
			}

			return RedirectToAction("Index", "Search", new SearchIndexViewModel(EntryType.Tag, filter));
		}

		var tags = _queries.GetTagsByCategories();

		PageProperties.Title = ViewRes.SharedStrings.Tags;

		return ReactIndex.File(PageProperties);
	}

	public ActionResult Merge()
	{
		return ReactIndex.File(PageProperties);
	}

	public ActionResult Versions(int id = InvalidId)
	{
		if (id == InvalidId)
			return NoId();

		var contract = _queries.GetTagWithArchivedVersions(id);

		PageProperties.Title = ViewRes.EntryDetailsStrings.Revisions + " - " + contract.Name;
		PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

		return ReactIndex.File(PageProperties);
	}

	public ActionResult ViewVersion(int id, int? ComparedVersionId)
	{
		var contract = _queries.GetVersionDetails(id, ComparedVersionId ?? 0);

		PageProperties.Title = "Revision " + contract.ArchivedVersion.Version + " for " + contract.Name;
		PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

		return ReactIndex.File(PageProperties);
	}

	[Authorize]
	public ActionResult Deleted()
	{
		PageProperties.Title = "Deleted Tags";

		return ReactIndex.File(PageProperties);
	}
}
