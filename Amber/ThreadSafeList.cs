using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Amber
{
    public sealed class ThreadSafeList<T>: IEnumerable
    {
        private readonly List<T> _mList = new List<T>();
        private readonly object _mLock = new object();

        public void Add(T value)
        {
            lock (_mLock)
            {
                _mList.Add(value);
            }
        }

        public bool TryRemove(T value)
        {
            lock (_mLock)
            {
                return _mList.Remove(value);
            }
        }

        public bool TryGet(int index, out T value)
        {
            lock (_mLock)
            {
                if (index < _mList.Count)
                {
                    value = _mList[index];
                    return true;
                }
                value = default(T);
                return false;
            }
        }

        public bool TryAdd(T value, int millisecondsTimeout)
        {
            if (!Monitor.TryEnter(_mLock, millisecondsTimeout)) return false;
            try
            {
                _mList.Add(value);
                return true;
            }
            finally
            {
                Monitor.Exit(_mLock);
            }
        }

        public bool TryGetRandom(out T value)
        {
            lock (_mLock)
                if (0 < _mList.Count)
                {
                    var randomIndex = new Random();
                    value = _mList[randomIndex.Next(_mList.Count)];
                    return true;
                }
            value = default(T);
            return false;
        }

        public int Size()
        {
            lock (_mLock)
            {
                return _mList.Count;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock (_mLock)
            {
                //return ((IEnumerable<T>)_mList).GetEnumerator();
                return _mList.ToList().GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
