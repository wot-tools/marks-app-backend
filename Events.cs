using System;
using System.Collections.Generic;
using System.Text;

namespace MarksAppBackend
{
    internal abstract class DomainEventBase
    {
        public Guid Guid { get; private set; }
        public ulong Number { get; private set; }
        public DateTime Occured { get; private set; }
        public DateTime Recorded { get; private set; }

        internal DomainEventBase(Guid guid, ulong number, DateTime occured, DateTime recorded)
        {
            if (guid != default(Guid))
                Guid = guid;
            Number = number;
            Occured = occured;
            if (recorded != default(DateTime))
                Recorded = recorded;
            else
                Recorded = DateTime.Now;
        }

        internal void SetNumber(ulong number)
        {
            if (Number == 0)
                Number = number;
        }
    }

    internal abstract class DomainObjectChangedEvent : DomainEventBase
    {
        public DomainObjectChangedEvent(Guid guid, ulong number, DateTime occured, DateTime recorded)
            : base(guid, number, occured, recorded) { }

        internal abstract void Process();
    }

    internal abstract class DomainObjectCreatedEvent<T> : DomainEventBase
    {
        public DomainObjectCreatedEvent(Guid guid, ulong number, DateTime occured, DateTime recorded)
            : base(guid, number, occured, recorded) { }

        internal abstract T Process();
    }

    internal class PlayerCreatedEvent : DomainObjectCreatedEvent<Player>
    {
        public int ID { get; private set; }

        public PlayerCreatedEvent(int id, DateTime occured)
            : this(id, Guid.NewGuid(), 0, occured, default(DateTime)) { }

        internal PlayerCreatedEvent(int id, Guid guid, ulong number, DateTime occured, DateTime recorded)
            : base(guid, number, occured, recorded)
        {
            ID = id;
        }

        internal override Player Process()
        {
            return new Player();
        }
    }

    internal class MarkObtainedEvent : DomainObjectChangedEvent
    {
        public int Level { get; private set; } // 1, 2, 3, maybe use enum?
        public int TankID { get; private set; }

        public MarkObtainedEvent(Player player, int tankID, int level, DateTime occured)
            : this(tankID, level, player.Guid, 0, occured, default(DateTime)) { }

        internal MarkObtainedEvent(int tankID, int level, Guid guid, ulong number, DateTime occured, DateTime recorded)
            : base(guid, number, occured, recorded)
        {
            TankID = tankID;
            Level = level;
        }

        internal override void Process()
        {

        }
    }
}
