using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Topicality.Web.Controllers
{
    [Authorize]
    public class FlowsController : Controller
    {
        // GET: FlowsController
        public ActionResult Index()
        {
            return View();
        }

    }
}
