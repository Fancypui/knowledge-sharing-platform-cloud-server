using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.CategoryReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.CategoryResp;
using knowledge_sharing_platform_cloud.Services;
using knowledge_sharing_platform_cloud.Services.impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace knowledge_sharing_platform_cloud.Controllers
{
    [ApiController]
    [Route("/category")]
    public class CategoryController : Controller
    {
        ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResult<CreateCategoryResp>> CreateCategory([FromBody] CreateCategoryReq createCategoryReq)
        {
            CreateCategoryResp createCategoryResp = await _categoryService.CreateCategory(createCategoryReq, long.Parse(HttpContext.Items["UserId"]?.ToString()));

            return ApiResult<CreateCategoryResp>.ServiceSucess(createCategoryResp);
        }

        [HttpPut("memberPrivilege")]
        [Authorize]

        public async Task<ApiResult<ModifyCategoryMemberPrivilegeResp>> ModifyCategoryMemberPrivilege([FromBody] ModifyCategoryMemberPrivilegeReq modifyCategoryMemberPrivilegeReq)
        {
            

            ModifyCategoryMemberPrivilegeResp modifyCategoryMemberPrivilegeResp = await _categoryService.ModifyCategoryMemberPrivilege(modifyCategoryMemberPrivilegeReq, long.Parse(HttpContext.Items["UserId"]?.ToString()));

            return ApiResult<ModifyCategoryMemberPrivilegeResp>.ServiceSucess(modifyCategoryMemberPrivilegeResp);
        }

        [HttpGet]
        [Authorize]
        public async Task<ApiResult<IEnumerable<CategoryListResp>>> CategoryList([FromQuery] CategoryListReq categoryListReq)
        {

            IEnumerable<CategoryListResp> categoryListResp = await _categoryService.CategoryList(categoryListReq);

            return ApiResult<IEnumerable<CategoryListResp>>.ServiceSucess(categoryListResp);
        }
    }
}
