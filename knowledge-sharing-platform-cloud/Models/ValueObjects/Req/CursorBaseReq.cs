using System.ComponentModel.DataAnnotations;

namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req
{
    /**
     * pagination request
     */
    public class CursorBaseReq
    {
        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;

        public string? Cursor { get; set; }

        public bool IsFirstPage()
        {
            return string.IsNullOrEmpty(Cursor);
        }
    }
}
