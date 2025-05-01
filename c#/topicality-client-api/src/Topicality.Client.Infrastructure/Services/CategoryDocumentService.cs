using Microsoft.EntityFrameworkCore;
using Topicality.Domain.Entities;
using Topicality.Domain.Interfaces;

namespace Topicality.Client.Infrastructure.Services;

public class CategoryDocumentService : ICategoryDocumentService
{
    private readonly ApplicationDbContext _context;

    public CategoryDocumentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryDocument>> GetAllCategoryDocumentsAsync()
    {
        return await _context.CategoryDocuments
            .Include(cd => cd.Category)
            .Include(cd => cd.DocumentMetadata)
            .ToListAsync();
    }

    public async Task<CategoryDocument?> GetCategoryDocumentByIdAsync(long id)
    {
        return await _context.CategoryDocuments
            .Include(cd => cd.Category)
            .Include(cd => cd.DocumentMetadata)
            .FirstOrDefaultAsync(cd => cd.Id == id);
    }

    public async Task<IEnumerable<UserCategory>> GetCategoriesByUserEmailAsync(string userEmail)
    {
        return await _context.UserCategories
            .Where(c => c.UserEmail == userEmail && !string.IsNullOrEmpty((c.Name)))
            .ToListAsync();
    }
    public async Task<CategoryDocument> CreateCategoryDocumentAsync(CategoryDocument categoryDocument)
    {
        _context.CategoryDocuments.Add(categoryDocument);
        await _context.SaveChangesAsync();
        return categoryDocument;
    }

    public async Task<CategoryDocument?> UpdateCategoryDocumentAsync(long id, CategoryDocument categoryDocument)
    {
        var existingDocument = await _context.CategoryDocuments.FindAsync(id);
        if (existingDocument == null)
        {
            return null;
        }

        existingDocument.Category = categoryDocument.Category;
        existingDocument.DocumentMetadata = categoryDocument.DocumentMetadata;
        existingDocument.Uuid = categoryDocument.Uuid;

        await _context.SaveChangesAsync();
        return categoryDocument;
    }

    public async Task<bool> DeleteCategoryDocumentAsync(long id)
    {
        var categoryDocument = await _context.CategoryDocuments.FindAsync(id);
        if (categoryDocument == null)
        {
            return false;
        }

        _context.CategoryDocuments.Remove(categoryDocument);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<IEnumerable<CategoryDocument>> GetDocumentsByCategoryIdAsync(long categoryId)
    {
        return await _context.CategoryDocuments
            .Include(cd => cd.DocumentMetadata)
            .Where(cd => cd.CategoryId == categoryId)
            .ToListAsync();
    }
}