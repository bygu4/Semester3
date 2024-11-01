// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using System.Collections;

namespace Matrix;

/// <summary>
/// Class representing a matrix of integers.
/// </summary>
public record Matrix : IEquatable<Matrix>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class.
    /// </summary>
    /// <param name="elements">Elements to set for the matrix instance.</param>
    public Matrix(int[,] elements)
    {
        this.Elements = elements;
        this.NumberOfRows = elements.GetLength(0);
        this.NumberOfColumns = elements.GetLength(1);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Matrix"/> class.
    /// Reads matrix from given file.
    /// </summary>
    /// <param name="sourceFile">Path of the file to read matrix from.</param>
    /// <exception cref="InvalidDataException">File is in invalid format.</exception>
    public Matrix(string sourceFile)
    {
        var rows = File.ReadAllLines(sourceFile);
        this.NumberOfRows = rows.Length;
        if (this.NumberOfRows == 0)
        {
            throw new InvalidDataException("Invalid format: file was empty");
        }

        var currentRow = rows[0].Split(' ');
        this.NumberOfColumns = currentRow.Length;

        if (this.NumberOfColumns == 0)
        {
            throw new InvalidDataException("Invalid format: empty row");
        }

        this.Elements = new int[this.NumberOfRows, this.NumberOfColumns];
        for (int i = 1; i < rows.Length; ++i)
        {
            this.CopyRowToTheMatrix(currentRow, i - 1);
            currentRow = rows[i].Split(' ');
        }

        this.CopyRowToTheMatrix(currentRow, this.NumberOfRows - 1);
    }

    /// <summary>
    /// Gets an array of elements of the matrix.
    /// </summary>
    public int[,] Elements { get; }

    /// <summary>
    /// Gets number of rows in the matrix.
    /// </summary>
    public int NumberOfRows { get; }

    /// <summary>
    /// Gets number of columns in the matrix.
    /// </summary>
    public int NumberOfColumns { get; }

    /// <summary>
    /// Writes matrix to specified file.
    /// </summary>
    /// <param name="destinationFile">File to write matrix to.</param>
    public void Write(string destinationFile)
    {
        using var writer = new StreamWriter(destinationFile);
        for (int i = 0; i < this.Elements.Length; ++i)
        {
            int currentRow = i / this.NumberOfColumns;
            int currentColumn = i % this.NumberOfColumns;
            writer.Write(this.Elements[currentRow, currentColumn]);
            if (currentColumn == this.NumberOfColumns - 1)
            {
                if (currentRow != this.NumberOfRows - 1)
                {
                    writer.Write('\n');
                }
            }
            else
            {
                writer.Write(' ');
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether this matrix instance is equal to given one.
    /// </summary>
    /// <param name="matrix">Matrix to check equality to.</param>
    /// <returns>Value indicating whether this matrix instance is equal to given one.</returns>
    public virtual bool Equals(Matrix? matrix)
    {
        if (matrix is null || this.NumberOfRows != matrix.NumberOfRows ||
            this.NumberOfColumns != matrix.NumberOfColumns)
        {
            return false;
        }

        for (int i = 0; i < this.NumberOfRows; ++i)
        {
            for (int j = 0; j < this.NumberOfColumns; ++j)
            {
                if (this.Elements[i, j] != matrix.Elements[i, j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the hash code of the Matrix instance.
    /// </summary>
    /// <returns>The hash code as an integer.</returns>
    public override int GetHashCode()
        => ((IStructuralEquatable)this.Elements).GetHashCode(EqualityComparer<int>.Default);

    private void CopyRowToTheMatrix(string[] rowElements, int rowIndex)
    {
        if (rowElements.Length != this.NumberOfColumns)
        {
            throw new InvalidDataException("Invalid format: rows of different length");
        }

        try
        {
            for (int i = 0; i < this.NumberOfColumns; ++i)
            {
                this.Elements[rowIndex, i] = int.Parse(rowElements[i]);
            }
        }
        catch (FormatException e)
        {
            throw new InvalidDataException($"Invalid format: {e.Message}");
        }
    }
}
