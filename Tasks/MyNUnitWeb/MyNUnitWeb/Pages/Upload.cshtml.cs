// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace MyNUnitWeb.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyNUnit.Core;
using MyNUnitWeb.Data;

/// <summary>
/// Page model for the assembly upload.
/// </summary>
/// <param name="dbContext">The database context to use.</param>
[ValidateAntiForgeryToken]
public class UploadModel(TestRunDbContext dbContext)
    : PageModel
{
    /// <summary>
    /// The extension of the assembly files to test.
    /// </summary>
    public const string AssemblyExtension = ".dll";

    /// <summary>
    /// The max accepted size of the upload in bytes.
    /// </summary>
    public const long MaxUploadSizeInBytes = 128 * 1024 * 1024;

    private const string TempDirectoryParent = "./";

    /// <summary>
    /// Uploads the files after validation and starts the testing.
    /// </summary>
    /// <param name="testFiles">The given assembly files to test.</param>
    /// <returns>The task representing the upload and test completion.</returns>
    public async Task<RedirectResult> OnPostAsync(IEnumerable<IFormFile> testFiles)
    {
        ValidateFiles(testFiles);
        var tempDirectoryPath = CreateTempDirectory();
        await UploadFiles(testFiles, tempDirectoryPath);

        var testRun = new TestRun();
        testRun.TimeOfRun = DateTime.Now;
        testRun.Summary = await MyNUnitCore.RunTestsFromEachAssembly(tempDirectoryPath);

        await dbContext.AddAsync(testRun);
        await dbContext.SaveChangesAsync();

        DeleteTempDirectory(tempDirectoryPath);
        return this.Redirect($"/TestResult?id={testRun.TestRunId}");
    }

    private static void ValidateFiles(IEnumerable<IFormFile> files)
    {
        foreach (var file in files)
        {
            InvalidFileExtensionException.ThrowIfExtensionIsNot(AssemblyExtension, file);
        }

        UploadTooLargeException.ThrowIfTooLarge(MaxUploadSizeInBytes, files);
    }

    private static async Task UploadFiles(IEnumerable<IFormFile> files, string tempDirectoryPath)
    {
        int i = 0;
        foreach (var file in files)
        {
            var destinationName = $"{i}{AssemblyExtension}";
            var destinationPath = Path.Join(tempDirectoryPath, destinationName);
            using var stream = new FileStream(destinationPath, FileMode.Create);
            await file.CopyToAsync(stream);
            ++i;
        }
    }

    private static string CreateTempDirectory()
    {
        var random = new Random((int)DateTime.Now.Ticks);
        var tempDirectoryName = random.Next().ToString();
        var tempDirectoryPath = Path.Join(TempDirectoryParent, tempDirectoryName);
        Directory.CreateDirectory(tempDirectoryPath);
        return tempDirectoryPath;
    }

    private static void DeleteTempDirectory(string tempDirectoryPath)
    {
        var directoryInfo = new DirectoryInfo(tempDirectoryPath);
        directoryInfo.Delete(true);
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
            if (actualExtension != expectedExtension)
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
