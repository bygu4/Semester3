// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

#pragma warning disable SA1011 // Closing square brackets should be spaced correctly

namespace SimpleFTP.Tests;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Tests for the FTP server and client.
/// </summary>
public static class FTPTests
{
    private const string HostName = "localhost";
    private const int Port = 42000;

    private const string TestFiles = "TestFiles";
    private const string SomeDirectory = "TestFiles/SomeDirectory";
    private const string SomeTextFile = "TestFiles/SomeTextFile.txt";
    private const string AnotherTextFile = "TestFiles/SomeDirectory/AnotherTextFile.doc";
    private const string SomeImage = "TestFiles/SomeDirectory/SomeImage.jpg";

    private static Server? server;

    /// <summary>
    /// Starts the server before the test.
    /// </summary>
    [SetUp]
    public static void SetupServer()
    {
        server = new Server(IPAddress.Any, Port);
        server.Start();
    }

    /// <summary>
    /// Stops the server after the test.
    /// </summary>
    [TearDown]
    public static void StopServer()
    {
        server?.Dispose();
    }

    /// <summary>
    /// Tests the client trying to access wrong port.
    /// </summary>
    [Test]
    [Timeout(60000)]
    public static void TryToAccessWrongPort_ThrowException()
    {
        var client = new Client(HostName, Port - 1);
        Assert.ThrowsAsync<SocketException>(
            async () => await client.List(TestFiles));
        Assert.ThrowsAsync<SocketException>(
            async () => await client.Get(SomeTextFile));
    }

    /// <summary>
    /// Tests the client trying to access stopped server.
    /// </summary>
    [Test]
    [Timeout(60000)]
    public static void TryToAccessStoppedServer_ThrowException()
    {
        server?.Dispose();
        var client = new Client(HostName, Port);
        Assert.ThrowsAsync<SocketException>(
            async () => await client.List(SomeDirectory));
        Assert.ThrowsAsync<SocketException>(
            async () => await client.Get(AnotherTextFile));
    }

    /// <summary>
    /// Tests accessing the server after restart.
    /// </summary>
    /// <returns>The task representing the test.</returns>
    [Test]
    [Timeout(60000)]
    public static async Task AccessServerAfterRestart()
    {
        server?.Dispose();
        server?.Start();
        var client = new Client(HostName, Port);
        var content = await client.Get(SomeImage);
        var actualContent = await File.ReadAllBytesAsync(SomeImage);
        Assert.That(content, Is.EqualTo(actualContent));
    }

    /// <summary>
    /// Tests the directory listing containing nested directories.
    /// </summary>
    /// <returns>The task representing the test.</returns>
    [Test]
    [Timeout(60000)]
    public static async Task TestList_ListTestFiles()
    {
        var client = new Client(HostName, Port);
        var files = await client.List(TestFiles);
        Assert.That(files, Is.EquivalentTo(
            new (string, bool)[]
            {
                (SomeDirectory, true),
                (SomeTextFile, false),
            }));
    }

    /// <summary>
    /// Tests the directory listing not containing nested directories.
    /// </summary>
    /// <returns>The task representing the test.</returns>
    [Test]
    [Timeout(60000)]
    public static async Task TestList_ListSomeDirectory()
    {
        var client = new Client(HostName, Port);
        var files = await client.List(SomeDirectory);
        Assert.That(files, Is.EquivalentTo(
            new (string, bool)[]
            {
                (AnotherTextFile, false),
                (SomeImage, false),
            }));
    }

    /// <summary>
    /// Tests the client trying to list non existing directory.
    /// </summary>
    /// <returns>The task representing the test.</returns>
    [Test]
    [Timeout(60000)]
    public static async Task TestList_TryToListNonExistentDirectory_ReturnNull()
    {
        var client = new Client(HostName, Port);
        var files = await client.List("sdafasfsa");
        Assert.That(files, Is.Null);
    }

    /// <summary>
    /// Tests the client trying to list not a directory.
    /// </summary>
    /// <returns>The task representing the test.</returns>
    [Test]
    [Timeout(60000)]
    public static async Task TestList_TryToListNonDirectory_ReturnNull()
    {
        var client = new Client(HostName, Port);
        var files = await client.List(SomeTextFile);
        Assert.That(files, Is.Null);
    }

    /// <summary>
    /// Tests the multiple listing from same client.
    /// </summary>
    [Test]
    [Timeout(60000)]
    public static void TestList_MultipleListsFromSameClient()
    {
        int numberOfTasks = 10;
        var tasks = new Task<(string, bool)[]?>[numberOfTasks];
        var client = new Client(HostName, Port);
        for (int i = 0; i < numberOfTasks; ++i)
        {
            tasks[i] = client.List(TestFiles);
        }

        Thread.Sleep(10000 / Environment.ProcessorCount);
        AssertThatListTasksAreCompleted(tasks);
    }

