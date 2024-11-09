// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using MyNUnit.Core;

Console.Write('\n');
if (args.Length != 1)
{
    Console.WriteLine("Invalid arguments");
    Console.WriteLine("Expected: path to look for assemblies at\n");
    return (int)ReturnCode.InvalidArguments;
}

var path = args[0];
try
{
    var testResult = await MyNUnitCore.RunTestsFromEachAssembly(path, writeToConsole: true);
    if (testResult.NumberOfTests == 0)
    {
        Console.WriteLine("No tests were found");
    }

    Console.Write('\n');
    return testResult.AllTestsPassed ? (int)ReturnCode.AllTestsPassed : (int)ReturnCode.SomeTestsFailed;
}
catch (DirectoryNotFoundException e)
{
    Console.WriteLine(e.Message);
    Console.Write('\n');
    return (int)ReturnCode.DirectoryNotFound;
}
