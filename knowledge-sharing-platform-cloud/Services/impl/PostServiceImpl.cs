using knowledge_sharing_platform_cloud.Cache;
using knowledge_sharing_platform_cloud.Data.Models.Category;
using knowledge_sharing_platform_cloud.Data.Models.Channel;
using knowledge_sharing_platform_cloud.Data.Models.Post;
using knowledge_sharing_platform_cloud.Data.Repositories;
using knowledge_sharing_platform_cloud.Exception;
using knowledge_sharing_platform_cloud.Models.DTO;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req.PostReq;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.PostResp;
using System.Collections.Generic;
using System.Text.Json;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class PostServiceImpl : IPostService
    {
        private readonly PostRepo _postRepo;
        private readonly CategoryRepo _categoryRepo;
        private readonly ChannelRepo _channelRepo;
        private readonly UserCache _userCache;
        private readonly LikesRepo _likesRepo;
        private readonly PostImgUrlsCache _postImgUrlsCache;

        public PostServiceImpl(PostRepo postRepo, CategoryRepo categoryRepo, 
            ChannelRepo channelRepo,UserCache userCache, LikesRepo likesRepo, PostImgUrlsCache postImgUrlsCache)
        {
            _postRepo = postRepo;
            _categoryRepo = categoryRepo;
            _channelRepo = channelRepo; 
            _userCache = userCache;
            _likesRepo = likesRepo;
            _postImgUrlsCache = postImgUrlsCache;   
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
                PostImgUrl = postImageList != null ? JsonSerializer.Serialize(postImageList, new JsonSerializerOptions { WriteIndented = true }) : JsonSerializer.Serialize(new List<string>(), new JsonSerializerOptions { WriteIndented = true })
            };


            Channel postChannel = await _channelRepo.GetChannelbyIdAsync(postCategory.ChannelId);

            if (postChannel == null)
            {
                throw new BusinessException("Fail to create post. Channel does not exist.");
            }

            Post newPost = await _postRepo.CreatePostAsync(post);

            if (newPost == null)
            {
                throw new BusinessException("Failed to create a new post.");
            }

            await _channelRepo.IncreaseTotalPostByOne(postChannel.Id);

            CreatePostResp response = new()
            {
                PostId = newPost.Id,
            };

            return response;
        }

        public async Task<CursorBasedResp<IEnumerable<PostPageResp>>> PostPage(PostPageReq request, long uid)
        {
            /**
            * validation
            */
            if(request.ChannelCateogryId == null)
            {
                throw new BusinessException("Channel Category Id are required to query the page");
            }
            /**
             * cursor conversion
             */
            long? cursor = null;
            if (!request.IsFirstPage() && long.TryParse(request.Cursor, out var parsedCursor))
            {
                cursor = parsedCursor;
            }
            /**
             * get post page from db
             */
            var listData = await _postRepo.GetPostPage(cursor, request.PageSize, request.ChannelCateogryId);
            /**
             * get post user id
             */
            var userIds = listData.Select(post => post.UserId).Distinct().ToList();
            var postIds = listData.Select(post => post.Id).Distinct().ToList();
            /**
             * get post creator detail from cache
             */
            var userMap = await _userCache.GetBatch(userIds);
            /**
             * get user like status
             */
            var userPostsLikeStatusList = await _likesRepo.GetUserLikeStatus(uid,postIds);
            var userPostsLikeStatusDict = userPostsLikeStatusList
                .ToDictionary(like => like.PostId, like => like);
            var presignedImgUrlMap = await _postImgUrlsCache.GetBatch(postIds);
            /**
             * data conversion
             */
            var listResp = listData.Select(post =>
            {
                var postCreator = userMap.GetValueOrDefault(post.UserId, null);
                var likeStatus = userPostsLikeStatusDict.GetValueOrDefault(post.Id, null);
                var presignedImgUrlsArr = presignedImgUrlMap.GetValueOrDefault(post.Id, null);


                return new PostPageResp()
                {
                    PostCreatedTime = post.CreatedTime,
                    PostContent = post.Body,
                    PostTitle = post.Title,
                    LikeByCurrentUser = likeStatus != null ? likeStatus.LikeStatus : false,
                    PostCreatorId = postCreator != null ? postCreator.Id : null,
                    CreatorName = postCreator != null ? postCreator.Username : null,
                    CreatorProfileUrl = postCreator != null ? postCreator.ProfileUrl : null,
                    PostID = post.Id,
                    ImgUrls = presignedImgUrlsArr != null ? presignedImgUrlsArr.ImageUrl : null
                };
            }).ToList();
            long? cursorId = listData.Any() ? listData.Min(post => post.Id) : null;
            return CursorBasedResp<IEnumerable< PostPageResp >>.Init(new List<IEnumerable<PostPageResp>> { listResp }, cursorId, listResp.Count()<request.PageSize);

        }
    }
}
