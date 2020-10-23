using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PriorityQueue<TElement, TPriority> : IPriorityQueue<TElement, TPriority>
    where TPriority : IComparable<TPriority>
{
    private readonly SortedDictionary<TPriority, Queue<TElement>> _sortedDictionary;
    private readonly Comparer<TPriority> _comparer;

    public PriorityQueue(Comparer<TPriority> comparer = null)
    {
        _comparer = comparer ?? Comparer<TPriority>.Default;
        _sortedDictionary = new SortedDictionary<TPriority, Queue<TElement>>(_comparer);
    }

    public void Insert(TElement item, TPriority priority)
    {
        if (_sortedDictionary.TryGetValue(priority, out Queue<TElement> queue))
        {
            queue.Enqueue(item);
        }
        else
        {
            queue = new Queue<TElement>();
            queue.Enqueue(item);

            _sortedDictionary.Add(priority, queue);
        }
    }

    public TElement Peek()
    {
        return _sortedDictionary[_sortedDictionary.Keys.ToList()[0]].Peek();
    }

    public TElement Pop()
    {
        //create shallow copy of keys
        var keyCollection = new List<TPriority>(_sortedDictionary.Keys.ToList());

        foreach(var key in keyCollection)
        {
            if (_sortedDictionary[key].Count > 0)
            {
                return _sortedDictionary[key].Dequeue();
            }
            else
            {
                _sortedDictionary.Remove(key);
            }
        }

        throw new ArgumentOutOfRangeException();
    }

    public void Remove(TElement elementToRemove)
    {
        if (Find(elementToRemove, out TElement element, out TPriority priority))
        {
            var queue = _sortedDictionary[priority];
            queue = new Queue<TElement>(queue.Where(s => s.Equals(element) == false));

            if (queue.Count == 0)
                _sortedDictionary.Remove(priority);
            else
                _sortedDictionary[priority] = queue;
        }
    }

    private bool Find(TElement elementToFind, out TElement element, out TPriority priority)
    {
        foreach (var p in _sortedDictionary.Keys)
        {
            foreach (var e in _sortedDictionary[p])
            {
                if (e.Equals(elementToFind))
                {
                    element = e;
                    priority = p;
                    return true;
                }
            }
        }

        element = default;
        priority = default;
        return false;
    }

    /// <summary>
    /// Returns number of elements
    /// </summary>
    public int Count()
    {
        return _sortedDictionary.Sum(q => q.Value.Count);
    }

    /// <summary>
    /// Returns number of elements with same priority
    /// </summary>
    public int Count(TPriority priority)
    {
        return _sortedDictionary[priority].Count;
    }
}
