// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MatrixMultiplication.Tests;

/// <summary>
/// Tests for the matrix representation.
/// </summary>
public static class MatrixTests
{
    private const string ConstructorTestsCorrectCasesPath = "TestSamples/MatrixTests/Correct";

    private const string ConstructorTestsIncorrectCasesPath = "TestSamples/MatrixTests/Incorrect";

    private const string TempDirectoryPath = "TestSamples/tmp";

    private static List<int[,]> testArrays =
    [
        new int[1, 1] { { 1 } },
        new int[3, 2]
        {
            { 5, 99 },
            { -100, 0 },
            { 32, 66 },
        },
        new int[1, 6] { { 100, 200, 300, -99, 50, 6 } },
        new int[8, 6]
        {
            { 1, 0, 0, 0, 0, 0 },
            { 0, 1, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 0, 0 },
            { 0, 0, 0, 1, 0, 0 },
            { 0, 0, 0, 0, 1, 0 },
            { 0, 0, 0, 0, 0, 1 },
            { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 },
        },
    ];

    /// <summary>
    /// Tests the creation of the matrix from the file.
    /// </summary>
    /// <param name="sourcePath">Path of the file to read the matrix from.</param>
    /// <param name="expectedElements">The expected elements of the matrix.</param>
    [TestCaseSource(nameof(ConstructorTests_CorrectCases))]
    public static void ConstructorTest_CorrectCases_MatrixIsReadCorrectly(
        string sourcePath, int[,] expectedElements)
    {
        var matrix = new Matrix(sourcePath);
        var expectedMatrix = new Matrix(expectedElements);
        Assert.That(matrix, Is.EqualTo(expectedMatrix));
    }

    /// <summary>
    /// Tests the creation of matrix from the file with incorrect format.
    /// </summary>
    /// <param name="sourcePath">Path of the file to read matrix from.</param>
    [TestCaseSource(nameof(ConstructorTests_IncorrectCases))]
    public static void ConstructorTest_IncorrectCases_ThrowException(string sourcePath)
        => Assert.Throws<InvalidDataException>(() => new Matrix(sourcePath));

    /// <summary>
    /// Tests the writing of matrix to the given path.
    /// </summary>
    /// <param name="elements">Elements to create a matrix with.</param>
    /// <param name="destination">Path of the file to write the matrix to.</param>
    [TestCaseSource(nameof(WriteMatrixTests_Cases))]
    public static void WriteMatrixTest_WriteMatrixToFile_MatrixIsUnchangedAfterReading(
        int[,] elements, string destination)
    {
        var matrix = new Matrix(elements);
        matrix.Write(destination);
        var writtenMatrix = new Matrix(destination);
        Assert.That(matrix, Is.EqualTo(writtenMatrix));
    }

    private static IEnumerable<TestCaseData> ConstructorTests_CorrectCases()
    {
        int i = 0;
        var files = Directory.GetFiles(ConstructorTestsCorrectCasesPath);
        Array.Sort(files);
        foreach (var file in files)
        {
            yield return new TestCaseData(file, testArrays[i++]);
        }
    }

    private static IEnumerable<TestCaseData> ConstructorTests_IncorrectCases()
        => Directory.GetFiles(ConstructorTestsIncorrectCasesPath).Select(file => new TestCaseData(file));

    private static IEnumerable<TestCaseData> WriteMatrixTests_Cases()
    {
        Directory.CreateDirectory(TempDirectoryPath);
        for (int i = 0; i < testArrays.Count; ++i)
        {
            var path = $"{TempDirectoryPath}/0{i + 1}.txt";
            yield return new TestCaseData(testArrays[i], path);
        }
    }
}
