// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

// <auto-generated/>

using MyNUnit;

// 4 passing
public static class TestClass6_1
{
    [Test]
    public static void Test1_Passing() => _ = 99;

    [Test]
    public static void Test2_Passing() => MyAssert.That(true);

    [Test]
    public static void Test3_Passing()
    {
        var testString = "aaa";
        MyAssert.That(testString.Length == 3);
    }

    [Test]
    public static void Test4_Passing() => MyAssert.That((36 + 64) == 100);
}