using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Fleck;
using lib;
using Npgsql;
namespace ws;

public class ClientWantsToBroadcastToRoomDto : BaseDto
{
    public string message { get; set; }
    public int roomId { get; set; }
    
    public string username { get; set; }
}

public class ClientWantsToBroadcastToRoom : BaseEventHandler<ClientWantsToBroadcastToRoomDto>

{
    public override Task Handle(ClientWantsToBroadcastToRoomDto dto, IWebSocketConnection socket)
    {
        var message = new ServerBroadcastsMessageWithUsername()
        {
            message = dto.message,
            username = dto.username,
            //username = StateService.OpenConnections[socket.ConnectionInfo.Id].Username
        };
        Console.WriteLine(message.message);
        StateService.BroadcastToRoom(dto.roomId, JsonSerializer.Serialize(message));
        var dbservice = new DatabaseService();
        dbservice.addMessage(message.message, message.username, dto.roomId);
        return Task.CompletedTask;
    }
}

public class ServerBroadcastsMessageWithUsername : BaseDto
{
    public string message { get; set; }
    public string username { get; set; }
}