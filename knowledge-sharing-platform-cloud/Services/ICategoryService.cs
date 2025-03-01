using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.CategoryReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.CategoryResp;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface ICategoryService
    {
        Task<CreateCategoryResp> CreateCategory(CreateCategoryReq createCategoryReq);

        Task<ModifyCategoryMemberPrivilegeResp> ModifyCategoryMemberPrivilege(ModifyCategoryMemberPrivilegeReq modifyCategoryMemberPrivilegeReq);

        Task<IEnumerable<CategoryListResp>> CategoryList(CategoryListReq categoryListReq);
    }
}