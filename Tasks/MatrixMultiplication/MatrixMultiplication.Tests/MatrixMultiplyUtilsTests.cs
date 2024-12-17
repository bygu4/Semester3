// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplication.Tests;

/// <summary>
/// Tests for the matrix multiplication.
/// </summary>
public static class MatrixMultiplyUtilsTests
{
    private const int NumberOfTestRuns = 8;

    private const string SmallDataCorrectCasesPath = "TestSamples/MultiplicationTests/Correct";

    private const string SmallDataIncorrectCasesPath = "TestSamples/MultiplicationTests/Incorrect";

    private static (int, int)[] bigDataLeftDimensions =
    [
        (5, 100), // 25 <- product matrix length
        (10, 10), // 100
        (20, 1000), // 600
        (100, 100), // 3000
        (100, 1000), // 20000
        (2000, 50), // 100000
        (100, 500), // 400000
        (1000, 100), // 1000000
    ];

    private static (int, int)[] bigDataRightDimensions =
    [
        (100, 5),
        (10, 10),
        (1000, 30),
        (100, 30),
        (1000, 200),
        (50, 50),
        (500, 4000),
        (100, 1000),
    ];

    private static float[] bigDataMatrixCoefficients =
        [0, 0, 0.25f, 0.25f, 0.5f, 0.5f, 0.75f, 0.75f];

    /// <summary>
    /// Tests the multiplication of small matrices.
    /// </summary>
    /// <param name="left">The matrix to multiply.</param>
    /// <param name="right">The matrix to multiply by.</param>
    /// <param name="expectedProduct">The expected product matrix.</param>
    [TestCaseSource(nameof(SmallData_CorrectCases))]
    public static void MatrixMultiplicationTest_SmallData_GetExpectedMatrices(
        Matrix left, Matrix right, Matrix expectedProduct)
    {
        var sequentialProduct = MatrixMultiplyUtils.MultiplySequentially(left, right);
        var concurrentProduct = MatrixMultiplyUtils.MultiplyConcurrently(left, right);
        Assert.That(sequentialProduct, Is.EqualTo(expectedProduct));
        Assert.That(concurrentProduct, Is.EqualTo(expectedProduct));
    }

    /// <summary>
    /// Tests the multiplication of matrices with incorrect dimensions.
    /// </summary>
    /// <param name="left">The matrix to multiply.</param>
    /// <param name="right">The matrix to multiply by.</param>
    [TestCaseSource(nameof(SmallData_IncorrectCases))]
    public static void MatrixMultiplicationTest_IncorrectDimensions_ThrowException(
        Matrix left, Matrix right)
    {
        Assert.Throws<InvalidOperationException>(() =>
            MatrixMultiplyUtils.MultiplySequentially(left, right));
        Assert.Throws<InvalidOperationException>(() =>
            MatrixMultiplyUtils.MultiplyConcurrently(left, right));
    }

    /// <summary>
    /// Tests the multiplication of large matrices.
    /// </summary>
    /// <param name="leftDimensions">The dimensions of the left matrix.</param>
    /// <param name="rightDimensions">The dimension of the right matrix.</param>
    /// <param name="expectedCoefficient">The minimal expected time coefficient.</param>
    [TestCaseSource(nameof(BigData_Cases))]
    public static void MatrixMultiplicationTest_BigData_GetEqualMatrices(
        (int, int) leftDimensions,
        (int, int) rightDimensions,
        float expectedCoefficient)
    {
        var testResults = new float[NumberOfTestRuns];
        for (int i = 0; i < NumberOfTestRuns; ++i)
        {
            var (sequentialProduct, concurrentProduct, coefficient) =
                MatrixTimeMeasureHelper.MultiplyAndGetTimeCoefficient(leftDimensions, rightDimensions);
            Assert.That(sequentialProduct, Is.EqualTo(concurrentProduct));
            testResults[i] = coefficient;
        }

        var expectedValue = testResults.Average();
        Assert.That(expectedValue, Is.GreaterThanOrEqualTo(expectedCoefficient));
    }

    private static IEnumerable<TestCaseData> SmallData_CorrectCases()
    {
        foreach (var dir in Directory.GetDirectories(SmallDataCorrectCasesPath))
        {
            var leftMatrix = new Matrix(Path.Join(dir, "LeftMatrix.txt"));
            var rightMatrix = new Matrix(Path.Join(dir, "RightMatrix.txt"));
            var productMatrix = new Matrix(Path.Join(dir, "ProductMatrix.txt"));
            yield return new TestCaseData(leftMatrix, rightMatrix, productMatrix);
        }
    }

    private static IEnumerable<TestCaseData> SmallData_IncorrectCases()
    {
        foreach (var dir in Directory.GetDirectories(SmallDataIncorrectCasesPath))
        {
            var leftMatrix = new Matrix(Path.Join(dir, "LeftMatrix.txt"));
            var rightMatrix = new Matrix(Path.Join(dir, "RightMatrix.txt"));
            yield return new TestCaseData(leftMatrix, rightMatrix);
        }
    }

    private static IEnumerable<TestCaseData> BigData_Cases()
    {
        for (int i = 0; i < bigDataLeftDimensions.Length; ++i)
        {
            var leftDimensions = bigDataLeftDimensions[i];
            var rightDimensions = bigDataRightDimensions[i];
            var expectedCoefficient = bigDataMatrixCoefficients[i] * Environment.ProcessorCount / 2;
            yield return new TestCaseData(leftDimensions, rightDimensions, expectedCoefficient);
        }
    }
}
