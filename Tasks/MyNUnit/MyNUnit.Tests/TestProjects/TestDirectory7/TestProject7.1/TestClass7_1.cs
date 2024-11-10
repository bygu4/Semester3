// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using MyNUnit;

// 2 passing, 1 failing, 1 ignored
public class TestClass7_1
{
    public int testNumber = 0;

    [Before]
    private void BeforeTest_Invisible() => ++testNumber;

    [Test]
    [Expected(typeof(InvalidOperationException))]
    public static int Test1_Failing() => 2 + 2;

    [Test]
    private bool Test2_Invisible() => 0 < 1;

    [Test]
    [Ignore("asfafa")]
    private static float Test3_Invisible() => 1.2345f;

    [Test]
    public double Test4_Passing()
    {
        MyAssert.That(this.testNumber == 0);
        return 0;
    }

    public class VisibleClass
    {
        [Test]
        public void Test5_Passing() => MyAssert.That(true);

        [Test]
        [Ignore("asfasf")]
        public static bool Test6_Ignored() => false;
    }

    private class InvisibleClass
    {
        [Test]
        public static void Test7_Invisible() => throw new ArgumentException();

        [Test]
        public int Test8_Invisible() => 1;
    }
}
