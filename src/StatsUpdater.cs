using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WGApi;

namespace MarksAppBackend
{
    class StatsUpdater
    {
        public Stack<int> IDStack { get; private set; } = new Stack<int>();
        private readonly WGApiClient ApiClient;

        public StatsUpdater(WGApiClient client)
        {
            ApiClient = client;
        }

        public async Task Run()
        {

        }
    }

    class IDStack
    {
        private readonly Stack<List<int>> Stack = new Stack<List<int>>();
        private object Lock = new object();
        private readonly int ListCapacity;

        public IDStack(int listCapacity)
        {
            ListCapacity = listCapacity;
        }

        public IEnumerable<int> Pop()
        {
            lock (Lock)
            {
                return Stack.Pop();
            }
        }

        public void Push(int value)
        {
            lock (Lock)
            {
                var list = Stack.Peek();
                if (list.Count < ListCapacity)
                    list.Add(value);
                else
                    Stack.Push(new List<int>{ value });
            }
        }

        public void Push(IEnumerable<int> values)
        {
            lock (Lock)
            {
                values = values.ToArray();
                var list = Stack.Peek();
                var toAdd = ListCapacity - list.Count;
                list.AddRange(values.Take(toAdd));
                values = values.Skip(toAdd).ToArray();

                while (values.Count() > 0)
                {
                    Stack.Push(new List<int>(values.Take(ListCapacity)));
                    values = values.Skip(ListCapacity).ToArray();
                }
            }
        }
    }
}
