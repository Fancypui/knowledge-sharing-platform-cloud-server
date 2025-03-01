using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using knowledge_sharing_platform_cloud.Data.Models.Post;
using Microsoft.EntityFrameworkCore;

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
    }
}
