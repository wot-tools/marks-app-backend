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
            MySqlCommand command = new MySqlCommand("INSERT INTO events VALUES(?number, ?guid, ?occured, ?recorded, ?data);", Connection);
            command.Parameters.AddWithValue("number", e.Number);
            command.Parameters.AddWithValue("guid", e.Guid);
            command.Parameters.AddWithValue("occured", e.Occured);
            command.Parameters.AddWithValue("recorded", e.Recorded);

            string jsonData = JsonConvert.SerializeObject(e, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });

            command.Parameters.AddWithValue("data", jsonData);

            command.ExecuteNonQuery();
        }
    }
}
