using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Topicality.Client.Application.Services;
using Topicality.Domain.Entities;

namespace Topicality.Web.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
       private readonly ICategoryService _categoryService;
       

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
          
            var categories = await _categoryService.GetAllCategoriesAsync(User.Identity.Name);
            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(long id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCategory categoryDto)
        {

            categoryDto.UserEmail = User.Identity.Name;
            categoryDto.Uuid = Guid.NewGuid();
            await _categoryService.CreateCategoryAsync(categoryDto);
            return RedirectToAction(nameof(Index));

        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, UserCategory categoryDto)
        {
            if (ModelState.IsValid)
            {
                var result = await _categoryService.UpdateCategoryAsync(id, categoryDto);
                if (result == null)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(categoryDto);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
