// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Test4.Tests;

/// <summary>
/// Tests for the priority queue implementation.
/// </summary>
public static class PriorityQueueTests
{
    /// <summary>
    /// Tests adding elements to the queue ny checking the queue size.
    /// </summary>
    [Test]
    public static void Test_EnqueueAndCheckSize_SizeIsUpdated()
    {
        var queue = new PriorityQueue<int, int>();
        Assert.That(queue.Size, Is.EqualTo(0));
        queue.Enqueue(20, 1);
        Assert.That(queue.Size, Is.EqualTo(1));
        queue.Enqueue(2, 100);
        Assert.That(queue.Size, Is.EqualTo(2));
    }

    /// <summary>
    /// Tests adding elements to the queue by obtaining max priority elements.
    /// </summary>
    [Test]
    public static void Test_EnqueueAndDequeue_ElementsAreObtainedProperly()
    {
        var queue = new PriorityQueue<string, float>();
        queue.Enqueue("qwew", 12f);
        queue.Enqueue("0", 54f);
        queue.Enqueue("ioio", 12f);
        queue.Enqueue("abc", 0);
        Assert.That(queue.Dequeue(), Is.EqualTo("0"));
        Assert.That(queue.Size, Is.EqualTo(3));
        Assert.That(queue.Dequeue(), Is.EqualTo("qwew"));
        Assert.That(queue.Dequeue(), Is.EqualTo("ioio"));
        Assert.That(queue.Dequeue(), Is.EqualTo("abc"));
        Assert.That(queue.Size, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests the concurrent element adding to the queue.
    /// </summary>
    [Test]
    public static void Test_AddElementsConcurrently_ElementsAreAddedProperly()
    {
        var queue = new PriorityQueue<int, string>();
        var firstThread = new Thread(() =>
        {
            queue.Enqueue(12, "a");
            queue.Enqueue(990, "c");
            queue.Enqueue(12, "dc");
        });
        var secondThread = new Thread(() =>
        {
            queue.Enqueue(-100, "d");
            queue.Enqueue(0, "b");
            queue.Enqueue(190, "ab");
        });
        firstThread.Start();
        secondThread.Start();
        JoinThreads([firstThread, secondThread]);
        Assert.That(queue.Size, Is.EqualTo(6));
        Assert.That(queue.Dequeue(), Is.EqualTo(12));
        Assert.That(queue.Dequeue(), Is.EqualTo(-100));
        Assert.That(queue.Dequeue(), Is.EqualTo(990));
        Assert.That(queue.Size, Is.EqualTo(3));
    }

    /// <summary>
    /// Tests the concurrent elements adding to the queue.
    /// </summary>
    [Test]
    public static void Test_AddElementsFromManyThreads_ElementsAreAddedProperly()
    {
        var numberOfThreads = 12;
        var threads = new Thread[numberOfThreads];
        var queue = new PriorityQueue<int, int>();
        for (int i = 0; i < numberOfThreads; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                Thread.Sleep(1000);
                queue.Enqueue(localI, localI);
            });
            threads[i].Start();
        }

        JoinThreads(threads);
        Assert.That(queue.Size, Is.EqualTo(numberOfThreads));
        Assert.That(queue.Dequeue(), Is.EqualTo(numberOfThreads));
        Assert.That(queue.Dequeue(), Is.EqualTo(numberOfThreads - 1));
        Assert.That(queue.Size, Is.EqualTo(numberOfThreads - 2));
    }

    /// <summary>
    /// Tests the obtaining of elements form the queue in case of adding from other queue.
    /// </summary>
    [Test]
    public static void Test_EnqueueAndDequeueFromDifferentThreads_ElementsAreObtainedProperly()
    {
        var queue = new PriorityQueue<string, int>();
        var firstThread = new Thread(() =>
        {
            Thread.Sleep(1000);
            queue.Enqueue("fas", 231);
            queue.Enqueue("2313211", 1);
            queue.Enqueue("op", -1);
        });
        var secondThread = new Thread(() =>
        {
            Assert.That(queue.Size, Is.EqualTo(0));
            Assert.That(queue.Dequeue(), Is.EqualTo("fas"));
            Assert.That(queue.Dequeue(), Is.EqualTo("2313211"));
            Assert.That(queue.Dequeue(), Is.EqualTo("op"));
        });
        firstThread.Start();
        secondThread.Start();
        JoinThreads([firstThread, secondThread]);
    }

    /// <summary>
    /// Tests the concurrent element obtaining from the queue.
    /// </summary>
    [Test]
    public static void Test_DequeueConcurrentlyFromManyThreads_ElementsAreObtainedProperly()
    {
        var numberOfThreads = 10;
        var threads = new Thread[numberOfThreads];
        var queue = new PriorityQueue<int, int>();
        for (int i = 0; i < numberOfThreads; ++i)
        {
            queue.Enqueue(42, 42);
            threads[i] = new Thread(() =>
            {
                Thread.Sleep(1000);
                Assert.That(queue.Dequeue(), Is.EqualTo(42));
            });
            threads[i].Start();
        }

        JoinThreads(threads);
    }

    private static void JoinThreads(IEnumerable<Thread> threads)
    {
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }
}
