// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using MyNUnit;

// 1 passing, 2 failing
public static class TestClass4_1
{
    private static int testNumber = int.MinValue;
    private static string testString = "";

    [BeforeClass]
    public static void BeforeClass()
    {
        testNumber = 0;
        testString = "a";
    }

    [Before]
    public static void BeforeTest() => ++testNumber;

    [Test]
    public static void Test1_Passing()
    {
        MyAssert.That(testNumber > 0);
        MyAssert.That(testString == "a");
    }

    [Test]
    public static void Test2_Failing() => MyAssert.That(false);

    [Test]
    public static void Test3_Failing() => throw new FileNotFoundException();

    [After]
    public static void AfterTest() => testNumber *= 2;

    [AfterClass]
    public static void AfterClass()
    {
        MyAssert.That(testNumber == 14);
        MyAssert.That(testString == "a");
    }
}
