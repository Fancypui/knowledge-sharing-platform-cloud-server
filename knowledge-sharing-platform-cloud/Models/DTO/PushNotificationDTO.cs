using System.Text.Json;
using knowledge_sharing_platform_cloud.Enum;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;

namespace knowledge_sharing_platform_cloud.Models.DTO
{
    public class PushNotificationDTO
    {
        /**
         * serialize the WSBaseResp to string json 
         * because the AWS.Messaging sdk does not allow to handle generic type in their package
         */
        public string RespJson { get; set; }

        public List<long> UserIdList { get; set; }

        /**
         * 1 Send to individual user
         * 2 send to all user
         */
        public PushNotificationType Type { get; set; }

        public WSRespBase<object> GetResp()
        {
            return JsonSerializer.Deserialize<WSRespBase<object>>(RespJson);
        }
        public void SetResp<T>(WSRespBase<T> response)
        {
            RespJson = JsonSerializer.Serialize(response);
        }

    }
}
