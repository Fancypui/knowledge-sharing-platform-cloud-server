using knowledge_sharing_platform_cloud.Data.Models.Comment;
using Microsoft.EntityFrameworkCore;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class CommentRepo
    {
        private readonly CommentContext _commentContext;
        public CommentRepo(CommentContext commentContext)
        {
            _commentContext = commentContext;
        }

        public async Task<IEnumerable<Comment>> GetPaginatedComments(long postId, long rootId, long? cursor, int pageSize)
        {
            var query = _commentContext.Comment
            .Where(c => c.PostId == postId && c.RootId == rootId);

            // Determine sorting order based on rootId
            if (rootId != 0)
            {
                query = query.OrderBy(c => c.Id); // Ascending order if rootId != 0
            }
            else
            {
                query = query.OrderByDescending(c => c.Id); // Descending order if rootId == 0
            }

            // Apply cursor-based filtering
            if (cursor.HasValue)
            {
                query = rootId != 0
                    ? query.Where(c => c.Id > cursor.Value) // If rootId != 0, fetch records after the cursor
                    : query.Where(c => c.Id < cursor.Value); // If rootId == 0, fetch records before the cursor
            }

            return await query.Take(pageSize).ToListAsync();

        }
        public async Task<IEnumerable<Comment>> GetCommentByIds(List<long> ids)
        {
            return await _commentContext.Comment
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();
        }
      
    }
}
