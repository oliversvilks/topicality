using Microsoft.AspNetCore.Mvc;

namespace Topicality.Web.Controllers;

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