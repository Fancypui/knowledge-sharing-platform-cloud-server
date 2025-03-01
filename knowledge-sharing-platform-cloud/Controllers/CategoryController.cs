using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.CategoryReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.CategoryResp;
using knowledge_sharing_platform_cloud.Services;
using knowledge_sharing_platform_cloud.Services.impl;
using Microsoft.AspNetCore.Mvc;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [Route("/category")]
    [ApiController]
    public class CategoryController : Controller
    {
        ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        public async Task<ApiResult<CreateCategoryResp>> CreateCategory(CreateCategoryReq createCategoryReq)
        {
            CreateCategoryResp createCategoryResp = await _categoryService.CreateCategory(createCategoryReq);

            return ApiResult<CreateCategoryResp>.ServiceSucess(createCategoryResp);
        }

        [HttpPut("memberPrivilege")]
        public async Task<ApiResult<ModifyCategoryMemberPrivilegeResp>> ModifyCategoryMemberPrivilege(ModifyCategoryMemberPrivilegeReq modifyCategoryMemberPrivilegeReq)
        {
            ModifyCategoryMemberPrivilegeResp modifyCategoryMemberPrivilegeResp = await _categoryService.ModifyCategoryMemberPrivilege(modifyCategoryMemberPrivilegeReq);

            return ApiResult<ModifyCategoryMemberPrivilegeResp>.ServiceSucess(modifyCategoryMemberPrivilegeResp);
        }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<CategoryListResp>>> CategoryList([FromQuery] CategoryListReq categoryListReq)
        {
            IEnumerable<CategoryListResp> categoryListResp = await _categoryService.CategoryList(categoryListReq);

            return ApiResult<IEnumerable<CategoryListResp>>.ServiceSucess(categoryListResp);
        }
    }
}
