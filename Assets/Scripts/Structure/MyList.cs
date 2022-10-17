namespace Incru.Structure
{
    using System;
    using System.Collections.Generic;

    public class MyList<T> : List<T>
    {
        public event Action<MyList<T>> Added;
        public event Action<MyList<T>> Removed;
        public event Action<MyList<T>> Changed;

        public new void Add(T item)
        {
            base.Add(item);
            Added?.Invoke(this);
            Changed?.Invoke(this);
        }

        public new void AddRange(IEnumerable<T> items)
        {
            base.AddRange(items);
            Added?.Invoke(this);
            Changed?.Invoke(this);
        }

        public new void Remove(T item)
        {
            base.Remove(item);
            Removed?.Invoke(this);
            Changed?.Invoke(this);
        }

        public new void RemoveAll(Predicate<T> items)
        {
            base.RemoveAll(items);
            Removed?.Invoke(this);
            Changed?.Invoke(this);
        }

        public new void RemoveAt(int index)
        {
            base.RemoveAt(index);
            Removed?.Invoke(this);
            Changed?.Invoke(this);
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            Removed?.Invoke(this);
            Changed?.Invoke(this);
        }

        public new void Clear()
        {
            base.Clear();
            Removed?.Invoke(this);
            Changed?.Invoke(this);
        }
    }
}