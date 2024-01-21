using Microsoft.AspNetCore.Mvc;

namespace VocaDb.Web.Controllers;

public class RewindController : ControllerBase
{
	public ActionResult Index()
	{
		return ReactIndex.File(PageProperties);
	}
}