﻿// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyThreadPool;

/// <summary>
/// Class representing a pool of threads, executing given tasks.
/// </summary>
public class MyThreadPool
{
    private readonly Thread[] threads;
    private readonly Queue<Action> remainingTasks = new ();
    private readonly CancellationTokenSource cancellation = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="MyThreadPool"/> class.
    /// Starts a given number of threads processing tasks in the queue.
    /// </summary>
    /// <param name="numberOfThreads">Number of active threads.</param>
    public MyThreadPool(int numberOfThreads)
    {
        this.threads = new Thread[numberOfThreads];
        for (int i = 0; i < numberOfThreads; ++i)
        {
            this.threads[i] = new Thread(this.ProcessTasks);
            this.threads[i].Start();
        }
    }

    /// <summary>
    /// Adds specific task to the queue for one of the threads to complete.
    /// </summary>
    /// <typeparam name="TResult">Type of given method's output.</typeparam>
    /// <param name="methodToEvaluate">Method to be evaluated.</param>
    /// <returns>IMyTask implementation representing the accepted task.</returns>
    public IMyTask<TResult> Submit<TResult>(Func<TResult> methodToEvaluate)
    {
        var newTask = new MyTask<TResult>(methodToEvaluate, this.cancellation.Token);
        lock (this.remainingTasks)
        {
            this.remainingTasks.Enqueue(newTask.Complete);
            Monitor.Pulse(this.remainingTasks);
        }

        return newTask;
    }

    /// <summary>
    /// Indicates active threads to finish processing tasks.
    /// Tasks waiting in the queue will finish with an exception thrown.
    /// </summary>
    public void Shutdown()
    {
        this.cancellation.Cancel();
        Monitor.PulseAll(this.remainingTasks);
        foreach (var thread in this.threads)
        {
            thread.Join();
        }
    }

    private void ProcessTasks()
    {
        while (!this.cancellation.IsCancellationRequested)
        {
            this.TakeAndCompleteTask();
        }
    }

    private void TakeAndCompleteTask()
    {
        Action taskToComplete;
        lock (this.remainingTasks)
        {
            while (this.remainingTasks.Count == 0)
            {
                if (this.cancellation.IsCancellationRequested)
                {
                    return;
                }

                Monitor.Wait(this.remainingTasks);
            }

            taskToComplete = this.remainingTasks.Dequeue();
        }

        taskToComplete();
    }
}
