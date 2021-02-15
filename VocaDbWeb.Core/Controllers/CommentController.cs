#nullable disable

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Service;

namespace VocaDb.Web.Controllers
{
	public class CommentController : ControllerBase
	{
		private readonly OtherService _otherService;
		private readonly UserService _userService;

		public CommentController(OtherService otherService, UserService userService)
		{
			_otherService = otherService;
			_userService = userService;
		}

		//
		// GET: /Comment/

		public async Task<ActionResult> Index(int? userId = null)
		{
			if (userId.HasValue)
			{
				var user = _userService.GetUser(userId.Value);
				return View("CommentsByUser", user);
			}
			else
			{
				var comments = await _otherService.GetRecentComments();
				return View(comments);
			}
		}
	}
}
