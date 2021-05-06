using Microsoft.AspNetCore.Mvc;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers
{
	public sealed record VocaDbPageProps
	{
		public VocaDbPageProps(VocaDbPage model, Login login)
		{
		}
	}

	public class ReactController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
