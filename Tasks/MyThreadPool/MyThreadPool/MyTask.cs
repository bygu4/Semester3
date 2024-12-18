// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyThreadPool;

/// <summary>
/// Class representing a task.
/// Contains data about task's completion.
/// </summary>
/// <typeparam name="TResult">Type of task's result.</typeparam>
internal class MyTask<TResult> : IMyTask<TResult>
{
    private readonly Func<TResult> methodToEvaluate;
    private readonly PipelineQueue<Action> tasksToContinueWith;
    private readonly CancellationToken cancellationToken;
    private readonly object lockObject = new ();

    private TResult? result;
    private Exception? thrownException;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
    /// Adds this task's completion as an Action to given queue.
    /// Notifies a blocked thread of queue's change of condition.
    /// </summary>
    /// <param name="methodToEvaluate">Method to be evaluated.</param>
    /// <param name="taskQueue">Queue to add this task to.</param>
    /// <param name="cancellationToken">Token containing task cancellation data.</param>
    public MyTask(
        Func<TResult> methodToEvaluate,
        PipelineQueue<Action> taskQueue,
        CancellationToken cancellationToken)
    {
        this.methodToEvaluate = methodToEvaluate;
        this.tasksToContinueWith = new PipelineQueue<Action>(taskQueue, () => this.IsCompleted);
        this.cancellationToken = cancellationToken;
        taskQueue.Enqueue(this.Complete);
    }

    /// <summary>
    /// Gets a value indicating whether a task was completed.
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Gets a result of task's completion.
    /// If tasks evaluation finished with an exception, throws
    /// AggregateException containing thrown Exception instance.
    /// Blocks the calling thread until task's evaluation is finished.
    /// </summary>
    public TResult Result => this.GetResult();

    /// <summary>
    /// Creates a new task that takes this task's output as input.
    /// </summary>
    /// <typeparam name="TNewResult">Type of new task's result.</typeparam>
    /// <param name="newTask">Method to be evaluated.</param>
    /// <returns>IMyTask instance representing the new task.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newTask)
        => new MyTask<TNewResult>(
            () => newTask(this.Result), this.tasksToContinueWith, this.cancellationToken);

    private void Complete()
    {
        lock (this.lockObject)
        {
            if (this.cancellationToken.IsCancellationRequested)
            {
                Monitor.PulseAll(this.lockObject);
                return;
            }

            try
            {
                this.result = this.methodToEvaluate();
            }
            catch (Exception e)
            {
                this.thrownException = e;
            }

            this.IsCompleted = true;
            Monitor.PulseAll(this.lockObject);
            this.tasksToContinueWith.PassToNext();
        }
    }

    private TResult GetResult()
    {
        lock (this.lockObject)
        {
            while (!this.IsCompleted)
            {
                if (this.cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }

                Monitor.Wait(this.lockObject);
            }

            if (this.thrownException != null)
            {
                throw new AggregateException(this.thrownException);
            }

            ArgumentNullException.ThrowIfNull(this.result);
            return this.result;
        }
    }
}
