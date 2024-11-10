// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using MyNUnit;

internal class TestClass7_2
{
    [Test]
    public void Test1_Invisible() => throw new InvalidOperationException();

    [Test]
    public static int Test2_Invisible() => 1;

    [Test]
    public float Test3_Invisible() => throw new NotImplementedException();
}
