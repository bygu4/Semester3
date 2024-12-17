// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplication;

using System.Diagnostics;

/// <summary>
/// Utility for measuring time of the matrix multiplication.
/// </summary>
public static class MatrixTimeMeasureHelper
{
    private const string ResultDirectoryPath = "MeasureResult/";

    private static string resultTablePath = Path.Join(ResultDirectoryPath, $"{DateTime.Now.Ticks}.txt");

    private static Random random = new (DateTime.Now.Millisecond);

    /// <summary>
    /// Generates a matrix with the given dimensions.
    /// </summary>
    /// <param name="dimensions">The dimensions of the matrix to generate.</param>
    /// <returns>The generated matrix.</returns>
    public static Matrix GenerateMatrix((int, int) dimensions)
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

    /// <summary>
    /// Multiplies matrices with the given method and measures the elapsed time.
    /// </summary>
    /// <param name="left">The matrix to multiply.</param>
    /// <param name="right">The matrix to multiply by.</param>
    /// <param name="multiplicationMethod">The multiplication method to use.</param>
    /// <returns>The pair of matrix product and elapsed time.</returns>
    public static (Matrix, long) MultiplyAndCountTime(
        Matrix left, Matrix right, Func<Matrix, Matrix, Matrix> multiplicationMethod)
    {
        var stopwatch = new Stopwatch();

        stopwatch.Restart();
        var product = multiplicationMethod(left, right);
        var elapsedTime = stopwatch.ElapsedMilliseconds;
        stopwatch.Stop();

        return (product, elapsedTime);
    }

    /// <summary>
    /// Generates matrices, multiplies using different methods and gets the resulting time coefficient.
    /// </summary>
    /// <param name="leftDimensions">Dimensions of the left matrix.</param>
    /// <param name="rightDimensions">Dimensions of the right matrix.</param>
    /// <returns>A tuple of matrix products and the time coefficient.</returns>
    public static (Matrix, Matrix, float) MultiplyAndGetTimeCoefficient(
        (int, int) leftDimensions,
        (int, int) rightDimensions)
    {
        var left = GenerateMatrix(leftDimensions);
        var right = GenerateMatrix(rightDimensions);

        var (sequentialProduct, sequentialTime) = MultiplyAndCountTime(
            left, right, MatrixMultiplyUtils.MultiplySequentially);
        var (concurrentProduct, concurrentTime) = MultiplyAndCountTime(
            left, right, MatrixMultiplyUtils.MultiplyConcurrently);
        var coefficient = (float)sequentialTime / concurrentTime;
        coefficient = float.IsNaN(coefficient) ? 0 : coefficient;

        return (sequentialProduct, concurrentProduct, coefficient);
    }

    /// <summary>
    /// Writes the header of measure result table.
    /// </summary>
    /// <param name="numberOfRuns">The number of runs in the measure.</param>
    public static void WriteHeaderOfResultTable(int numberOfRuns)
    {
        using var writer = new StreamWriter(resultTablePath);
        writer.Write("sample |");
        for (int i = 0; i < numberOfRuns; ++i)
        {
            writer.Write($" run{i + 1}");
        }

        writer.Write(" | expectedValue standardDeviation\n");
    }

    /// <summary>
    /// Writes the result of the measure to the table.
    /// </summary>
    /// <param name="sample">The number of the sample to multiply.</param>
    /// <param name="results">The results of the multiplication.</param>
    public static void WriteMeasureResultToTable(
        int sample, float[] results)
    {
        using var writer = new StreamWriter(resultTablePath, true);
        writer.Write($"{sample} |");
        for (var i = 0; i < results.Length; ++i)
        {
            writer.Write($" {results[i]}");
        }

        var expectedValue = GetExpectedValue(results);
        var standardDeviation = GetStandardDeviation(results);
        writer.Write($" | {expectedValue} {standardDeviation}\n");
    }

    /// <summary>
    /// Gets the expected value of the measure results.
    /// </summary>
    /// <param name="measureResults">The results of the measure.</param>
    /// <returns>The calculated expected value.</returns>
    public static float GetExpectedValue(float[] measureResults)
        => measureResults.Average();

    /// <summary>
    /// Gets the standard deviation of the measure results.
    /// </summary>
    /// <param name="measureResults">The results of the measure.</param>
    /// <returns>The calculated standard deviation.</returns>
    public static float GetStandardDeviation(float[] measureResults)
    {
        var expectedValue = GetExpectedValue(measureResults);
        return (float)Math.Sqrt(
            measureResults.Average(x => Math.Pow(x - expectedValue, 2)));
    }
}
