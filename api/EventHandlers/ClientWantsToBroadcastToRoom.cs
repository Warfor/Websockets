using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Fleck;
using lib;
using System.Net.Http;
using System.Net.Http.Headers;
using Npgsql;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace api;

public class ClientWantsToBroadcastToRoomDto : BaseDto
{
    public string message { get; set; }
    public int roomId { get; set; }
    public string username { get; set; }
}

public class CategoriesAnalysis
{
    public string category { get; set; }
    public int severity { get; set; }
}
public class ContentFilterResponse
{
    public List<object> blocklistsMatch { get; set; }
    public List<CategoriesAnalysis> categoriesAnalysis { get; set; }
}

public class ClientWantsToBroadcastToRoom : BaseEventHandler<ClientWantsToBroadcastToRoomDto>
{
    public override async Task<Task> Handle(ClientWantsToBroadcastToRoomDto dto, IWebSocketConnection socket)
    {
        await isMessageToxic(dto.message);
        if (!StateService.OpenConnections[socket.ConnectionInfo.Id].IsSignedIn)
        {
            throw new Exception("Unauthorized");
        }
        var message = new ServerBroadcastsMessageWithUsername()
        {
            message = dto.message,
            //username = dto.username,
            username = StateService.OpenConnections[socket.ConnectionInfo.Id].Username
        };
        StateService.BroadcastToRoom(dto.roomId, JsonSerializer.Serialize(message));
        var dbservice = new DatabaseService();
        dbservice.addMessage(message.message, message.username, dto.roomId);
        return Task.CompletedTask;
        
    }

    public record RequestModel(string text, List<string> categories, string outputType)
    {
        public override string ToString()
        {
            return $"{{ text = {text}, categories = {categories}, outputType = {outputType} }}";
        }
    }
    private async Task isMessageToxic(string message)
    {
        HttpClient client = new HttpClient();

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://toxicchat.cognitiveservices.azure.com/contentsafety/text:analyze?api-version=2023-10-01");

        request.Headers.Add("accept", "application/json");
        request.Headers.Add("Ocp-Apim-Subscription-Key", "32c943bd3b0a4b8c97e723645c7cea08");

        var req = new RequestModel(message, new List<string>() { "Hate", "Violence" }, "FourSeverityLevels");

        request.Content = new StringContent(JsonSerializer.Serialize(req));
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        HttpResponseMessage response = await client.SendAsync(request);
        Console.WriteLine(response);
        Console.WriteLine("spade");
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        var obj = JsonSerializer.Deserialize<ContentFilterResponse>(responseBody);
        var isToxic = obj.categoriesAnalysis.Count(e => e.severity > 1) >= 1;
        if (isToxic)
        {
            throw new ValidationException("Speak like that, you may not");
        }
    }
    
}

public class ServerBroadcastsMessageWithUsername : BaseDto
{
    public string message { get; set; }
    public string username { get; set; }
}