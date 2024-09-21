// Copyright (c) 2024
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
public class MyTask<TResult> : IMyTask<TResult>
{
    private readonly Func<TResult> methodToEvaluate;
    private readonly CancellationToken cancellationToken;
    private readonly object lockObject = new ();

    private TResult? result;
    private Exception? thrownException;

    /// <summary>
    /// Initializes a new instance of the <see cref="MyTask{TResult}"/> class.
    /// </summary>
    /// <param name="methodToEvaluate">Method to be evaluated.</param>
    /// <param name="cancellationToken">Token containing task cancellation data.</param>
    public MyTask(Func<TResult> methodToEvaluate, CancellationToken cancellationToken)
    {
        this.methodToEvaluate = methodToEvaluate;
        this.cancellationToken = cancellationToken;
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
    public TResult Result
    {
        get
        {
            lock (this.lockObject)
            {
                while (!this.IsCompleted && this.thrownException == null)
                {
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

    /// <summary>
    /// Creates a new task that takes this task's output as input.
    /// </summary>
    /// <typeparam name="TNewResult">Type of new task's result.</typeparam>
    /// <param name="newTask">Method to be evaluated.</param>
    /// <returns>IMyTask instance representing the new task.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newTask)
    {
        var nextTask = new MyTask<TNewResult>(
            () => newTask(this.Result), this.cancellationToken);
        return nextTask;
    }

    /// <summary>
    /// Finish this task and update properties of this instance.
    /// </summary>
    public void Complete()
    {
        if (this.cancellationToken.IsCancellationRequested)
        {
            throw new TaskCanceledException();
        }

        lock (this.lockObject)
        {
            try
            {
                this.result = this.methodToEvaluate();
                this.IsCompleted = true;
            }
            catch (Exception e)
            {
                this.thrownException = e;
            }

            Monitor.Pulse(this.lockObject);
        }
    }
}
