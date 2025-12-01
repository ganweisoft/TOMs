// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public sealed class EGroupCollection<T> : ICollection<T>, ISerializable, IDeserializationCallback, IReadOnlyCollection<T>
{
    private EGroupLink<T> _head;
    private EGroupLink<T> _current;
    private int m_count;
    private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
    private const int AnyTime = 10;
    public EGroupCollection()
    {
    }

    public EGroupCollection(T data)
    {
        _head = new EGroupLink<T>(data);
        m_count = 1;
        _current = _head;
    }

    public int Count => m_count;

    public bool IsReadOnly => false;

    public bool TryAdd(T item)
    {
        if (item == null)
            return false;

        if (!_lock.TryEnterUpgradeableReadLock(AnyTime))
            return false;

        try
        {
            if (!_lock.TryEnterWriteLock(AnyTime))
                return false;

            if (m_count == 0)
            {
                _head = new EGroupLink<T>(item);
                _current = _head;
            }
            else if (m_count == 1)
            {
                _current = new EGroupLink<T>(item);
                _head.SetNext(_current);
                _current.SetLast(_head);
            }
            else
            {
                var tmp = new EGroupLink<T>(item);
                _current.SetNext(tmp);
                tmp.SetLast(_current);
                _current = tmp;
            }

            Interlocked.Increment(ref m_count);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
        _lock.ExitUpgradeableReadLock();

        return true;
    }

    public void Add(T item)
    {
        _ = TryAdd(item);
    }


    public void Clear()
    {
        EGroupLink<T> current = _head;
        while (current != null)
        {
            var tmp = current.Next;
            current.Invalidate();
            current = tmp;
        }

        _head = null;
        _current = null;
        m_count = 0;
    }

    public bool Contains(T item)
    {
        EGroupLink<T> current = _head;
        while (current != null)
        {
            if (current.Data.Equals(item))
                return true;
            current = current.Next;
        }
        return false;
    }

    public bool Get(int key,out T value)
    {
        value = default(T);
        EGroupLink<T> current = _head;
        while (current != null)
        {
            if (current.Data.Equals(key))
            {
                value = current.Data;
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    public void CopyTo(IEnumerable<T> array)
    {
        Span<T> list = array.ToArray().AsSpan();
        Copy(list);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Copy(array.AsSpan(0,arrayIndex+1));
    }

    private void Copy(Span<T> array)
    {
        Clear();
        if (!array.IsEmpty)
        {
            EGroupLink<T> head = new EGroupLink<T>(array[0]);
            EGroupLink<T> current = head;
            if (array.Length >= 1)
            {
                for (int i = 1; i < array.Length; i++)
                {
                    var tmp = new EGroupLink<T>(array[i]);
                    tmp.SetLast(current);
                    current.SetNext(tmp);
                    current = tmp;
                }
            }
            _head = head;
            m_count = array.Length;
        }    
    }

    public IEnumerator<T> GetEnumerator()
    {
        EGroupLink<T> current = _head;
        while (current != null)
        {
            var tmp = current.Next;
            yield return current.Data;
            current = tmp;
        }
    }

    public bool Remove(int key)
    {
        EGroupLink<T> current = _head;
        while (current != null)
        {
            if (current.Data.Equals(key))
            {
                var last = current.Last;
                var next = current.Next;
                current.Invalidate();
                last.SetNext(next);
                next.SetLast(last);
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    public bool Remove(T item)
    {
        EGroupLink<T> current = _head;
        while (current != null)
        {
            if (current.Data.Equals(item))
            {
                var last = current.Last;
                var next = current.Next;
                current.Invalidate();
                last.SetNext(next);
                next.SetLast(last);
                return true;
            }
            current = current.Next;
        }
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        EGroupLink<T> current = _head;
        while (current != null)
        {
            var tmp = current.Next;
            yield return current.Data;
            current = tmp;
        }
    }

    
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        var properties = typeof(T).GetProperties();

        EGroupLink<T> current = _head;
        while (current != null)
        {
            var tmp = current.Next;

            foreach (PropertyInfo item in properties)
            {
                info.AddValue(item.Name, item.GetValue(current.Data));
            }
            current = tmp;
        }
    }

    public void OnDeserialization(object sender)
    {
        if (sender == null)
            return;
    }

    protected EGroupCollection(SerializationInfo serializationInfo, StreamingContext streamingContext)
    {
        GetObjectData(serializationInfo,streamingContext);
    }

    ~EGroupCollection()
    {
        Clear();
        if (_lock != null) _lock.Dispose();
    }
}
