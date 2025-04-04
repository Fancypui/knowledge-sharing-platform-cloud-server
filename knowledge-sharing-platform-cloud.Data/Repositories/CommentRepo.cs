using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using knowledge_sharing_platform_cloud.Data.Models;

using Microsoft.EntityFrameworkCore;

namespace knowledge_sharing_platform_cloud.Data.Repositories
{
    public class CommentRepo
    {
        private readonly ApplicationContext _applicationContext;

        public CommentRepo(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public async Task<IEnumerable<Comment>> GetPaginatedComments(long postId, long rootId, long? cursor, int pageSize)
        {
            var query = _applicationContext.Comment
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
            return await _applicationContext.Comment
                .Where(c => ids.Contains(c.Id))
                .ToListAsync();
        }
        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            _applicationContext.Comment.Add(comment);
            await _applicationContext.SaveChangesAsync();

            return comment;
        }

    }
}
