using System;
using System.Collections.Generic;
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

        internal void HandleEvent(DomainObjectChangedEvent e)
        {
            switch (e)
            {
                case MarkObtainedEvent e1: HandleMarkObtained(e1); break;
            }
        }

        private void HandleMarkObtained(MarkObtainedEvent e)
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
