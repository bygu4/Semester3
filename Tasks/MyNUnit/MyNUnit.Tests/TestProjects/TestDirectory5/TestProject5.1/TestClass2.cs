// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using MyNUnit;

// 1 passing, 2 failing
public static class TestClass2
{
    [Test]
    [Expected(typeof(IndexOutOfRangeException))]
    public static void Test1_Passing()
    {
        var array = new int[100];
        var _ = array[200];
    }

    [Test]
    [Expected(typeof(FileNotFoundException))]
    public static void Test2_Failing() => throw new ArgumentNullException();

    [Test]
    [Expected(typeof(SystemException))]
    public static void Test3_Failing() => MyAssert.That(true);
}
