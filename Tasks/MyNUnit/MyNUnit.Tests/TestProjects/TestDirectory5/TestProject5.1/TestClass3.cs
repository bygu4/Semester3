// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using MyNUnit;

// 1 passing
public class TestClass3
{
    private int testNumber = 100;

    [Test]
    public void Test1_Passing() => MyAssert.That(testNumber % 2 == 0);
}
