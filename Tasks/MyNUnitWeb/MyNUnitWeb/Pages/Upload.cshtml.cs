// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnitWeb.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyNUnit.Core;

/// <summary>
/// Page model for the assembly upload.
/// </summary>
public class UploadModel : PageModel
{
    /// <summary>
    /// The extension of the assembly files to test.
    /// </summary>
    public const string AssemblyExtension = ".dll";

    /// <summary>
    /// The max accepted size of the upload.
    /// </summary>
    public const long MaxUploadSize = 20;

    private const string TempDirectory = "tmp";

    /// <summary>
    /// Uploads the files after validation and starts the testing.
    /// </summary>
    /// <param name="testFiles">The given assembly files to test.</param>
    /// <returns>The task representing the upload and test completion.</returns>
    [ValidateAntiForgeryToken]
    public async Task OnPost(IEnumerable<IFormFile> testFiles)
    {
        this.ValidateFiles(testFiles);
        Directory.CreateDirectory(TempDirectory);
        await UploadFiles(testFiles);

        var testResult = await MyNUnitCore.RunTestsFromEachAssembly(TempDirectory);

        // add to db and redirect
        DeleteTempDirectory();
    }

    private static async Task UploadFiles(IEnumerable<IFormFile> files)
    {
        foreach (var file in files)
        {
            await UploadFile(file);
        }
    }

    private static async Task UploadFile(IFormFile file)
    {
        var destination = Path.Join(TempDirectory, file.FileName);
        using var stream = new FileStream(destination, FileMode.Create);
        await file.CopyToAsync(stream);
    }

    private static void DeleteTempDirectory()
    {
        var directoryInfo = new DirectoryInfo(TempDirectory);
        directoryInfo.Delete(true);
    }

    private void ValidateFiles(IEnumerable<IFormFile> files)
    {
        foreach (var file in files)
        {
            InvalidFileExtensionException.ThrowIfExtensionIsNot(AssemblyExtension, file);
        }

        UploadTooLargeException.ThrowIfTooLarge(MaxUploadSize, files);
    }

    /// <summary>
    /// An exception thrown in case of unexpected file extension.
    /// </summary>
    /// <param name="expectedExtension">The expected file extension.</param>
    /// <param name="actualExtension">The actual received file extension.</param>
    public sealed class InvalidFileExtensionException(string expectedExtension, string actualExtension)
        : Exception($"Expected file extension {expectedExtension}, but got {actualExtension}")
    {
        /// <summary>
        /// Throw this exception if the extension of given file does not match.
        /// </summary>
        /// <param name="expectedExtension">The expected file extension.</param>
        /// <param name="file">File to check extension of.</param>
        /// <exception cref="InvalidFileExtensionException">The exception to throw.</exception>
        public static void ThrowIfExtensionIsNot(string expectedExtension, IFormFile file)
        {
            var actualExtension = Path.GetExtension(file.FileName);
            if (actualExtension != ".dll")
            {
                throw new InvalidFileExtensionException(expectedExtension, actualExtension);
            }
        }
    }

    /// <summary>
    /// An exception thrown if the the received upload is too large.
    /// </summary>
    /// <param name="maxSize">The max accepted upload size.</param>
    /// <param name="actualSize">The actual received upload size.</param>
    public sealed class UploadTooLargeException(long maxSize, long actualSize)
        : Exception($"The max upload size was {maxSize}, but got {actualSize}")
    {
        /// <summary>
        /// Throw this exception if the given upload is too large.
        /// </summary>
        /// <param name="maxSize">The max accepted upload size.</param>
        /// <param name="files">The files to check size of.</param>
        /// <exception cref="UploadTooLargeException">The exception to throw.</exception>
        public static void ThrowIfTooLarge(long maxSize, IEnumerable<IFormFile> files)
        {
            var actualSize = files.Sum(file => file.Length);
            if (actualSize > maxSize)
            {
                throw new UploadTooLargeException(maxSize, actualSize);
            }
        }
    }
}
