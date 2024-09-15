// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Diagnostics;

namespace Matrix.Tests;

public static class MatrixMultiplierTests
{
    private static string SmallData_CorrectCases_Path =
        "TestData/TestSamples/MultiplicationTests/Correct";

    private static string SmallData_IncorrectCases_Path =
        "TestData/TestSamples/MultiplicationTests/Incorrect";

    private static string TestResultFile_Path =
        $"../../../TestData/TestResults/{DateTime.Now}.txt";

    private static Random random = new (DateTime.Now.Millisecond);
    private static Stopwatch stopwatch = new ();

    private static (int, int)[] BigData_LeftDimensions =
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

    private static (int, int)[] BigData_RightDimensions =
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

    private static float[] BigData_Coefficients =
        [0, 0, 1, 1, 2, 2, 3, 3];
    
    private static int numberOfTestRuns = 10;

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

    private static IEnumerable<TestCaseData> BigData_Cases()
    {
        for (int i = 0; i < BigData_LeftDimensions.Length; ++i)
        {
            var leftDimensions = BigData_LeftDimensions[i];
            var rightDimensions = BigData_RightDimensions[i];
            var expectedCoefficient = BigData_Coefficients[i];
            yield return new TestCaseData(leftDimensions, rightDimensions, expectedCoefficient);
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

    [TestCaseSource(nameof(BigData_Cases))]
    public static void MatrixMultiplierTest_BigData(
        (int, int) leftDimensions, (int, int) rightDimensions, float expectedCoefficient)
    {
        var testResults = new float[numberOfTestRuns];
        for (int i = 0; i < numberOfTestRuns; ++i)
        {
            testResults[i] = BigData_TestRun(leftDimensions, rightDimensions, expectedCoefficient);
        }

        var expectedValue = testResults.Average();
        var standardDeviation = Math.Sqrt(
            testResults.Average(x => Math.Pow(x - expectedValue, 2)));
        
        Assert.That(expectedValue, Is.GreaterThanOrEqualTo(expectedCoefficient));
    }

    private static Matrix GenerateMatrix((int, int) dimensions)
    {
        var (numberOfRows, numberOfColumns) = dimensions;
        var elements = new int[numberOfRows, numberOfColumns];
        for (int i = 0; i < numberOfRows; ++i)
        {
            for (int j = 0; j < numberOfColumns; ++j)
            {
                elements[i, j] = random.Next();
            }
        }

        return new Matrix(elements);
    }

    private static (Matrix, long) MultiplyAndCountTime(
        Matrix left, Matrix right, Func<Matrix, Matrix, Matrix> multiplicationMethod)
    {
        stopwatch.Restart();
        var product = multiplicationMethod(left, right);
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();

        return (product, elapsedTime);
    }

    private static float BigData_TestRun(
        (int, int) leftDimensions, (int, int) rightDimensions, float expectedCoefficient)
    {
        var left = GenerateMatrix(leftDimensions);
        var right = GenerateMatrix(rightDimensions);

        var (sequentialProduct, sequentialTime) = MultiplyAndCountTime(
            left, right, MatrixMultiplier.MultiplySequentially);
        var (concurrentProduct, concurrentTime) = MultiplyAndCountTime(
            left, right, MatrixMultiplier.MultiplyConcurrently);
        var coeffitient = (float)sequentialTime / concurrentTime;

        Assert.That(sequentialProduct, Is.EqualTo(concurrentProduct));
        return coeffitient;
    }
}
