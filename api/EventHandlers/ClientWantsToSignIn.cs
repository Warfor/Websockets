using System.Text.Json;
using Fleck;
using lib;

namespace api;

public class ClientWantsToSignInDto : BaseDto
{
    public string Username { get; set; }
}

public class ClientWantsToSignIn : BaseEventHandler<ClientWantsToSignInDto>
{
    public override Task Handle(ClientWantsToSignInDto dto, IWebSocketConnection socket)
    {
        StateService.OpenConnections[socket.ConnectionInfo.Id].Username = dto.Username;
        StateService.OpenConnections[socket.ConnectionInfo.Id].IsSignedIn = true;
        socket.Send(JsonSerializer.Serialize(new ServerWelcomesUser()));
        Console.WriteLine("vi er signed in");
        return Task.CompletedTask;
    }
}

public class ServerWelcomesUser : BaseDto;
