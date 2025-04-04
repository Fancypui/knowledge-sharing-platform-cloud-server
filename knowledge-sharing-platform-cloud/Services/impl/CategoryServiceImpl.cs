using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Data.Models;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.CategoryReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.CategoryResp;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class CategoryServiceImpl : ICategoryService
    {
        private readonly CategoryRepo _categoryRepo;
        private readonly ChannelSummaryCache _channelSummaryCache;
        

        public CategoryServiceImpl(CategoryRepo categoryRepo, ChannelSummaryCache channelSummaryCache)
        {
            _channelSummaryCache = channelSummaryCache;
            _categoryRepo = categoryRepo;
        }

        public async Task<CreateCategoryResp> CreateCategory(CreateCategoryReq createCategoryReq,long uid)
        {

            var channelInfo = await _channelSummaryCache.Get(createCategoryReq.ChannelId);
            if (channelInfo == null || channelInfo.ChannelOwnerId != uid)
            {
                throw new BusinessException("User does not have permission to modify privilege/Channel does not exist");
            }
            if (string.IsNullOrEmpty(createCategoryReq.CategoryName) || createCategoryReq.CategoryName.Length < 3)
            {
                throw new BusinessException("Category Name cannot be less than 3 letters");
            }

            Category category = new()
            {
                CategoryName = createCategoryReq.CategoryName,
                MemberPrivilege = true,
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
                CategoryName= newCategory.CategoryName,
                MemberPrivilege = newCategory.MemberPrivilege
            };


            return response;
        }

        public async Task<ModifyCategoryMemberPrivilegeResp> ModifyCategoryMemberPrivilege(ModifyCategoryMemberPrivilegeReq modifyCategoryMemberPrivilegeReq, long uid)
        {
            

            var cateogry = await _categoryRepo.GetByIdAsync(modifyCategoryMemberPrivilegeReq.CategoryId);
            if (cateogry == null)
            {
                throw new BusinessException("Category not found");
            }
            var channelInfo = await _channelSummaryCache.Get(cateogry.ChannelId);
            if (channelInfo == null || channelInfo.ChannelOwnerId != uid)
            {
                throw new BusinessException("User does not have permission to modify privilege");
            }
            
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
