namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp
{
    public class CursorBasedResp<T>
    {
        public long? Cursor { get; set; }
        public Boolean IsLast { get; set; }
        public IEnumerable<T> List { get; set; }

        public static CursorBasedResp<T> empty()
        {
            return new CursorBasedResp<T>
            {
                IsLast = true,
                List = Enumerable.Empty<T>(),
                Cursor = null
            };
        }

        public static CursorBasedResp<T> Init(IEnumerable<T> list, long? cursor, Boolean isLast)
        {
            return new CursorBasedResp<T>
            {
                List = list,
                Cursor = cursor,
                IsLast = isLast
            };
        }
    }
}
