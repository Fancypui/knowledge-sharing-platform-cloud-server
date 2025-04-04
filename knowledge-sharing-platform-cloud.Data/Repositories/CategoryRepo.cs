using knowledge_sharing_platform_cloud.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class CategoryRepo
    {
        private readonly ApplicationContext _applicationContext;

        public CategoryRepo(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _applicationContext.Category.Add(category);
            await _applicationContext.SaveChangesAsync();

            return category;
        }

        public async Task<bool> UpdateCategoryMemberPrivilege(long categoryId, bool newMemberPrivilege)
        {
            return await _applicationContext.Category
                .Where(c => c.Id == categoryId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.MemberPrivilege, c => newMemberPrivilege)) > 0;
        }
        public async Task<Category?> GetByIdAsync(long categoryId)
        {
            return await _applicationContext.Category.FirstOrDefaultAsync(c => c.Id == categoryId);
        }


        public async Task<IEnumerable<Category>> GetCategoriesByChannelId(long channelId)
        {
            return await _applicationContext.Category
                .Where(c => c.ChannelId == channelId)
                .ToListAsync();
        }

        public async Task<Category> GetCategoryById(long id)
        {
            return await _applicationContext.Category.FindAsync(id);
        }
    }
}
