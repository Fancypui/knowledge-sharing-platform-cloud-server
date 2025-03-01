using knowledge_sharing_platform_cloud.Data.Models.Category;
using knowledge_sharing_platform_cloud.Data.Models.Channel;
using Microsoft.EntityFrameworkCore;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class CategoryRepo
    {
        CategoryContext _categoryContext;

        public CategoryRepo(CategoryContext categoryContext)
        {
            _categoryContext = categoryContext;
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _categoryContext.Category.Add(category);
            await _categoryContext.SaveChangesAsync();

            return category;
        }

        public async Task<bool> UpdateCategoryMemberPrivilege(long categoryId, bool newMemberPrivilege)
        {
            return await _categoryContext.Category
                .Where(c => c.Id == categoryId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.MemberPrivilege, c => newMemberPrivilege)) > 0;
        }

        public async Task<IEnumerable<Category>> GetCategoriesByChannelId(long channelId)
        {
            return await _categoryContext.Category
                .Where(c => c.ChannelId == channelId)
                .ToListAsync();
        }

        public async Task<Category> GetCategoryById(long id)
        {
            return await _categoryContext.Category.FindAsync(id);
        }
    }
}
