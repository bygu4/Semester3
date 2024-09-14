// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Matrix.Tests;

public static class MatrixMultiplierTests
{
    private static string SmallData_CorrectCases_Path = "TestData/MultiplicationTests/Correct";
    private static string SmallData_IncorrectCases_Path = "TestData/MultiplicationTests/Incorrect";

    private static IEnumerable<TestCaseData> SmallData_CorrectCases()
    {
        foreach (var dir in Directory.GetDirectories(SmallData_CorrectCases_Path))
        {
            var leftMatrix = new Matrix(Path.Join(dir, "LeftMatrix.txt"));
            var rightMatrix = new Matrix(Path.Join(dir, "RightMatrix.txt"));
            var productMatrix = new Matrix(Path.Join(dir, "ProductMatrix.txt"));
            yield return new TestCaseData(leftMatrix, rightMatrix, productMatrix);
        }
    }

    private static IEnumerable<TestCaseData> SmallData_IncorrectCases()
    {
        foreach (var dir in Directory.GetDirectories(SmallData_IncorrectCases_Path))
        {
            var leftMatrix = new Matrix(Path.Join(dir, "LeftMatrix.txt"));
            var rightMatrix = new Matrix(Path.Join(dir, "RightMatrix.txt"));
            yield return new TestCaseData(leftMatrix, rightMatrix);
        }
    }

    [TestCaseSource(nameof(SmallData_CorrectCases))]
    public static void MatrixMultiplierTest_SmallData_CorrectCases(
        Matrix left, Matrix right, Matrix expectedProduct)
    {
        var sequentialProduct = MatrixMultiplier.MultiplySequentially(left, right);
        var concurrentProduct = MatrixMultiplier.MultiplyConcurrently(left, right);
        Assert.That(sequentialProduct, Is.EqualTo(expectedProduct));
        Assert.That(concurrentProduct, Is.EqualTo(expectedProduct));
    }

    [TestCaseSource(nameof(SmallData_IncorrectCases))]
    public static void MatrixMultiplierTest_SmallData_IncorrectCases(
        Matrix left, Matrix right)
    {
        Assert.Throws<InvalidOperationException>(() =>
            MatrixMultiplier.MultiplySequentially(left, right));
        Assert.Throws<InvalidOperationException>(() =>
            MatrixMultiplier.MultiplyConcurrently(left, right));
    }
}
