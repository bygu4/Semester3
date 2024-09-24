// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.namespace MyThreadPool.Tests;

namespace MyThreadPool.Tests;

public static class MyThreadPoolTests
{
    [Test]
    public static void Test_NegativeNumberOfThreads_ThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MyThreadPool(-4));
    }

    [Test]
    public static void Test_NumberOfThreadsZero_ThrowException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MyThreadPool(0));
    }

    [Test]
    public static void Test_CheckNumberOfActiveThreads()
    {
        var numberOfThreads = Environment.ProcessorCount;
        var threadPool = new MyThreadPool(numberOfThreads);
        var tasks = new IMyTask<int>[numberOfThreads];
        for (int i = 0; i < numberOfThreads; ++i)
        {
            tasks[i] = threadPool.Submit(() => { Thread.Sleep(100); return 1; });
        }

        Thread.Sleep(150);
        foreach (var task in tasks)
        {
            AssertThatTaskIsCompleted_NonBlock(task, 1);
        }
    }

    [Test]
    public static void Test_FewerTasksThenThreads()
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

    [Test]
    public static void Test_MoreTasksThenThreads()
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

    [Test]
    public static void Test_TasksOfDifferentType()
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

    [Test]
    public static void Test_TaskThrowsException()
    {
        var numberOfThreads = 6;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task = threadPool.Submit<string[]>(() => throw new IndexOutOfRangeException());
        AssertThatTaskThrowsException(task, typeof(IndexOutOfRangeException));
    }

    [Test]
    public static void Test_SingleContinueWith()
    {
        var numberOfThreads = 1;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task = threadPool.Submit(
            () => "12345678").ContinueWith(
                (s) => int.Parse(s.Reverse().ToArray()));
        AssertThatTaskIsCompleted_BlockThread(task, 87654321);
    }

    [Test]
    public static void Test_ContinueTaskWithException()
    {
        var numberOfThreads = 3;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task = threadPool.Submit<bool>(
            () => throw new SystemException()).ContinueWith((x) => !x);
        AssertThatTaskThrowsException(task, typeof(AggregateException));
    }

    [Test]
    public static void Test_MultipleContinueWith()
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
                    (x) => x.ToString());
        }

        for (int i = 0; i < numberOfTasks; ++i)
        {
            AssertThatTaskIsCompleted_BlockThread(
                tasks[i], Math.Pow(i + 10, 2).ToString());
        }
    }

    [Test]
    public static void Test_ContinueFromTheSameTask()
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
            tasks[i] = baseTask.ContinueWith((s) => s[localI]);
        }

        for (int i = 0; i < numberOfTasks; ++i)
        {
            AssertThatTaskIsCompleted_BlockThread(tasks[i], testString[i]);
        }
    }

    [Test]
    public static void Test_ConsecutiveContinueWith()
    {
        var numberOfThreads = 12;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task = threadPool.Submit<int[]>(
            () => [15, 88, -1, 0, 2]).ContinueWith(
            (arr) => { Array.Sort(arr); return arr; }).ContinueWith(
            (arr) => arr.Last()).ContinueWith(
            (x) => x * x).ContinueWith(
            (x) => x.ToString());
        AssertThatTaskIsCompleted_BlockThread(task, "7744");
    }

    [Test]
    public static void Test_TaskWasCanceled_ThrowException()
    {
        var numberOfThreads = 1;
        var threadPool = new MyThreadPool(numberOfThreads);
        var task = threadPool.Submit(() => { Thread.Sleep(10000); return 1; });
        threadPool.Shutdown();
        AssertThatTaskWasCanceled(task);
    }

    [Test]
    public static void Test_TaskWasAddedAfterShutdown_ThrowException()
    {
        var numberOfThreads = 10;
        var threadPool = new MyThreadPool(numberOfThreads);
        threadPool.Shutdown();
        var task = threadPool.Submit(() => "I'll be a little late");
        AssertThatTaskWasCanceled(task);
    }

    [Test]
    public static void Test_ContinueFromCanceledTask_ThrowException()
    {
        var numberOfThreads = 2;
        var threadPool = new MyThreadPool(numberOfThreads);
        var baseTask = threadPool.Submit(() => { Thread.Sleep(10000); return 42; });
        threadPool.Shutdown();
        var task = baseTask.ContinueWith((x) => x * x);
        AssertThatTaskWasCanceled(task);
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
        var exception = Assert.Throws<AggregateException>(() => { T _ = task.Result; });
        Assert.That(exception.InnerException, Is.Not.Null);
        Assert.That(exception.InnerException.GetType(), Is.EqualTo(exceptionType));
        Assert.That(task.IsCompleted, Is.False);
    }

    private static void AssertThatTaskWasCanceled<T>(IMyTask<T> task)
    {
        Assert.Throws<TaskCanceledException>(() => { T _ = task.Result; });
        Assert.That(task.IsCompleted, Is.False);
    }
}
