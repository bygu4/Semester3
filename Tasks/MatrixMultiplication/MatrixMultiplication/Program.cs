// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplication;

/// <summary>
/// Program for measuring time of the matrix multiplication.
/// </summary>
public static class Program
{
    private const int NumberOfRuns = 8;

    private static (int, int)[] sampleLeftDimensions =
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

    private static (int, int)[] sampleRightDimensions =
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

    /// <summary>
    /// Measures the time of matrix multiplication with different methods and creates the result table.
    /// </summary>
    public static void Main()
    {
        Directory.CreateDirectory("MeasureResult");
        MatrixTimeMeasureHelper.WriteHeaderOfResultTable(NumberOfRuns);
        for (int i = 0; i < sampleLeftDimensions.Length; ++i)
        {
            var left = sampleLeftDimensions[i];
            var right = sampleRightDimensions[i];
            MeasureTimeForSampleAndWriteResult(i, left, right);
        }
    }

    private static void MeasureTimeForSampleAndWriteResult(
        int sample,
        (int, int) leftDimensions,
        (int, int) rightDimensions)
    {
        var results = new float[NumberOfRuns];
        for (int i = 0; i < NumberOfRuns; ++i)
        {
            (_, _, results[i]) = MatrixTimeMeasureHelper.MultiplyAndGetTimeCoefficient(
                leftDimensions, rightDimensions);
        }

        MatrixTimeMeasureHelper.WriteMeasureResultToTable(sample, results);
    }
}
