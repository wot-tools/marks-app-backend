using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarksAppBackend
{
    internal interface IEventStore
    {
        void Store(DomainEventBase e);
        void Store(IEnumerable<DomainEventBase> events);
        IEnumerable<DomainEventBase> GetByID(int id);
        IEnumerable<DomainEventBase> GetByGuid(Guid guid);
        IEnumerable<DomainEventBase> GetByNumber(ulong number);
        IEnumerable<DomainEventBase> GetNumberRange(ulong start, ulong end);
        IEnumerable<DomainEventBase> GetAll();
    }

    internal class DataBaseEventStore : IEventStore
    {
        private MySqlConnection Connection;
        private readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects };


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

            string jsonData = JsonConvert.SerializeObject(e, JsonSettings);

            command.Parameters.AddWithValue("data", jsonData);

            e.SetNumber((ulong)command.ExecuteScalar());
        }

        public void Store(IEnumerable<DomainEventBase> events)
        {
            DomainEventBase[] data = events?.ToArray();
            if ((events?.Count() ?? 0) < 1)
                return;
            
            MySqlCommand command = new MySqlCommand(
                $@"INSERT INTO events(guid, occured, recorded, data)
                VALUES{String.Join(",", Enumerable.Range(0, data.Length).Select(i => $"(?guid{i}, ?occured{i}, ?recorded{i}, ?data{i})"))};", Connection);
            for (int i = 0; i < data.Length; i++)
            {
                command.Parameters.AddWithValue($"guid{i}", data[i].Guid);
                command.Parameters.AddWithValue($"occured{i}", data[i].Occured);
                command.Parameters.AddWithValue($"recorded{i}", data[i].Recorded);

                string jsonData = JsonConvert.SerializeObject(data[i], JsonSettings);
                command.Parameters.AddWithValue($"data{i}", jsonData);
            }

            command.ExecuteNonQuery();
        }

        public IEnumerable<DomainEventBase> GetByID(int id)
        {
            return GetGeneric("id = ?insert_id", c => c.Parameters.AddWithValue("insert_id", id));
        }

        public IEnumerable<DomainEventBase> GetByGuid(Guid guid)
        {
            return GetGeneric("guid = ?insert_guid", c => c.Parameters.AddWithValue("insert_guid", guid));
        }

        public IEnumerable<DomainEventBase> GetByNumber(ulong number)
        {
            return GetGeneric("number = ?insert_number", c => c.Parameters.AddWithValue("insert_number", number));
        }

        public IEnumerable<DomainEventBase> GetNumberRange(ulong start, ulong end)
        {
            return GetGeneric("number BETWEEN ?start AND ?end", c =>
            {
                c.Parameters.AddWithValue("start", start);
                c.Parameters.AddWithValue("end", end);
            });
        }

        public IEnumerable<DomainEventBase> GetAll()
        {
            return GetGeneric(null, null);
        }

        private IEnumerable<DomainEventBase> GetGeneric(string where, Action<MySqlCommand> addValues)
        {
            MySqlCommand command = new MySqlCommand($"SELECT number, data FROM events{(String.IsNullOrEmpty(where) ? String.Empty : $" WHERE {where}")};", Connection);
            addValues?.Invoke(command);
            using (var reader = command.ExecuteReader())
                while (reader.Read())
                {
                    var result = (DomainEventBase)JsonConvert.DeserializeObject((string)reader["data"], JsonSettings);
                    result.SetNumber((ulong)reader["number"]);
                    yield return result;
                }
        }
    }
}
