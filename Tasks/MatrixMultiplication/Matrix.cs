// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Matrix;

/// <summary>
/// Class representing a matrix of integers.
/// </summary>
public class Matrix
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
        var sourceData = this.ReadFile(sourceFile);
        var rows = sourceData.Split('\n');
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
            this.CopyToMatrix(currentRow, i - 1);
            currentRow = rows[i].Split(' ');
        }

        this.CopyToMatrix(currentRow, this.NumberOfRows - 1);
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
                writer.Write('\n');
            }
            else
            {
                writer.Write(' ');
            }
        }
    }

    private string ReadFile(string path)
    {
        using var reader = new StreamReader(path);
        return reader.ReadToEnd();
    }

    private void CopyToMatrix(string[] elements, int row)
    {
        if (elements.Length != this.NumberOfColumns)
        {
            throw new InvalidDataException("Invalid format: rows of different length");
        }

        for (int i = 0; i < this.NumberOfColumns; ++i)
        {
            this.Elements[row, i] = int.Parse(elements[i]);
        }
    }
}
