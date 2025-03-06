using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using knowledge_sharing_platform_cloud.Data.Models.Post;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class PostRepo
    {
        PostContext _postContext;

        public PostRepo(PostContext postContext) 
        { 
            _postContext = postContext;
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            _postContext.Post.Add(post);
            await _postContext.SaveChangesAsync();

            return post;
        }

        public async Task<Post> GetPostById(long postId)
        {
            return await _postContext.Post
            .Where(p => p.Id == postId && !p.DeletedStatus)
            .FirstOrDefaultAsync();
        }

        public async Task<List<Post>> GetPostPage(long? cursor, int pageSize, long channelCategoryId)
        {
            if (cursor.HasValue && cursor > 0)
            {
                return await _postContext.Post
                .Where(p => p.CategoryId == channelCategoryId && !p.DeletedStatus)
                .Where(p => p.Id < cursor)
                .OrderByDescending(p => p.Id)
                .Take(pageSize) // Fetch only pageSize posts
                .ToListAsync();

            }
            else
            {
                return await _postContext.Post
                .Where(p => p.CategoryId == channelCategoryId && !p.DeletedStatus)
                .OrderByDescending(p => p.Id)
                .Take(pageSize) // Fetch only pageSize posts
                .ToListAsync();
            }

            
        }

        public async Task<List<Post>> GetPostImgUrlsByIds(List<long> ids)
        {
            return await _postContext.Post
                .Where(p => ids.Contains(p.Id) && !p.DeletedStatus)
                .ToListAsync();
        }
        public async Task<IDbContextTransaction> GetTransactionAsync()
        {
            return await _postContext.Database.BeginTransactionAsync();
        }
    }
}
