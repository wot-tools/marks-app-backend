using System;
using System.Collections.Generic;
using System.Text;

namespace MarksAppBackend
{
    internal static class Cache
    {
        public static PlayerCache PlayerCache;

        internal static void DumpNewObject(object obj)
        {
            switch (obj)
            {
                case Player player: PlayerCache.AddNewPlayer(player); break;
            }
        }
    }

    internal class PlayerCache
    {
        private Dictionary<Guid, Player> GuidCache = new Dictionary<Guid, Player>();
        private Dictionary<int, Player> IDCache = new Dictionary<int, Player>();

        internal PlayerCache(EventProcessor processor, IEventStore store)
        {
            processor.EventStored += HandleEvents;
        }

        internal void AddNewPlayer(Player player)
        {
            GuidCache.Add(player.Guid, player);
            IDCache.Add(player.ID, player);
        }

        private void HandleEvents(object sender, EventStoredArgs e)
        {
            switch (e)
            {
                // dont handle creation here
            }
        }
    }
}
