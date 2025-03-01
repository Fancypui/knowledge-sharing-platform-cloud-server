using knowledge_sharing_platform_cloud.Data.Models.Post;

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
    }
}
