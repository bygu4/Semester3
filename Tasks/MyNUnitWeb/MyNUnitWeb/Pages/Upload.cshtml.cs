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
    private const string TempDirectory = "tmp";
    private const long MaxUploadSize = 512 * 1024 ^ 2;

    /// <summary>
    /// Uploads the files after validation and starts the testing.
    /// </summary>
    /// <param name="testFiles">The given assembly files to test.</param>
    /// <returns>The task representing the upload and test completion.</returns>
    [ValidateAntiForgeryToken]
    public async Task OnPost(IEnumerable<IFormFile> testFiles)
    {
        var uploadSize = testFiles.Sum(file => file.Length);
        if (uploadSize > MaxUploadSize)
        {
            this.Redirect("Error");
        }

        Directory.CreateDirectory(TempDirectory);
        await UploadFiles(testFiles);
        var testResult = await MyNUnitCore.RunTestsFromEachAssembly(TempDirectory);

        // add to db and redirect
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
        var fileName = file.FileName;
        if (Path.GetExtension(fileName) != ".dll")
        {
            return;
        }

        var destination = Path.Join(TempDirectory, fileName);
        using var stream = new FileStream(destination, FileMode.Create);
        await file.CopyToAsync(stream);
    }

    private static void DeleteTempDirectory()
    {
        var directoryInfo = new DirectoryInfo(TempDirectory);
        directoryInfo.Delete(true);
    }
}
