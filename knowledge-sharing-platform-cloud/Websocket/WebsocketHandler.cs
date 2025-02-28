using System.Net.WebSockets;
using System.Text;
using knowledge_sharing_platform_cloud.Models.ValueObjects.Req;
using knowledge_sharing_platform_cloud.Services;
using Newtonsoft.Json;

namespace knowledge_sharing_platform_cloud.Websocket
{
    public class WebsocketHandler
    {
        private readonly IWebsocketService _websocketService;
        private readonly ILogger<WebsocketHandler> _logger; 
        public WebsocketHandler(IWebsocketService websocketService,ILogger<WebsocketHandler> logger)
        {
            _websocketService = websocketService;
            _logger = logger;
        }

        public async Task Handle(long id, WebSocket websocket)
        {
            _websocketService.Connect(id, websocket);
            var readBuffer = new byte[1024 * 4];
            /**
             * receive message from client, this method is blocked until receive a msg
             */
            var receiveMsg = await websocket.ReceiveAsync(new ArraySegment<byte>(readBuffer),CancellationToken.None);
            /**
             * listen to receive msg in a while loop
             */
            while (websocket.State == WebSocketState.Open)
            {
                try
                {
                    /**
                     * the buffer size is fixed,so need to get the actual msg size
                     */
                    string msg = Encoding.UTF8.GetString(readBuffer[..receiveMsg.Count]).TrimEnd('\0');
                    WSReqBase wsRequest = JsonConvert.DeserializeObject<WSReqBase>(msg);
                    _logger.LogInformation($"Client infomation type{wsRequest.Type} body {wsRequest.Data}");
                    /**
                     * keep listening the channel
                     */
                    receiveMsg = await websocket.ReceiveAsync(new ArraySegment<byte>(readBuffer), CancellationToken.None);
                }
                catch(System.Exception e)
                {
                    _logger.LogError(e, e.Message);
                    break;
                }
            }
            /**
             * if break from loop, remove channel from dictionary
             */
            _ = _websocketService.Remove(id, out _);
            /**
             * close channel
             */
            await websocket.CloseAsync(receiveMsg.CloseStatus.Value, receiveMsg.CloseStatusDescription, CancellationToken.None);
        }
    }
}
