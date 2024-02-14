using System.Text.Json;
using Fleck;
using lib;

namespace ws;

public class ClientWantsToEnterRoomDto : BaseDto
{
    public int roomId { get; set; }
}

public class ClientWantsToEnterRoom : BaseEventHandler<ClientWantsToEnterRoomDto>
{
    public override Task Handle(ClientWantsToEnterRoomDto dto, IWebSocketConnection socket)
    {
        StateService.AddToRoom(socket, dto.roomId);
        var dbservice = new DatabaseService();
        List<string> messages = dbservice.GetMessagesByRoomId(dto.roomId).Result;
        string result = string.Join(", ", messages);
        socket.Send(JsonSerializer.Serialize(new ServerAddsClientToRoom()
        {
            message = $"you were succesfully added to room with ID {dto.roomId}, recent messages: {result}"
        }));
        return Task.CompletedTask;
    }
}

public class ServerAddsClientToRoom : BaseDto
{
    public string message { get; set; }
}