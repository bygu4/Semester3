// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using MyNUnit;

// 4 failing
public static class testClass1
{
    [Test]
    public static void Test1_Failing() => throw new SystemException();

    [Test]
    public static void Test2_Failing() => MyAssert.That(false);

    [Test]
    public static void Test3_Failing() => MyAssert.That("a" == "b");

    [Test]
    [Expected(typeof(InvalidOperationException))]
    public static void Test4_Failing() => _ = "fsafas";
}
