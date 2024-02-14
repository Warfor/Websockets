namespace api;
using System;

public class DbConnection
{
    public static string Get()
    {
        var uriString = "postgres://wxobotpu:4_ucMISjaKXQ7AtroS2KIn94nUv2-p4y@lucky.db.elephantsql.com/wxobotpu";
        var uri = new Uri(uriString);        
        var db = uri.AbsolutePath.Trim('/');         
        var user = uri.UserInfo.Split(':')[0];         
        var passwd = uri.UserInfo.Split(':')[1];
        var port = uri.Port > 0 ? uri.Port : 5432;
        var connStr = string.Format("Host={0};Username={2};Password={3};Database={1};", uri.Host, db, user,
            passwd, port);
        return connStr;
    }
}