    /// <summary>
    /// Tests the listing from different clients.
    /// </summary>
    [Test]
    [Timeout(60000)]
    public static void TestList_ListFromDifferentClients()
    {
        int numberOfClients = 8;
        var tasks = new Task<(string, bool)[]?>[numberOfClients];
        for (int i = 0; i < numberOfClients; ++i)
        {
            var client = new Client(HostName, Port);
            tasks[i] = client.List(TestFiles);
        }

        Thread.Sleep(10000 / Environment.ProcessorCount);
        AssertThatListTasksAreCompleted(tasks);
    }

    /// <summary>
    /// Tests the text file downloading.
    /// </summary>
    /// <returns>The task representing the test.</returns>
    [Test]
    [Timeout(60000)]
    public static async Task TestGet_GetSomeTextFile()
    {
        var client = new Client(HostName, Port);
        var content = await client.Get(SomeTextFile);
        var actualContent = await File.ReadAllBytesAsync(SomeTextFile);
        Assert.That(content, Is.EqualTo(actualContent));
    }

    /// <summary>
    /// Tests the image downloading.
    /// </summary>
    /// <returns>The task representing the test.</returns>
    [Test]
    [Timeout(60000)]
    public static async Task TestGet_GetSomeImage()
    {
        var client = new Client(HostName, Port);
        var content = await client.Get(SomeImage);
        var actualContent = await File.ReadAllBytesAsync(SomeImage);
        Assert.That(content, Is.EqualTo(actualContent));
    }

    /// <summary>
    /// Tests the client trying to download non existing file.
    /// </summary>
    /// <returns>The task representing the test.</returns>
    [Test]
    [Timeout(60000)]
    public static async Task TestGet_TryToGetNonExistentFile_ReturnNull()
    {
        var client = new Client(HostName, Port);
        var content = await client.Get("asfgasggsd");
        Assert.That(content, Is.Null);
    }

    /// <summary>
    /// Tests the client trying to download a directory.
    /// </summary>
    /// <returns>The task representing the test.</returns>
    [Test]
    [Timeout(60000)]
    public static async Task TestGet_TryToGetDirectory_ReturnNull()
    {
        var client = new Client(HostName, Port);
        var content = await client.Get(SomeDirectory);
        Assert.That(content, Is.Null);
    }

    /// <summary>
    /// Tests the multiple downloading from the same client.
    /// </summary>
    /// <returns>The task representing the test.</returns>
    [Test]
    [Timeout(60000)]
    public static async Task TestGet_MultipleGetsFromSameClient()
    {
        int numberOfTasks = 4;
        var tasks = new Task<byte[]?>[numberOfTasks];
        var client = new Client(HostName, Port);
        for (int i = 0; i < numberOfTasks; ++i)
        {
            tasks[i] = client.Get(SomeImage);
        }

        Thread.Sleep(30000 / Environment.ProcessorCount);
        await AssertThatGetTasksAreCompleted(tasks);
    }

    /// <summary>
    /// Tests the downloading from different clients.
    /// </summary>
    /// <returns>The task representing the test.</returns>
    [Test]
    [Timeout(60000)]
    public static async Task TestGet_GetFromDifferentClients()
    {
        int numberOfClients = 3;
        var tasks = new Task<byte[]?>[numberOfClients];
        for (int i = 0; i < numberOfClients; ++i)
        {
            var client = new Client(HostName, Port);
            tasks[i] = client.Get(SomeImage);
        }

        Thread.Sleep(30000 / Environment.ProcessorCount);
        await AssertThatGetTasksAreCompleted(tasks);
    }

    private static void AssertThatListTasksAreCompleted(
        Task<(string, bool)[]?>[] tasks)
    {
        var expectedResult = new (string, bool)[]
        {
            (SomeDirectory, true),
            (SomeTextFile, false),
        };

        foreach (var task in tasks)
        {
            Assert.That(task.IsCompleted, Is.True);
        }

        foreach (var task in tasks)
        {
            Assert.That(task.Result, Is.EquivalentTo(expectedResult));
        }
    }

    private static async Task AssertThatGetTasksAreCompleted(
        Task<byte[]?>[] tasks)
    {
        var expectedResult = await File.ReadAllBytesAsync(SomeImage);
        foreach (var task in tasks)
        {
            Assert.That(task.IsCompleted, Is.True);
        }

        foreach (var task in tasks)
        {
            Assert.That(task.Result, Is.EqualTo(expectedResult));
        }
    }
}
