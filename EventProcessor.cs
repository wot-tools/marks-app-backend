using System;
using System.Collections.Generic;
using System.Text;

namespace MarksAppBackend
{
    internal class EventStoredArgs : EventArgs
    {
        public DomainEventBase Event { get; private set; }

        public EventStoredArgs(DomainEventBase @event)
        {
            Event = @event;
        }
    }

    internal class EventProcessor
    {
        private IEventStore EventStore;

        public event EventHandler<EventStoredArgs> EventStored;

        public EventProcessor(IEventStore eventStore)
        {
            EventStore = eventStore;
        }

        public T Process<T>(T e) where T: DomainEventBase
        {
            EventStore.Store(e);
            EventStored?.Invoke(this, new EventStoredArgs(e));
            return e;
        }

        public void Replay(DomainEventBase e)
        {
            EventStored?.Invoke(this, new EventStoredArgs(e));
        }
    }
}
