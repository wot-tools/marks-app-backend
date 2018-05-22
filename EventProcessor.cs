using System;
using System.Collections.Generic;
using System.Text;

namespace MarksAppBackend
{
    internal class EventProcessor
    {
        private IEventStore EventStore;

        public EventProcessor(IEventStore eventStore)
        {
            EventStore = eventStore;
        }

        public void Process(DomainEventBase e)
        {
            EventStore.Store(e);
            //TODO: process event already when saving?
        }
    }
}
