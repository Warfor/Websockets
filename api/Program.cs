using System.Reflection;
using System.Text.Json;
using Fleck;
using lib;
using Npgsql;
using api;

public static class Startup
{
    public static void Main(string[] args)
    {
        Statup(args);
        Console.ReadLine();
    }

    public static void Statup(string[]args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var clientEventHandlers = builder.FindAndInjectClientEventHandlers(Assembly.GetExecutingAssembly());

        var app = builder.Build();

        var server = new WebSocketServer("ws://0.0.0.0:8181");


        server.Start(ws =>
        {
            ws.OnOpen = () =>
            {
                StateService.AddConnection(ws);
            };
            ws.OnMessage = async message =>
            {
           
                try
                {
                    await app.InvokeClientEventHandler(clientEventHandlers, ws, message);
                }
                catch (Exception e)
                {
                    ws.Send(JsonSerializer.Serialize(new ServerSendsErrorMessageToClient()
                    {
                        
                        errorMessage = e.Message
                    }));
                    
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.InnerException);
                    Console.WriteLine(e.StackTrace);
                }
            };
        });
//var dbServer = new NpgsqlConnection(DbConnection.Get());
//dbServer.Open();
    }
}

public class ServerSendsErrorMessageToClient : BaseDto
{
    public string errorMessage { get; set; }
}

