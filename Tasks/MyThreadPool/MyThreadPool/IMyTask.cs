// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyThreadPool;

/// <summary>
/// Interface representing a task.
/// Contains data about task's completion.
/// </summary>
/// <typeparam name="TResult">Type of task's result.</typeparam>
public interface IMyTask<TResult>
{
    /// <summary>
    /// Gets a value indicating whether a task was completed.
    /// </summary>
    public bool IsCompleted { get; }

    /// <summary>
    /// Gets a result of task's completion.
    /// If tasks evaluation finished with an exception, throws
    /// AggregateException containing thrown Exception instance.
    /// Blocks the calling thread until task's evaluation is finished.
    /// </summary>
    public TResult Result { get; }

    /// <summary>
    /// Creates a new task that takes this task's output as input.
    /// </summary>
    /// <typeparam name="TNewResult">Type of new task's result.</typeparam>
    /// <param name="newTask">Method to be evaluated.</param>
    /// <returns>IMyTask instance representing the new task.</returns>
    public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newTask);
}
