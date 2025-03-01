using System.Runtime.ConstrainedExecution;

namespace knowledge_sharing_platform_cloud.Constant
{
    public class RedisConstant
    {
        public const long KEY_EXPIRY_DURATION = 120L;

        /**
         * %d means id here
         */
        //public const string COMMENT_LIST_KEY = "comment_index:post_%d:root_%d";

        /**
         * individual comment info
         */
        public const string COMMENT_DETAIL = "comment_index:commentId_{0}";

        public const string USER_INFO = "user_info:userid_{0}";

        public const string CHANNEL_SUMMARY = "channel_summary:channelId_{0}";

        /**
         * channel leaderboard list
         */
        public const string CHANNEL_LEADERBOARD = "channel_leaderboard";
        /**
         * Post image presigned urls
         */
        public const string POST_IMAGE_PRESIGNED_URLS = "post_image_presigned_urls:post_{0}";


        /**
         * format key
         */
        public static string GetKey(string key, params object[] args)
        {
            return string.Format(key, args);
        }
    }
}
