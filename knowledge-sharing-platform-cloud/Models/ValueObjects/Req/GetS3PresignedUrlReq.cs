namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req
{
    public class GetS3PresignedUrlReq
    {
        public string? FileType { get; set; }
        public string ObjectKey {  get; set; }
    }
}
