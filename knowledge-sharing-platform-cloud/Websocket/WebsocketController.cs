using Microsoft.AspNetCore.Mvc;

namespace knowledge_sharing_platform_cloud.Websocket
{
    [Route("websocket")]
    public class WebsocketController : ControllerBase
    {
        private readonly WebsocketHandler _websocketHandler;

        public WebsocketController(WebsocketHandler handler)
        {

            _websocketHandler = handler;
        }

        [HttpGet("{id}")]
        public async Task RegisterWebsocketChannel(long id)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var websocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _websocketHandler.Handle(id,websocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
