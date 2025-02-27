namespace knowledge_sharing_platform_cloud.Models.ValueObjects.Resp
{
    /**
     * comment list response type
     */
    public class CommentListResp
    {
        public string Content {  get; set; }

        public long SenderUid { get; set; } 

        public long ReceiverUid { get; set; }

        public long CommentId { get; set; }

        public string SenderName { get; set; }

        public string ReceiverName { get; set; }

        public DateTime ReplyTime{ get; set; }

        public long PostId { get; set; }

        public long RootId { get; set; }

        public long ParentId { get; set; }
    }
}
