// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

// <auto-generated/>

using MyNUnit;

// 1 passing, 1 failing, 2 ignored
public class TestClass6_2
{
    public static bool start = false;

    [BeforeClass]
    public static void BeforeClass() => start = true;

    [Test]
    [Ignore("")]
    public void Test1_Ignored() => throw new NotImplementedException();

    [Test]
    public static void Test2_Passing() => MyAssert.That(start);

    [Test]
    [Expected(typeof(InvalidDataException))]
    public static void Test3_Failing() => MyAssert.That(false);

    [Test]
    [Ignore("")]
    public void Test4_Ignored() => _ = 0;
}