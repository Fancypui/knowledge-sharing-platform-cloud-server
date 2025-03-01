using knowledge_sharing_platform_cloud.Data.Models.Channel;
using knowledge_sharing_platform_cloud.Data.Models.Likes;
using knowledge_sharing_platform_cloud.Data.Models.Post;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class LikesRepo
    {
        private readonly LikesContext _likesContext;

        public LikesRepo(LikesContext context)
        {
            _likesContext = context;
        }

        public async Task<bool> ChangeLikeStatus(long userId, long postId, bool likeStatus)
        {
            return await _likesContext.Likes
                .Where(l => l.UserId == userId && l.PostId == postId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(l => l.LikeStatus, l => likeStatus)) > 0;
        }

        public async Task<List<Likes>> GetUserLikeStatus(long userId, List<long> postIds)
        {
            return await _likesContext.Likes
                .Where(like => like.UserId == userId && postIds.Contains(like.PostId)) 
                .ToListAsync();
        }


    }
}
