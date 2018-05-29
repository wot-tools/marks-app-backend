using System;
using System.Collections.Generic;
using System.Text;

namespace MarksAppBackend
{
    internal static class Cache
    {
        public static PlayerCache PlayerCache = new PlayerCache();

        public static void SetProcessor(EventProcessor processor)
        {
            processor.EventStored += (sender, e) => PlayEvent(e.Event);
        }

        internal static void PlayEvent(DomainEventBase e)
        {
            switch (e)
            {
                case PlayerCreatedEvent playerCreated: ((ICache<Player>)PlayerCache).AddNew(Player.HandleEvent(playerCreated)); break;
                case MarkObtainedEvent markObtained: PlayerCache[markObtained.Guid].HandleEvent(markObtained); break;
            }
        }
    }

    internal interface ICache<T>
    {
        void AddNew(T item);
    }

    internal class PlayerCache : ICache<Player>
    {
        private Dictionary<Guid, Player> GuidCache = new Dictionary<Guid, Player>();
        private Dictionary<int, Player> IDCache = new Dictionary<int, Player>();

        public Player this[Guid guid] => GuidCache[guid];
        public Player this[int id] => IDCache[id];

        void ICache<Player>.AddNew(Player player)
        {
            GuidCache.Add(player.Guid, player);
            IDCache.Add(player.ID, player);
        }
    }
}
