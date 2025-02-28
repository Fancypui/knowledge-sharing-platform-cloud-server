using System.Net.WebSockets;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;

namespace knowledge_sharing_platform_cloud.Services
{
    public interface IWebsocketService
    {
        public bool Connect(long id, WebSocket websocket);

        public bool Remove(long id, out WebSocket websocket);
        public void SendUid<T>(long uid,WSRespBase<T> wsRespBase);

        public void SendToGroup<T>(List<long> uidList, WSRespBase<T> wsRespBase);
    }
}
