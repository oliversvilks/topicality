using Microsoft.AspNetCore.Mvc;

namespace Topicality.Web.Controllers;

public class ComparisonController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}