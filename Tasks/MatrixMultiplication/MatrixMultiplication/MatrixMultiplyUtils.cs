// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Matrix;

/// <summary>
/// Class for multiplying matrices.
/// </summary>
public static class MatrixMultiplyUtils
{
    /// <summary>
    /// Multiply two matrices using one thread.
    /// </summary>
    /// <param name="matrix1">Matrix to multiply.</param>
    /// <param name="matrix2">Matrix to multiply by.</param>
    /// <returns>The product matrix.</returns>
    /// <exception cref="InvalidOperationException">
    /// Number of columns in matrix1 must be equal to number of rows in matrix2.</exception>
    public static Matrix MultiplySequentially(Matrix matrix1, Matrix matrix2)
    {
        if (matrix1.NumberOfColumns != matrix2.NumberOfRows)
        {
            throw new InvalidOperationException("Incorrect matrix dimensions");
        }

        var product = new int[matrix1.NumberOfRows, matrix2.NumberOfColumns];
        for (int i = 0; i < matrix1.NumberOfRows; ++i)
        {
            for (int j = 0; j < matrix2.NumberOfColumns; ++j)
            {
                product[i, j] = GetElementOfProductMatrix(matrix1, matrix2, i, j);
            }
        }

        return new Matrix(product);
    }

    /// <summary>
    /// Multiply two matrices using multiple threads.
    /// </summary>
    /// <param name="matrix1">Matrix to multiply.</param>
    /// <param name="matrix2">Matrix to multiply by.</param>
    /// <returns>The product matrix.</returns>
    /// <exception cref="InvalidOperationException">
    /// Number of columns in matrix1 must be equal to number of rows in matrix2.</exception>
    public static Matrix MultiplyConcurrently(Matrix matrix1, Matrix matrix2)
    {
        if (matrix1.NumberOfColumns != matrix2.NumberOfRows)
        {
            throw new InvalidOperationException("Incorrect matrix dimensions");
        }

        var product = new int[matrix1.NumberOfRows, matrix2.NumberOfColumns];
        var numberOfThreads = int.Min(Environment.ProcessorCount, product.Length);
        var threads = new Thread[numberOfThreads];
        int chunkSize = (int)Math.Ceiling((float)product.Length / numberOfThreads);

        for (int i = 0; i < numberOfThreads; ++i)
        {
            var chunkIndex = i;
            threads[i] = new Thread(() =>
                CalculateChunk(matrix1, matrix2, product, chunkIndex, chunkSize));
            threads[i].Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        return new Matrix(product);
    }

    private static int GetElementOfProductMatrix(
        Matrix matrix1, Matrix matrix2, int row, int column)
    {
        int acc = 0;
        for (int i = 0; i < matrix1.NumberOfColumns; ++i)
        {
            acc += matrix1.Elements[row, i] * matrix2.Elements[i, column];
        }

        return acc;
    }

    private static void CalculateChunk(
        Matrix matrix1,
        Matrix matrix2,
        int[,] product,
        int chunkIndex,
        int chunkSize)
    {
        for (int i = chunkIndex * chunkSize; i < (chunkIndex + 1) * chunkSize && i < product.Length; ++i)
        {
            int row = i / matrix2.NumberOfColumns;
            int column = i % matrix2.NumberOfColumns;
            product[row, column] = GetElementOfProductMatrix(matrix1, matrix2, row, column);
        }
    }
}
