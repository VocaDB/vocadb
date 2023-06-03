#nullable disable

using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Utils;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.SongLists;

namespace VocaDb.Web.Controllers;

public class SongListController : ControllerBase
{
	public const int SongsPerPage = 50;

	private readonly IEntryLinkFactory _entryLinkFactory;
	private readonly SongListQueries _queries;
	private readonly MarkdownParser _markdownParser;

	public SongListController(SongListQueries queries, IEntryLinkFactory entryLinkFactory, MarkdownParser markdownParser)
	{
		_queries = queries;
		_entryLinkFactory = entryLinkFactory;
		_markdownParser = markdownParser;
	}

	public ActionResult Details(int id = InvalidId)
	{
		if (id == InvalidId)
			return NoId();

		var contract = _queries.GetDetails(id);

		PageProperties.CanonicalUrl = VocaUriBuilder.CreateAbsolute(Url.Action("Details", new { id })).ToString();
		PageProperties.Description = contract.Description;

		if (contract.FeaturedCategory == SongListFeaturedCategory.Nothing)
		{
			PageProperties.PageTitle = $"{ViewRes.SongList.DetailsStrings.SongList} - {contract.Name}";
			PageProperties.Title = contract.Name;
			PageProperties.Subtitle = ViewRes.SongList.DetailsStrings.SongList;
		}
		else
		{
			var categoryName = Translate.SongListFeaturedCategoryNames[contract.FeaturedCategory];

			PageProperties.PageTitle = $"{categoryName} - {contract.Name}";
			PageProperties.Title = contract.Name;
			PageProperties.Subtitle = categoryName;
		}

		var viewModel = new SongListDetailsViewModel(contract, PermissionContext);

		viewModel.SmallThumbUrl = Url.ImageThumb(contract.MainPicture, ImageSize.SmallThumb);
		var thumbUrl = viewModel.ThumbUrl = Url.ImageThumb(contract.MainPicture, ImageSize.Original) ?? Url.ImageThumb(contract.MainPicture, ImageSize.Thumb);
		if (!string.IsNullOrEmpty(thumbUrl))
		{
			PageProperties.OpenGraph.Image = thumbUrl;
		}

		PageProperties.OpenGraph.ShowTwitterCard = true;

		var descriptionStripped = _markdownParser.GetPlainText(viewModel.SongList.Description);

		PageProperties.Description = descriptionStripped;
		PageProperties.Robots = viewModel.SongList.Deleted ? PagePropertiesData.Robots_Noindex_Follow : string.Empty;

		return ReactIndex.File(PageProperties);
	}

#nullable enable
	//
	// GET: /SongList/Edit/
	[Authorize]
	public ActionResult Edit(int? id)
	{
		return ReactIndex.File(PageProperties);
	}
#nullable disable

	public ActionResult Export(int id)
	{
		var songList = _queries.GetSongList(id);
		var formatString = "%notes%;%publishdate%;%title%;%url%;%pv.original.niconicodouga%;%pv.original.!niconicodouga%;%pv.reprint%";
		var tagString = _queries.GetTagString(id, formatString);

		var enc = new UTF8Encoding(true);
		var data = enc.GetPreamble().Concat(enc.GetBytes(tagString)).ToArray();

		return File(data, "text/csv", songList.Name + ".csv");
	}

	public ActionResult Featured(FeaturedViewModel viewModel)
	{
		PageProperties.CanonicalUrl = UrlMapper.FullAbsolute(Url.Action("Featured"));

		PageProperties.Title = ViewRes.SharedStrings.FeaturedSongLists;

		return ReactIndex.File(PageProperties);
	}

	[Authorize]
	public ActionResult Import()
	{
		PageProperties.Title = Resources.Views.SongList.ImportStrings.Title;

		return ReactIndex.File(PageProperties);
	}

	public ActionResult Versions(int id = InvalidId)
	{
		if (id == InvalidId)
			return NoId();

		var contract = _queries.GetSongListWithArchivedVersions(id);

		if (contract == null)
			return NotFound();

		PageProperties.Title = ViewRes.EntryDetailsStrings.Revisions + " - " + contract.Name;
		PageProperties.Robots = PagePropertiesData.Robots_Noindex_Nofollow;

		return ReactIndex.File(PageProperties);
	}
}
