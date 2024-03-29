using Microsoft.AspNetCore.Mvc.Infrastructure;
using Npgsql;

namespace api;

public class DatabaseService
{
   private NpgsqlDataSource _dataSource = NpgsqlDataSource.Create(DbConnection.Get());

   public async Task<string> addMessage(string message, string username, int roomId)
   {
       try
       {
           await using var connection = await _dataSource.OpenConnectionAsync();
           await using var command = _dataSource.CreateCommand($"INSERT INTO messages.messagetable (roomid, username, message) VALUES ('{roomId}','{username}','{message}')");
           await command.ExecuteScalarAsync();
           
         await connection.CloseAsync();
           return "det virker måske";
       }
       catch (Exception e)
       {
           Console.WriteLine(e);
           throw;       
       }

   }

   public async Task<List<string>> GetMessagesByRoomId(int roomId){
       try
       {
           List<string> messages = [];
           await using var connection = await _dataSource.OpenConnectionAsync();
           await using var command = _dataSource.CreateCommand($"SELECT message FROM messages.messagetable WHERE roomid={roomId}");
           await using var reader= await command.ExecuteReaderAsync();
           while (await reader.ReadAsync())
           {
              messages.Add(reader.GetString(0));
              break;
           }

           await connection.CloseAsync();
           return messages;
       }
       catch (Exception e)
       {
           Console.WriteLine(e);
           throw;
       }
   }
}