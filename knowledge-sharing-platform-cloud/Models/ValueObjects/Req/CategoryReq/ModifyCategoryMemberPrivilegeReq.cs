using System.ComponentModel.DataAnnotations;

namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Req.CategoryReq
{
    public class ModifyCategoryMemberPrivilegeReq
    {
        [Required]
        public long CategoryId {  get; set; }
        [Required]
        public bool MemberPrivilege { get; set; }
    }
}
