using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using knowledge_sharing_platform_cloud.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class PostRepo
    {
        private readonly ApplicationContext _applicationContext;

        public PostRepo(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            _applicationContext.Post.Add(post);
            await _applicationContext.SaveChangesAsync();

            return post;
        }

        public async Task<Post> GetPostById(long postId)
        {
            return await _applicationContext.Post
            .Where(p => p.Id == postId && !p.DeletedStatus)
            .FirstOrDefaultAsync();
        }

        public async Task<List<Post>> GetPostPage(long? cursor, int pageSize, long channelCategoryId)
        {
            if (cursor.HasValue && cursor > 0)
            {
                return await _applicationContext.Post
                .Where(p => p.CategoryId == channelCategoryId && !p.DeletedStatus)
                .Where(p => p.Id < cursor)
                .OrderByDescending(p => p.Id)
                .Take(pageSize) // Fetch only pageSize posts
                .ToListAsync();

            }
            else
            {
                return await _applicationContext.Post
                .Where(p => p.CategoryId == channelCategoryId && !p.DeletedStatus)
                .OrderByDescending(p => p.Id)
                .Take(pageSize) // Fetch only pageSize posts
                .ToListAsync();
            }

            
        }

        public async Task<List<Post>> GetPostImgUrlsByIds(List<long> ids)
        {
            return await _applicationContext.Post
                .Where(p => ids.Contains(p.Id) && !p.DeletedStatus)
                .ToListAsync();
        }
        public async Task<IDbContextTransaction> GetTransactionAsync()
        {
            return await _applicationContext.Database.BeginTransactionAsync();
        }
    }
}
