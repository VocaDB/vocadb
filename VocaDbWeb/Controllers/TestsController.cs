using System.Web.Mvc;

namespace VocaDb.Web.Controllers
{
#if DEBUG
    public class TestsController : Controller
    {
        //
        // GET: /Tests/

        public ActionResult Index()
        {
            return View();
        }

    }
#endif
}
