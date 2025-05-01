using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Topicality.Client.Application.Services;
using Topicality.Domain.Entities;
using Topicality.Web.Models;

namespace Topicality.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICategoryService _categoryService;
    private readonly UserManager<IdentityUser> _userManager; 
    public HomeController(ILogger<HomeController> logger
    , ICategoryService categoryService,
    UserManager<IdentityUser> userManager
    )
    {
        _logger = logger;
        _categoryService = categoryService;
        _userManager = userManager;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var categories = await _categoryService.GetAllCategoriesAsync(User.Identity.Name);

        if (!categories.Where(x => x.Name == "notes").Any())
            await _categoryService.CreateCategoryAsync(new UserCategory()
            {

                Name = "notes",
                Description = "notes",
                UserEmail = User.Identity.Name
            });
        
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}