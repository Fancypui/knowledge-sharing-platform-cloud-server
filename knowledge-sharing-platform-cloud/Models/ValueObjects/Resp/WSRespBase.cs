namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp
{
    public class WSRespBase<T>
    {
        public int Type { get; set; }
        public T Data { get; set; }
    }
}
