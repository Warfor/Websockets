using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Fleck;
using lib;

namespace ws;

public class BroadcastToclientsServerdto : BaseDto
{
    public string messageContent { get; set; }
}**

/// <summary>
/// Message all class?
public class BroadcastToclientsServer : BaseEventHandler<BroadcastToclientsServer>
{
    public override Task Handle(BroadcastToclientsServerDto dto, IWebSocketConnection socket)
}
/// </summary>
    var broadcast = new ServerBroadcast()
    {
        broadcastValue = "broadcasting:" + dto.messageContent
    };
    
    public class ServerBroadcast : BaseDto
    {
        public string broadcastValue { get; set; }
    }
