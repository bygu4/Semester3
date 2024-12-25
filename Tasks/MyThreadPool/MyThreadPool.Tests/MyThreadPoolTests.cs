// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

#pragma warning disable SA1010 // Opening square brackets should be spaced correctly

namespace MyThreadPool.Tests;

/// <summary>
/// Tests for MyThreadPool implementation.
/// </summary>
public static class MyThreadPoolTests
{
    /// <summary>
    /// Tests the thread pool creation with an invalid thread number.
    /// </summary>
    [Test]
    public static void Test_NegativeNumberOfThreads_ThrowException()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new MyThreadPool(-4));

    /// <summary>
    /// Tests the thread pool creation with an invalid thread number.
    /// </summary>
    [Test]
    public static void Test_NumberOfThreadsZero_ThrowException()
        => Assert.Throws<ArgumentOutOfRangeException>(() => new MyThreadPool(0));

    /// <summary>
    /// Tests the number of active threads in the pool.
    /// </summary>
    [Test]
    public static void Test_CheckNumberOfActiveThreads_TasksAreCompleted()
    {
        var numberOfThreads = Environment.ProcessorCount;
        var threadPool = new MyThreadPool(numberOfThreads);
        var tasks = new IMyTask<int>[numberOfThreads];
        for (int i = 0; i < numberOfThreads; ++i)
        {
            tasks[i] = threadPool.Submit(() =>
            {
                Thread.Sleep(100);
                return 1;
            });
        }

        Thread.Sleep(150);
        foreach (var task in tasks)
        {
            AssertThatTaskIsCompleted_NonBlock(task, 1);
        }
    }

    /// <summary>
    /// Tests the thread pool with fewer given tasks than active threads.
    /// </summary>
    [Test]
    public static void Test_FewerTasksThanThreads_TasksAreCompleted()
    {
        var numberOfThreads = 16;
        var numberOfTasks = 5;
        var threadPool = new MyThreadPool(numberOfThreads);
        var tasks = new IMyTask<string>[numberOfTasks];
        for (int i = 0; i < numberOfTasks; ++i)
        {
            var localI = i;
            tasks[i] = threadPool.Submit(() => Math.Pow(localI, 3).ToString());
        }

        for (int i = 0; i < numberOfTasks; ++i)
        {
            AssertThatTaskIsCompleted_BlockThread(tasks[i], Math.Pow(i, 3).ToString());
        }
    }

    /// <summary>
    /// Tests the thread pool with more given tasks than active threads.
    /// </summary>
    [Test]
    public static void Test_MoreTasksThanThreads_TasksAreCompleted()
    {
        var numberOfThreads = 4;
        var numberOfTasks = 32;
        var threadPool = new MyThreadPool(numberOfThreads);
        var tasks = new IMyTask<bool>[numberOfTasks];
        for (int i = 0; i < numberOfTasks; ++i)
        {
            var localI = i;
            tasks[i] = threadPool.Submit(() => localI % 7 == 0);
        }

        for (int i = 0; i < numberOfTasks; ++i)
        {
           AssertThatTaskIsCompleted_BlockThread(tasks[i], i % 7 == 0);
        }
    }

    /// <summary>
    /// Tests the thread pool with given tasks of different return types.
    /// </summary>
    [Test]
    public static void Test_TasksOfDifferentType_TasksAreCompleted()
    {
        var numberOfThreads = 8;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task1 = threadPool.Submit(() => 512f * 89 / 5);
        var task2 = threadPool.Submit(() => "amogus".ToUpper());
        var task3 = threadPool.Submit(() => Enumerable.Range(1, 7).ToArray());
        var task4 = threadPool.Submit(() => "aaaadcba".IndexOf('c'));
        AssertThatTaskIsCompleted_BlockThread(task1, 9113.6f);
        AssertThatTaskIsCompleted_BlockThread(task2, "AMOGUS");
        AssertThatTaskIsCompleted_BlockThread(task3, [1, 2, 3, 4, 5, 6, 7]);
        AssertThatTaskIsCompleted_BlockThread(task4, 5);
    }

    /// <summary>
    /// Tests the thread pool with a task that throws an exception.
    /// </summary>
    [Test]
    public static void Test_TaskThrowsException_ThrowException()
    {
        var numberOfThreads = 6;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task = threadPool.Submit<string[]>(() => throw new IndexOutOfRangeException());
        AssertThatTaskThrowsException(task, typeof(IndexOutOfRangeException));
    }

    /// <summary>
    /// Tests the single task continuation.
    /// </summary>
    [Test]
    public static void Test_SingleContinueWith_TasksAreCompleted()
    {
        var numberOfThreads = 1;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task = threadPool.Submit(
            () => "12345678").ContinueWith(
                s => int.Parse(s.Reverse().ToArray()));
        AssertThatTaskIsCompleted_BlockThread(task, 87654321);
    }

    /// <summary>
    /// Tests the task continuation after a delay.
    /// </summary>
    [Test]
    public static void Test_ContinueTaskWithDelay_TasksAreCompleted()
    {
        var numberOfThreads = 2;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task = threadPool.Submit(() => "pupupupu");
        Thread.Sleep(100);
        var continuation = task.ContinueWith(s => s.Length);
        AssertThatTaskIsCompleted_BlockThread(continuation, 8);
    }

    /// <summary>
    /// Tests the continuation of a task that throws an exception.
    /// </summary>
    [Test]
    public static void Test_ContinueTaskWithException_ThrowException()
    {
        var numberOfThreads = 3;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task = threadPool.Submit<bool>(
            () => throw new SystemException()).ContinueWith(x => !x);
        AssertThatTaskThrowsException(task, typeof(AggregateException));
    }

    /// <summary>
    /// Tests the continuation of different tasks.
    /// </summary>
    [Test]
    public static void Test_MultipleContinueWith_TasksAreCompleted()
    {
        var numberOfThreads = 7;
        var numberOfTasks = 7;
        var threadPool = new MyThreadPool(numberOfThreads);
        var tasks = new IMyTask<string>[numberOfTasks];
        for (int i = 0; i < numberOfTasks; ++i)
        {
            var localI = i;
            tasks[i] = threadPool.Submit(
                () => Math.Pow(localI + 10, 2)).ContinueWith(
                    x => x.ToString());
        }

        for (int i = 0; i < numberOfTasks; ++i)
        {
            AssertThatTaskIsCompleted_BlockThread(
                tasks[i], Math.Pow(i + 10, 2).ToString());
        }
    }

    /// <summary>
    /// Tests the multiple continuation of the same task.
    /// </summary>
    [Test]
    public static void Test_ContinueFromTheSameTask_TasksAreCompleted()
    {
        var numberOfThreads = 2;
        var numberOfTasks = 6;
        var testString = "qwerty";
        var threadPool = new MyThreadPool(numberOfThreads);
        var baseTask = threadPool.Submit(() => testString);
        var tasks = new IMyTask<char>[numberOfTasks];
        for (int i = 0; i < numberOfTasks; ++i)
        {
            var localI = i;
            tasks[i] = baseTask.ContinueWith(s => s[localI]);
        }

        for (int i = 0; i < numberOfTasks; ++i)
        {
            AssertThatTaskIsCompleted_BlockThread(tasks[i], testString[i]);
        }
    }

    /// <summary>
    /// Tests the consecutive continuation of a task.
    /// </summary>
    [Test]
    public static void Test_ConsecutiveContinueWith_TasksAreCompleted()
    {
        var numberOfThreads = 12;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task = threadPool.Submit<int[]>(() =>
            [15, 88, -1, 0, 2]).ContinueWith(arr =>
            {
                Array.Sort(arr);
                return arr;
            }).ContinueWith(
            arr => arr.Last()).ContinueWith(
            x => x * x).ContinueWith(
            x => x.ToString());
        AssertThatTaskIsCompleted_BlockThread(task, "7744");
    }

    /// <summary>
    /// Tests the continuation from a continued task.
    /// </summary>
    [Test]
    public static void Test_NestedContinuation_TasksAreCompleted()
    {
        var numberOfThreads = 5;
        var threadPool = new MyThreadPool(numberOfThreads);
        var baseTask = threadPool.Submit<int[]>(() => [55, 8, 1, 0, 20]);
        var nextTask = baseTask.ContinueWith(arr =>
        {
            Array.Sort(arr);
            return arr;
        });
        var continuations = new IMyTask<int>[numberOfThreads];
        for (int i = 0; i < numberOfThreads; ++i)
        {
            continuations[i] = nextTask.ContinueWith(arr => arr.Last());
        }

        foreach (var continuation in continuations)
        {
            AssertThatTaskIsCompleted_BlockThread(continuation, 55);
        }
    }

    /// <summary>
    /// Tests the order of the continued task completion.
    /// </summary>
    [Test]
    public static void Test_ContinueFromLongToRunTask_ContinuationsAreCompletedLast()
    {
        var numberOfThreads = 10;
        var threadPool = new MyThreadPool(numberOfThreads);
        var baseTask = threadPool.Submit(() =>
        {
            Thread.Sleep(10000);
            return 1;
        });
        var continuations = new IMyTask<bool>[numberOfThreads];
        for (int i = 0; i < numberOfThreads; ++i)
        {
            continuations[i] = baseTask.ContinueWith(x => x > 0);
        }

        for (int i = 0; i < numberOfThreads; ++i)
        {
            var newTask = threadPool.Submit(() => 99f);
            Thread.Sleep(50);
            AssertThatTaskIsCompleted_NonBlock(newTask, 99f);
        }

        Thread.Sleep(10000);
        foreach (var continuation in continuations)
        {
            AssertThatTaskIsCompleted_NonBlock(continuation, true);
        }
    }

    /// <summary>
    /// Tests the task completion after shutdown during evaluation.
    /// </summary>
    [Test]
    public static void Test_ShutdownDuringTaskCompletion_TaskIsCompleted()
    {
        var numberOfThreads = 1;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task = threadPool.Submit(() =>
        {
            Thread.Sleep(10000);
            return 1;
        });
        Thread.Sleep(100);
        threadPool.Shutdown();
        AssertThatTaskIsCompleted_NonBlock(task, 1);
    }

    /// <summary>
    /// Tests the completion of task that wasn't being evaluated during shutdown.
    /// </summary>
    [Test]
    public static void Test_ShutdownWithTaskNotBeingEvaluated_TaskIsCanceled()
    {
        var numberOfThreads = 1;
        var threadPool = new MyThreadPool(numberOfThreads);
        var firstTask = threadPool.Submit(() =>
        {
            Thread.Sleep(10000);
            return "first";
        });
        var secondTask = threadPool.Submit(() => "second");
        Thread.Sleep(100);
        threadPool.Shutdown();
        AssertThatTaskIsCompleted_NonBlock(firstTask, "first");
        AssertThatTaskWasCanceled(secondTask);
    }

    /// <summary>
    /// Tests the task submission after shutdown.
    /// </summary>
    [Test]
    public static void Test_TaskAddedAfterShutdown_TaskIsCanceled()
    {
        var numberOfThreads = 10;
        var threadPool = new MyThreadPool(numberOfThreads);
        threadPool.Shutdown();
        var task = threadPool.Submit(() => "I'll be a little late");
        AssertThatTaskWasCanceled(task);
    }

    /// <summary>
    /// Tests the task submission with continuation after shutdown.
    /// </summary>
    [Test]
    public static void Test_AddTaskWithContinuationAfterShutdown_TaskIsCanceled()
    {
        var numberOfThreads = 3;
        var threadPool = new MyThreadPool(numberOfThreads);
        threadPool.Shutdown();
        var task = threadPool.Submit(() => "abc").ContinueWith(s => s.IndexOf('b'));
        AssertThatTaskWasCanceled(task);
    }

    /// <summary>
    /// Tests the task continuation after shutdown.
    /// </summary>
    [Test]
    public static void Test_ContinueTaskAfterShutdown_TaskIsCanceled()
    {
        var numberOfThreads = 2;
        var threadPool = new MyThreadPool(numberOfThreads);
        var baseTask = threadPool.Submit(() =>
        {
            Thread.Sleep(10000);
            return 42;
        });
        threadPool.Shutdown();
        var task = baseTask.ContinueWith(x => x * x);
        AssertThatTaskWasCanceled(task);
    }

    /// <summary>
    /// Tests the concurrent task submission.
    /// </summary>
    [Test]
    public static void Test_SubmitFromDifferentThreads_TasksAreCompleted()
    {
        var numberOfThreads = 6;
        var threadPool = new MyThreadPool(numberOfThreads);
        var threads = new Thread[numberOfThreads];
        for (int i = 0; i < numberOfThreads; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                var task = threadPool.Submit(() =>
                {
                    Thread.Sleep(100);
                    return localI;
                });
                AssertThatTaskIsCompleted_BlockThread(task, localI);
            });
            threads[i].Start();
        }

        JoinThreads(threads);
    }

    /// <summary>
    /// Tests the concurrent task continuation.
    /// </summary>
    [Test]
    public static void Test_ContinueTaskInDifferentThreads_TasksAreCompleted()
    {
        var numberOfThreads = 4;
        var threadPool = new MyThreadPool(numberOfThreads);
        var threads = new Thread[numberOfThreads];
        var baseTask = threadPool.Submit(() => "oollqqqqw");
        for (int i = 0; i < numberOfThreads; ++i)
        {
            threads[i] = new Thread(() =>
            {
                var continuation = baseTask.ContinueWith(s =>
                {
                    Thread.Sleep(100);
                    return s.Count(c => c == 'q');
                });
                AssertThatTaskIsCompleted_BlockThread(continuation, 4);
            });
            threads[i].Start();
        }

        JoinThreads(threads);
    }

    /// <summary>
    /// Tests the concurrent thread pool shutdown.
    /// </summary>
    [Test]
    public static void Test_ShutdownFromOtherThreadDuringCompletion_TaskIsCompleted()
    {
        var numberOfThreads = 4;
        var threadPool = new MyThreadPool(numberOfThreads);
        var taskThread = new Thread(() =>
        {
            var task = threadPool.Submit(() =>
            {
                Thread.Sleep(10000);
                return 42;
            });
            AssertThatTaskIsCompleted_BlockThread(task, 42);
        });
        var shutdownThread = new Thread(() =>
        {
            Thread.Sleep(100);
            threadPool.Shutdown();
        });

        taskThread.Start();
        shutdownThread.Start();
        JoinThreads([taskThread, shutdownThread]);
    }

    private static void AssertThatTaskIsCompleted_BlockThread<T>(
        IMyTask<T> task, T expectedResult)
    {
        Assert.That(task.Result, Is.EqualTo(expectedResult));
        Assert.That(task.IsCompleted, Is.True);
    }

    private static void AssertThatTaskIsCompleted_NonBlock<T>(
        IMyTask<T> task, T expectedResult)
    {
        Assert.That(task.IsCompleted, Is.True);
        Assert.That(task.Result, Is.EqualTo(expectedResult));
    }

    private static void AssertThatTaskThrowsException<T>(
        IMyTask<T> task, Type exceptionType)
    {
        var exception = Assert.Throws<AggregateException>(() => { _ = task.Result; });
        Assert.That(exception?.InnerException, Is.Not.Null);
        Assert.That(exception?.InnerException?.GetType(), Is.EqualTo(exceptionType));
        Assert.That(task.IsCompleted, Is.True);
    }

    private static void AssertThatTaskWasCanceled<T>(IMyTask<T> task)
    {
        Assert.Throws<TaskCanceledException>(() => { _ = task.Result; });
        Assert.That(task.IsCompleted, Is.False);
    }

    private static void JoinThreads(IEnumerable<Thread> threads)
    {
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}
