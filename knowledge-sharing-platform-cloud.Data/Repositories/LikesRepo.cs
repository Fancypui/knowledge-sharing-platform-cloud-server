using knowledge_sharing_platform_cloud.Data.Models;
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
        private readonly ApplicationContext _applicationContext;

        public LikesRepo(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }


        public async Task<Likes> CreateLikesAsync(Likes like)
        {
            _applicationContext.Likes.Add(like);
            await _applicationContext.SaveChangesAsync();

            return like;
        }

        public async Task<Likes?> FindLikesByUserIdAndPostIdAsync(long userId,  long postId)
        {
            return await _applicationContext.Likes
                .FirstOrDefaultAsync(like => like.UserId == userId && like.PostId == postId);
        }

        public async Task<bool> ChangeLikeStatus(long likeId, bool likeStatus)
        {
            return await _applicationContext.Likes
                .Where(l => l.Id == likeId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(l => l.LikeStatus, l => likeStatus)) > 0;
        }

        public async Task<List<Likes>> GetUserLikeStatus(long userId, List<long> postIds)
        {
            return await _applicationContext.Likes
                .Where(like => like.UserId == userId && postIds.Contains(like.PostId)) 
                .ToListAsync();
        }



        public async Task<IEnumerable<Likes>> GetPaginatedLikes(long postId, long? cursor, int pageSize)
        {
            if (cursor.HasValue && cursor > 0)
            {
                return await _applicationContext.Likes
                .Where(l => l.PostId == postId && l.LikeStatus)
                .Where(p => p.Id < cursor)
                .OrderByDescending(p => p.Id)
                .Take(pageSize) // Fetch only pageSize likes
                .ToListAsync();

            }
            else
            {
                return await _applicationContext.Likes
                .Where(l => l.PostId == postId && l.LikeStatus)
                .OrderByDescending(p => p.Id)
                .Take(pageSize) // Fetch only pageSize likes
                .ToListAsync();
            }
    

        }
    }
}
