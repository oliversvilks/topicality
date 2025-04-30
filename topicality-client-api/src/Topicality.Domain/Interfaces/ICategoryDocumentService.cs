using Topicality.Domain.Entities;

namespace Topicality.Domain.Interfaces;

public interface ICategoryDocumentService
{
    Task<IEnumerable<CategoryDocument>> GetAllCategoryDocumentsAsync();
    Task<CategoryDocument?> GetCategoryDocumentByIdAsync(long id);
    Task<CategoryDocument> CreateCategoryDocumentAsync(CategoryDocument categoryDocument);
    Task<CategoryDocument?> UpdateCategoryDocumentAsync(long id, CategoryDocument categoryDocument);
    Task<bool> DeleteCategoryDocumentAsync(long id);

    Task<IEnumerable<CategoryDocument>> GetDocumentsByCategoryIdAsync(long categoryId);
    Task<IEnumerable<UserCategory>> GetCategoriesByUserEmailAsync(string userEmail);
}
