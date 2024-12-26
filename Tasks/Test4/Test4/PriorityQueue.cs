// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Test4;

/// <summary>
/// Thread safe queue with priorities.
/// </summary>
/// <typeparam name="TValue">The type of the value stored in the queue.</typeparam>
/// <typeparam name="TPriority">The type of the priorities used in the queue.</typeparam>
public class PriorityQueue<TValue, TPriority>
    where TPriority : IComparable<TPriority>
{
    private readonly object lockObject = new ();
    private Element? head;

    /// <summary>
    /// Gets the size of the queue at some moment.
    /// </summary>
    public int Size { get; private set; }

    /// <summary>
    /// Inserts the given element with given priority to the queue.
    /// </summary>
    /// <param name="value">The value to insert.</param>
    /// <param name="priority">The priority of the element.</param>
    public void Enqueue(TValue value, TPriority priority)
    {
        lock (this.lockObject)
        {
            var (previous, current) = this.GetElementByPriority(priority);
            var elementToInsert = new Element(value, priority, current);
            if (previous is not null)
            {
                previous.Next = elementToInsert;
            }
            else
            {
                this.head = elementToInsert;
            }

            ++this.Size;
            Monitor.Pulse(this.lockObject);
        }
    }

    /// <summary>
    /// Gets the element with max priority from the queue.
    /// </summary>
    /// <returns>The value with the max priority in the queue.</returns>
    public TValue Dequeue()
    {
        lock (this.lockObject)
        {
            while (this.Size == 0)
            {
                Monitor.Wait(this.lockObject);
            }

            ArgumentNullException.ThrowIfNull(this.head);
            var valueToReturn = this.head.Value;
            this.head = this.head.Next;
            --this.Size;
            return valueToReturn;
        }
    }

    private (Element?, Element?) GetElementByPriority(TPriority priority)
    {
        Element? previous = null;
        Element? current = this.head;
        for (; current is not null && current.Priority.CompareTo(priority) >= 0; current = current.Next)
        {
            previous = current;
        }

        return (previous, current);
    }

    private class Element(TValue value, TPriority priority, Element? next = null)
    {
        public TValue Value { get; } = value;

        public TPriority Priority { get; } = priority;

        public Element? Next { get; set; } = next;
    }
}
