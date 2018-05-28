using System;
using System.Linq;

namespace MarksAppBackend
{
    class Program
    {
        private static Random Random = new Random();

        static void Main(string[] args)
        {
            if (Settings.Instance is null)
            {
                Console.WriteLine("go and edit settings.json");
                return;
            }
            DataBaseEventStore eventStore = new DataBaseEventStore(Settings.Instance.Database);
            EventProcessor eventProcessor = new EventProcessor(eventStore);

            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            Cache.SetProcessor(eventProcessor);
            foreach (var @event in eventStore.GetAll())
                eventProcessor.Replay(@event);
            Console.WriteLine(watch.Elapsed);

            watch.Restart();
            for (int i = 0; i < 12_000; i++)
            {
                var player = Player.Create(eventProcessor, i + 2354323);
                player.AddMark(eventProcessor, Enumerable.Range(13512, 10));
            }
            Console.WriteLine(watch.Elapsed);

                Cache.PlayerCache.ToString();
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
