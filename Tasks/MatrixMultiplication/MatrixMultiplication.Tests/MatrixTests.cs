// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Matrix.Tests;

public static class MatrixTests
{
    private static string ConstructorTests_CorrectCases_Path = "TestData/MatrixTests/Correct";
    private static string ConstructorTests_IncorrectCases_Path = "TestData/MatrixTests/Incorrect";
    private static string TempDirectory_Path = "tmp";

    private static List<int[,]> TestArrays =
    [
        new int[1, 1] { { 1 } },
        new int[3, 2] { { 5, 99 }, { -100, 0 }, { 32, 66 } },
        new int[1, 6] { { 100, 200, 300, -99, 50, 6 } },
        new int[8, 6] { { 1, 0, 0, 0, 0, 0 },
                        { 0, 1, 0, 0, 0, 0 }, 
                        { 0, 0, 1, 0, 0, 0 },
                        { 0, 0, 0, 1, 0, 0 }, 
                        { 0, 0, 0, 0, 1, 0 },
                        { 0, 0, 0, 0, 0, 1 },
                        { 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0 } },
    ];

    private static IEnumerable<TestCaseData> ConstructorTests_CorrectCases()
    {
        int i = 0;
        var files = Directory.GetFiles(ConstructorTests_CorrectCases_Path);
        Array.Sort(files);
        foreach (var file in files)
        {
            yield return new TestCaseData(file, TestArrays[i++]);
        }
    }

    private static IEnumerable<TestCaseData> ConstructorTests_IncorrectCases()
    {
        foreach (var file in Directory.GetFiles(ConstructorTests_IncorrectCases_Path))
        {
            yield return new TestCaseData(file);
        }
    }

    private static IEnumerable<TestCaseData> WriteMatrixTests_Cases()
    {
        Directory.CreateDirectory(TempDirectory_Path);
        for (int i = 0; i < TestArrays.Count; ++i)
        {
            var path = $"{TempDirectory_Path}/0{i + 1}.txt";
            yield return new TestCaseData(TestArrays[i], path);
        }
    }

    [TestCaseSource(nameof(ConstructorTests_CorrectCases))]
    public static void ConstructorTest_CorrectCases(string sourcePath, int[,] expectedElements)
    {
        var matrix = new Matrix(sourcePath);
        var expectedMatrix = new Matrix(expectedElements);
        Assert.That(matrix, Is.EqualTo(expectedMatrix));
    }

    [TestCaseSource(nameof(ConstructorTests_IncorrectCases))]
    public static void ConstructorTest_IncorrectCases(string sourcePath)
    {
        Assert.Throws<InvalidDataException>(() => new Matrix(sourcePath));
    }

    [TestCaseSource(nameof(WriteMatrixTests_Cases))]
    public static void WriteMatrixTest(int[,] elements, string destination)
    {
        var matrix = new Matrix(elements);
        matrix.Write(destination);
        var writtenMatrix = new Matrix(destination);
        Assert.That(matrix, Is.EqualTo(writtenMatrix));
    }
}
