namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp.CategoryResp
{
    public class CreateCategoryResp
    {
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool MemberPrivilege { get; set; }
    }
}
