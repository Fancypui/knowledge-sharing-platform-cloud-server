using Amazon.S3;
using knowledge_sharing_platform_cloud.Data.Models.Category;
using knowledge_sharing_platform_cloud.Data.Models.Post;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.DTO;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.PostReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.PostResp;
using System.Text.Json;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class PostServiceImpl : IPostService
    {
        PostRepo _postRepo;
        CategoryRepo _categoryRepo;

        public PostServiceImpl(PostRepo postRepo, CategoryRepo categoryRepo)
        {
            _postRepo = postRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<CreatePostResp> CreatePost(CreatePostReq createPostReq)
        {
            Category postCategory = await _categoryRepo.GetCategoryById(createPostReq.CategoryId);

            if (postCategory == null)
            {
                throw new BusinessException("Failed to create post. Category does not exist in db.");
            }

            if (postCategory.MemberPrivilege == false)
            {
                throw new BusinessException("Failed to create post. Member does not have privilege to create post for this category.");
            }

            List<PostImageUrlDTO> postImageList = createPostReq.PostImageUrl;

            Post post = new()
            {
                Title = createPostReq.PostTitle,
                Body = createPostReq.PostBody,
                CategoryId = createPostReq.CategoryId,
                UserId = createPostReq.UserId,
                PostImgUrl = JsonSerializer.Serialize(postImageList, new JsonSerializerOptions { WriteIndented = true })
            };

            Post newPost = await _postRepo.CreatePostAsync(post);

            if (newPost == null)
            {
                throw new BusinessException("Failed to create a new post.");
            }

            CreatePostResp response = new()
            {
                PostId = newPost.Id,
            };

            return response;
        }
    }
}
