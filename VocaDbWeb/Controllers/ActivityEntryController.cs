#nullable disable

using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Service;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers;

public class ActivityEntryController : ControllerBase
{
	private new const int EntriesPerPage = 50;

	private readonly ActivityFeedService _service;

	private ActivityFeedService Service => _service;

	public ActivityEntryController(ActivityFeedService service)
	{
		_service = service;
	}

	public ActionResult FollowedArtistActivity()
	{
		var result = Service.GetFollowedArtistActivity(EntriesPerPage);

		PageProperties.Title = "New activity by followed artists";

		return _reactIndex.File(PageProperties);
	}

	//
	// GET: /ActivityEntry/

	public ActionResult Index(DateTime? before)
	{
		ViewBag.Before = before;

		PageProperties.Title = Resources.Views.ActivityEntry.IndexStrings.RecentActivity;

		return _reactIndex.File(PageProperties);
	}
}

public class DetailedPageResult
{
	public DateTime? LastEntryDate { get; set; }

	public string ViewHtml { get; set; }
}
