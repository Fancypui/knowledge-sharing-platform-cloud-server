using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using knowledge_sharing_platform_cloud.Enum;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Resp;

namespace knowledge_sharing_platform_cloud.Services.impl
{
    public class WebsocketServiceImpl : IWebsocketService
    {

        private ConcurrentDictionary<long, WebSocket> _channels = new ConcurrentDictionary<long, WebSocket>();

        public bool Connect(long id, WebSocket websocket)
        {
           return _channels.TryAdd(id, websocket);
        }

        public bool Remove(long id,out WebSocket websocket)
        {
            return _channels.TryRemove(id, out websocket);
        }

        public void SendToGroup<T>(List<long> uidList, WSRespBase<T> wsRespBase)
        {
            if (uidList == null || uidList.Count == 0)
            {
                return; 
            }
            /**
             * extract one by one, then send message to client
             */
            foreach (var uid in uidList)
            {
                SendUid(uid, wsRespBase);
            }
        }

        public async void SendUid<T>(long uid, WSRespBase<T> wsRespBase)
        {
            if (_channels.TryGetValue(uid, out var userChannel) && userChannel.State == WebSocketState.Open)
            {
                try
                {
                    // Convert the message to JSON format
                    string jsonMessage = JsonSerializer.Serialize(wsRespBase);
                    byte[] messageBytes = Encoding.UTF8.GetBytes(jsonMessage);
                    var buffer = new ArraySegment<byte>(messageBytes);

                    // Send the message asynchronously
                    await userChannel.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"Failed to send message to user {uid}: {ex.Message}");
                }
            }
        }
    }
}
