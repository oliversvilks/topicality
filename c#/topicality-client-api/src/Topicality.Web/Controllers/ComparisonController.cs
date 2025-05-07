using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Topicality.Web.Controllers;

[Authorize]
public class ComparisonController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}