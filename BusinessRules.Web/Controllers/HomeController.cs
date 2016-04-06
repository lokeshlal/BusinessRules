using System.Web.Mvc;

namespace BusinessRules.Web.Controllers
{
    public partial class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Rules()
        {
            return View();
        }

        public ActionResult Constants()
        {
            return View();
        }

        public ActionResult Workflows()
        {
            return View();
        }
    }
}