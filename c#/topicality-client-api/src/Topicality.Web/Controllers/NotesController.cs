using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Topicality.Web.Controllers;

[Authorize]
public class NotesController : Controller
{
    
    public NotesController()
    {
 
    }
    // Action to render the view
    public IActionResult Index()
    {
        return View();
    }
}