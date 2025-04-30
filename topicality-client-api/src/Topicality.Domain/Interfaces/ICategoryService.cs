using Topicality.Domain.Entities;

namespace Topicality.Client.Application.Services;

public interface ICategoryService
{
    Task<IEnumerable<UserCategory>> GetAllCategoriesAsync(string userId);
    Task<UserCategory?> GetCategoryByIdAsync(long id);
    Task<UserCategory> CreateCategoryAsync(UserCategory category);
    Task<UserCategory?> UpdateCategoryAsync(long id, UserCategory category);
    Task<bool> DeleteCategoryAsync(long id);
    
    Task<List<UserCategory>> GetCategoriesByIdsAsync(List<long> categoryIds, string userEmail);

}