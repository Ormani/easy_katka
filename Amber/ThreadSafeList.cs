using System.Collections;
using System.Collections.Generic;

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

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) _mList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
