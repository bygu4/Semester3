// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using MyNUnit;

// 1 passing, 1 failing, 2 skipping
public class TestClass6_2
{
    public static bool start = false;

    [BeforeClass]
    public static void BeforeClass() => start = true;

    [Test]
    [Ignore("")]
    public void Test1_Skipping() => throw new NotImplementedException();

    [Test]
    public void Test2_Passing() => MyAssert.That(start);

    [Test]
    [Expected(typeof(InvalidDataException))]
    public void Test3_Failing() => MyAssert.That(false);

    [Test]
    [Ignore("")]
    public void Test4_Skipping() => _ = 0;
}
