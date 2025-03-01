namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req.CategoryReq
{
    public class CreateCategoryReq
    {
        public long ChannelId { get; set; }

        public string CategoryName { get; set; }

        public bool MemberPrivilege { get; set; }
    }
}
