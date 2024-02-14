using Npgsql;

namespace ws;

public class DatabaseService
{
   private NpgsqlDataSource _dataSource = NpgsqlDataSource.Create(DbConnection.Get());

   public async Task<string> addMessage(string message, string username, int roomid)
   {
       Console.WriteLine(DbConnection.Get());
       try
       {
            
           await using var connection = await _dataSource.OpenConnectionAsync();
           Console.WriteLine($"diller {username}");
           await using var command = _dataSource.CreateCommand($"INSERT INTO messages.messagetable (roomid, username, message) VALUES ('{roomid}','{username}','{message}')");
           Console.WriteLine("doller");
           await command.ExecuteScalarAsync();
           Console.WriteLine("duller");

         await connection.DisposeAsync();
           return "det virker måske";
       }
       catch (Exception e)
       {
           Console.WriteLine(e);
           throw;       
       }

   }
}