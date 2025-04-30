using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Topicality.Client.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Topicality.Domain.Entities;
using Topicality.Domain.Interfaces;

namespace Topicality.Web.Controllers;
[ApiController]
[Route("api/[controller]")] 
public class UserDataController : ControllerBase
{
    private readonly ICategoryDocumentService _categoryDocumentService;
    private readonly UserManager<IdentityUser> _userManager; // Assuming you have an ApplicationUser class

    public UserDataController(ICategoryDocumentService categoryDocumentService, UserManager<IdentityUser> userManager)
    {
        _categoryDocumentService = categoryDocumentService;
        _userManager = userManager;
    }
    
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<UserCategory>>> GetUserCategories()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var categories = await _categoryDocumentService.GetCategoriesByUserEmailAsync(user.Email);
        return Ok(categories);
    }
    
    [HttpGet("documents")]
    public async Task<ActionResult<IEnumerable<DocumentMetadata>>> GetDocumentsByCategory(long categoryId)
    {
        var documents = await _categoryDocumentService.GetDocumentsByCategoryIdAsync(categoryId);
        return Ok(documents);
    }

}