using knowledge_sharing_platform_cloud.Data.Models.Category;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.CategoryReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.CategoryResp;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class CategoryServiceImpl : ICategoryService
    {
        private readonly CategoryRepo _categoryRepo;

        public CategoryServiceImpl(CategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        public async Task<CreateCategoryResp> CreateCategory(CreateCategoryReq createCategoryReq)
        {
            Category category = new()
            {
                CategoryName = createCategoryReq.CategoryName,
                MemberPrivilege = createCategoryReq.MemberPrivilege,
                ChannelId = createCategoryReq.ChannelId,
            };

            Category newCategory = await _categoryRepo.CreateCategoryAsync(category);

            if (newCategory == null)
            {
                throw new BusinessException("Failed to create new category");
            }

            CreateCategoryResp response = new()
            {
                CategoryId = newCategory.Id,
            };


            return response;
        }

        public async Task<ModifyCategoryMemberPrivilegeResp> ModifyCategoryMemberPrivilege(ModifyCategoryMemberPrivilegeReq modifyCategoryMemberPrivilegeReq)
        {
            bool updatedCategory = await _categoryRepo.UpdateCategoryMemberPrivilege(modifyCategoryMemberPrivilegeReq.CategoryId, modifyCategoryMemberPrivilegeReq.MemberPrivilege);

            if (!updatedCategory)
            {
                throw new BusinessException("Failed to update category in db.");
            }

            ModifyCategoryMemberPrivilegeResp response = new()
            {
                CategoryId = modifyCategoryMemberPrivilegeReq.CategoryId
            };

            return response;
        }

        public async Task<IEnumerable<CategoryListResp>> CategoryList(CategoryListReq categoryListReq)
        {
            IEnumerable<Category> categoryListbyChannelId = await _categoryRepo.GetCategoriesByChannelId(categoryListReq.ChannelId);

            IEnumerable<CategoryListResp> response = categoryListbyChannelId.Select(channelCategory =>
            {
                return new CategoryListResp
                {
                    CategoryId = channelCategory.Id,
                    CategoryName = channelCategory.CategoryName,
                    MemberPrivilege = channelCategory.MemberPrivilege,
                };
            });

            return response;
        }
    }
}
