using Fleck;

namespace api;

public class WsWithMetaData(IWebSocketConnection connection)
{
    public IWebSocketConnection Connection { get; set; } = connection;
    public string Username { get; set; }
    public bool IsSignedIn { get; set; }
}

public static class StateService
{
    public static Dictionary<Guid, WsWithMetaData> OpenConnections = new();

    public static Dictionary<int, HashSet<Guid>> RoomsWithConnections = new();

    public static bool AddConnection(IWebSocketConnection ws)
    {
        return OpenConnections.TryAdd(ws.ConnectionInfo.Id,
            new WsWithMetaData(ws));
    }

    public static bool AddToRoom(IWebSocketConnection ws, int room)
    {
        if (!RoomsWithConnections.ContainsKey(room))
            RoomsWithConnections.Add(room, new HashSet<Guid>());
        return RoomsWithConnections[room].Add(ws.ConnectionInfo.Id);
    }

    public static void BroadcastToRoom(int room, string message)
    {
        if (RoomsWithConnections.TryGetValue(room, out var guids))
            foreach (var guid in guids)
            {
                if (OpenConnections.TryGetValue(guid, out var ws))
                    ws.Connection.Send(message: message);
            }
    }
}