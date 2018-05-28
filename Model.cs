using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarksAppBackend
{
    public class Player
    {
        public Guid Guid { get; private set; }
        public string Name { get; private set; }
        public int ID { get; private set; }
        private List<int> _ThreeMarks = new List<int>();
        private List<int> _TwoMarks = new List<int>();
        private List<int> _OneMarks = new List<int>();
        public IEnumerable<int> ThreeMarks => _ThreeMarks;
        public IEnumerable<int> TwoMarks => _TwoMarks;
        public IEnumerable<int> OneMarks => _OneMarks;
        public Guid ClanGuid { get; private set; }

        private Player() { }

        internal static Player Create(EventProcessor processor, int id)
        {
            var @event = processor.Process(new PlayerCreatedEvent(id));
            return Cache.PlayerCache[@event.Guid];

        }

        internal void AddMark(EventProcessor processor, int tankID, int level)
        {
            processor.Process(new MarkObtainedEvent(this, tankID, level));
        }

        internal void AddMark(EventProcessor processor, IEnumerable<int> threeMarks = null, IEnumerable<int> twoMarks = null, IEnumerable<int> oneMarks = null)
        {
            IEnumerable<MarkObtainedEvent> convert(IEnumerable<int> tankIDs, int level)
            {
                if (tankIDs is null)
                    return Enumerable.Empty<MarkObtainedEvent>();
                return tankIDs.Select(id => new MarkObtainedEvent(this, id, level));
            }

            processor.ProcessMultiple(convert(threeMarks, 3).Concat(convert(twoMarks, 2)).Concat(convert(oneMarks, 1)));
        }

        internal static Player HandleEvent(PlayerCreatedEvent e)
        {
            return new Player
            {
                Guid = e.Guid,
                ID = e.ID,
            };
        }

        internal void HandleEvent(MarkObtainedEvent e)
        {
            void addMark(int level, List<int> list, Action deeper)
            {
                if (e.Level == level)
                    list.Add(e.TankID);
                else
                {
                    list.RemoveAll(i => i == e.TankID);
                    deeper?.Invoke();
                }
            }

            addMark(1, _OneMarks, () => addMark(2, _TwoMarks, () => addMark(3, _ThreeMarks, () => throw new ArgumentException("unexpected marks level"))));
        }
    }
}
