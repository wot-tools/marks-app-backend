using System;

namespace MarksAppBackend
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Settings.Instance is null)
            {
                Console.WriteLine("go and edit settings.json");
                return;
            }
            DataBaseEventStore eventStore = new DataBaseEventStore(Settings.Instance.Database);
            EventProcessor eventProcessor = new EventProcessor(eventStore);

            
        }

        /*
         * events:
         * 
         * player created
         * clan created (do we even need clans?)
         * mark obtained (this way we can even give an estimate as to when the mark was obtained, and we can create a graph that would show the marks for a tank/player over time)
         * 
         * 
         * stats regarding aggregates (mark count, clan mark count) could be tracked by just listening to events and adding numbers, no need to always query the db
         * 
         */
    }
}
