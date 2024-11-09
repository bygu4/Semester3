// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using MyNUnit;

// 1 passing, 2 skipping
public class TestClass1
{
    private static int testNumber = 0;

    [Before]
    public void BeforeTest() => ++testNumber;

    [Test]
    [Ignore("for some reason")]
    public void Test1_Skipping() => throw new InvalidDataException();

    [Test]
    [Ignore("again")]
    public void Test2_Skipping() => MyAssert.That(2 > 0);

    [Test]
    public void Test3_Passing() => _ = "pass";

    [AfterClass]
    public static void AfterClass() => MyAssert.That(testNumber == 1);
}
