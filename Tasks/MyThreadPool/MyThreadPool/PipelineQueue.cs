// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyThreadPool;

/// <summary>
/// Queue that supports passing its items to the specified one.
/// </summary>
/// <typeparam name="T">The type of the items in the queue.</typeparam>
internal class PipelineQueue<T> : Queue<T>
{
    private readonly PipelineQueue<T>? nextQueue;
    private readonly Func<bool>? condition;

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineQueue{T}"/> class.
    /// </summary>
    /// <param name="nextQueue">Queue to pass items to.</param>
    /// <param name="condition">Condition to be satisfied in order to pass the items.</param>
    public PipelineQueue(
        PipelineQueue<T>? nextQueue = null,
        Func<bool>? condition = null)
    {
        this.nextQueue = nextQueue;
        this.condition = condition;
    }

    /// <summary>
    /// Add the given item to the queue and pass to next if possible.
    /// Notifies a blocked thread of queue's change of condition.
    /// </summary>
    /// <param name="item">The item to add to the queue.</param>
    public new void Enqueue(T item)
    {
        lock (this)
        {
            base.Enqueue(item);
            this.PassToNext();
            Monitor.Pulse(this);
        }
    }

    /// <summary>
    /// Take items from the queue and add them to the next one if possible.
    /// </summary>
    public void PassToNext()
    {
        if (this.nextQueue is not null && this.condition is not null && this.condition())
        {
            lock (this)
            {
                while (this.Count > 0)
                {
                    this.nextQueue.Enqueue(this.Dequeue());
                }
            }
        }
    }
}
