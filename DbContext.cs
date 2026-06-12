using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp01
{
    public static class DbContext
    {
        private static string connectionString = "Server=localhost;Port=3306;Database=QLSinhVienCSharp;User ID=root;Password=1234;AllowPublicKeyRetrieval=True;SSL Mode=None;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}