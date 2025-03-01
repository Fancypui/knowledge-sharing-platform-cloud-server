using knowledge_sharing_platform_cloud.Data.Models.Channel;
using knowledge_sharing_platform_cloud.Data.Models.Comment;
using knowledge_sharing_platform_cloud.Data.Models.Likes;
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


        public async Task<Likes> CreateLikesAsync(Likes like)
        {
            _likesContext.Likes.Add(like);
            await _likesContext.SaveChangesAsync();

            return like;
        }

        public async Task<Likes> FindLikesByUserIdAndPostIdAsync(long userId,  long postId)
        {
            return await _likesContext.Likes
                .FirstOrDefaultAsync(like => like.UserId == userId && like.PostId == postId);
        }

        public async Task<bool> ChangeLikeStatus(long likeId, bool likeStatus)
        {
            return await _likesContext.Likes
                .Where(l => l.Id == likeId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(l => l.LikeStatus, l => likeStatus)) > 0;
        }

        public async Task<IEnumerable<Likes>> GetPaginatedLikes(long postId, long? cursor, int pageSize)
        {
            var query = _likesContext.Likes
            .Where(l => l.PostId == postId && l.LikeStatus);

            return await query.Take(pageSize).ToListAsync();

        }
    }
}
