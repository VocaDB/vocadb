#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.DataContracts.MikuDb;
using VocaDb.Model.Domain.MikuDb;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers;

using VocaDb.Web.Models.MikuDbAlbums;

public class MikuDbAlbumController : ControllerBase
{
	private readonly MikuDbAlbumService _service;

	private MikuDbAlbumService Service => _service;

	public MikuDbAlbumController(MikuDbAlbumService service)
	{
		_service = service;
	}

	public FileResult CoverPicture(int id)
	{
		var pictureData = Service.GetCoverPicture(id);

		if (pictureData == null)
			return File("~/Content/unknown.png", "image/png");

		return File(pictureData.Bytes, pictureData.Mime);
	}

	//
	// GET: /MikuDb/

	public ActionResult Index(string titleFilter, AlbumStatus? status)
	{
		var s = status ?? AlbumStatus.New;
		var albums = Service.GetAlbums(titleFilter, s, new PagingProperties(0, EntriesPerPage, false));
		var model = new Index(albums, titleFilter, s);

		PageProperties.Title = "Imported albums";

		return View(model);
	}

	[HttpGet]
	[Authorize]
	public ActionResult PrepareForImport(int id)
	{
		var result = Service.Inspect(new[] { new ImportedAlbumOptions(id) }).First();

		return View("PrepareForImport", result);
	}

	/*
	[HttpPost]
	[Authorize]
	public ActionResult PrepareForImport(IEnumerable<MikuDbAlbumContract> albums) {

		var selectedIds = (albums != null ? albums.Where(a => a.Selected).Select(a => new ImportedAlbumOptions(a.Id)).ToArray() : new ImportedAlbumOptions[] {});
		var result = Service.Inspect(selectedIds);

		return View("PrepareForImport", new PrepareAlbumsForImport(result));
	}*/

	[HttpPost]
	[Authorize]
	public ActionResult ImportFromFile()
	{
		if (Request.Form.Files.Count == 0 || Request.Form.Files[0].Length == 0)
			return RedirectToAction("Index");

		var file = Request.Form.Files[0];
		var id = Service.ImportFromFile(file.OpenReadStream());

		return RedirectToAction("PrepareForImport", new { id });
	}

	[HttpPost]
	[Authorize]
	public ActionResult ImportOne(string AlbumUrl)
	{
		var result = Service.ImportOne(AlbumUrl);

		if (result.AlbumContract != null)
		{
			TempData.SetSuccessMessage("Album was imported successfully and is ready to be processed.");
			return RedirectToAction("PrepareForImport", new { id = result.AlbumContract.Id });
		}
		else if (!string.IsNullOrEmpty(result.Message))
		{
			TempData.SetWarnMessage(result.Message);
		}

		return RedirectToAction("Index");
	}

	[Authorize]
	public ActionResult ImportNew()
	{
		var count = Service.ImportNew();

		if (count > 0)
		{
			TempData.SetSuccessMessage(count + " album(s) were downloaded successfully and are ready to be processed.");
		}
		else
		{
			TempData.SetWarnMessage("No new albums to download.");
		}

		return RedirectToAction("Index");
	}

	[HttpPost]
	[Authorize]
	public ActionResult AcceptImported(InspectedAlbum album, string commit)
	{
		if (commit != "Accept")
		{
			//var options = albums.Select(a => new ImportedAlbumOptions(a)).ToArray();
			var options = new ImportedAlbumOptions(album);
			var inspectResult = Service.Inspect(options);

			return View("PrepareForImport", inspectResult);
		}

		var ids = new ImportedAlbumOptions(album);
		var selectedSongIds = (album.Tracks != null ? album.Tracks.Where(t => t.Selected).Select(t => t.ExistingSong.Id).ToArray() : Array.Empty<int>());

		Service.AcceptImportedAlbum(ids, selectedSongIds);

		TempData.SetSuccessMessage("Imported album approved successfully.");

		return RedirectToAction("Index");
	}

	[Authorize]
	public ActionResult Delete(int id)
	{
		Service.Delete(id);

		TempData.SetSuccessMessage("Imported album deleted.");

		return RedirectToAction("Index");
	}

	[Authorize]
	public ActionResult SkipAlbum(int id)
	{
		Service.SkipAlbum(id);

		TempData.SetSuccessMessage("Imported album rejected.");

		return RedirectToAction("Index");
	}
}
