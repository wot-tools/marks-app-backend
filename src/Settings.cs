using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MarksAppBackend
{
    internal class Settings
    {
        private const string FILE_NAME = "settings.json";
        private static object Lock = new object();
        private static Settings _Instance;
        public static Settings Instance
        {
            get
            {
                if (_Instance is null)
                    lock(Lock)
                        if (_Instance is null)
                            _Instance = Load();
                return _Instance;
            }
        }

        public DBCredentials Database { get; private set; }
        public int ParallelApiRequestCount { get; private set; }

        public Settings(DBCredentials database, int parallelApiRequestCount)
        {
            Database = database;
            ParallelApiRequestCount = parallelApiRequestCount;
        }

        private static Settings Load()
        {
            try
            {
                using (var reader = File.OpenText(Path.GetFullPath(FILE_NAME)))
                    return JsonConvert.DeserializeObject<Settings>(reader.ReadToEnd());
            }
            catch (FileNotFoundException)
            {
                File.WriteAllText(Path.GetFullPath(FILE_NAME), JsonConvert.SerializeObject(CreateDefault()));
                return null;
            }
        }

        private static Settings CreateDefault()
        {
            return new Settings(new DBCredentials("", "", "", ""), 10);
        }
    }

    internal class DBCredentials
    {
        public string User { get; private set; }
        public string Password { get; private set; }
        public string Server { get; private set; }
        public string Database { get; private set; }

        public DBCredentials(string user, string password, string server, string database)
        {
            User = user;
            Password = password;
            Server = server;
            Database = database;
        }
    }
}
