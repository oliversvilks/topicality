using Microsoft.EntityFrameworkCore;
using Topicality.Client.Application.Services;
using Topicality.Domain.Entities;

namespace Topicality.Client.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserCategory>> GetAllCategoriesAsync(string userId)
    {
        return await _context.UserCategories.Where(x=>x.UserEmail == userId && !string.IsNullOrEmpty(x.Name)).Select(c => new UserCategory()
        {
            Id = c.Id,
            UserEmail = c.UserEmail,
            Name = c.Name,
            Uuid = c.Uuid
        }).ToListAsync();
    }

    public async Task<UserCategory?> GetCategoryByIdAsync(long id)
    {
        var category = await _context.UserCategories.FindAsync(id);
        return category == null ? null : new UserCategory()
        {
            Id = category.Id,
            UserEmail = category.UserEmail,
            Name = category.Name,
            Uuid = category.Uuid
        };
    }

    public async Task<UserCategory> CreateCategoryAsync(UserCategory categoryDto)
    {
        var category = new UserCategory()
        {
            UserEmail = categoryDto.UserEmail,
            Name = categoryDto.Name,
            Uuid = categoryDto.Uuid
        };

        _context.UserCategories.Add(category);
        await _context.SaveChangesAsync();

        categoryDto.Id = category.Id;
        return categoryDto;
    }

    public async Task<UserCategory?> UpdateCategoryAsync(long id, UserCategory categoryDto)
    {
        var category = await _context.UserCategories.FindAsync(id);
        if (category == null)
        {
            return null;
        }

        category.UserEmail = categoryDto.UserEmail;
        category.Name = categoryDto.Name;
        category.Uuid = categoryDto.Uuid;

        await _context.SaveChangesAsync();
        return categoryDto;
    }

    public async Task<bool> DeleteCategoryAsync(long id)
    {
        var category = await _context.UserCategories.FindAsync(id);
        if (category == null)
        {
            return false;
        }

        _context.UserCategories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<UserCategory>> GetCategoriesByIdsAsync(List<long> categoryIds, string userEmail)
    {
        return await _context.UserCategories
            .Where(c => categoryIds.Contains(c.Id) && c.UserEmail == userEmail)
            .ToListAsync();
    }

}
