using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarksAppBackend
{
    internal interface IEventStore
    {
        void Store(DomainEventBase e);
    }

    internal class DataBaseEventStore : IEventStore
    {
        private MySqlConnection Connection;

        public DataBaseEventStore(string address, string database, string user, string password)
        {
            Connection = new MySqlConnection($"Server={address}; Database={database}; User Id={user}; Pwd={password}; SslMode=none;");
            Connection.Open();
        }

        public DataBaseEventStore(DBCredentials credentials)
            : this(credentials.Server, credentials.Database, credentials.User, credentials.Password) { }

        ~DataBaseEventStore()
        {
            Connection.Close();
        }

        public void Store(DomainEventBase e)
        {
            // LAST_INSERT_ID() is not thread safe
            MySqlCommand command = new MySqlCommand(
                @"INSERT INTO events(guid, occured, recorded, data)
                VALUES(?guid, ?occured, ?recorded, ?data); SELECT LAST_INSERT_ID();", Connection);
            command.Parameters.AddWithValue("guid", e.Guid);
            command.Parameters.AddWithValue("occured", e.Occured);
            command.Parameters.AddWithValue("recorded", e.Recorded);

            string jsonData = JsonConvert.SerializeObject(e, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });

            command.Parameters.AddWithValue("data", jsonData);

            e.SetNumber((ulong)command.ExecuteScalar());
        }
    }
}
