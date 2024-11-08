// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using MyNUnit.Core;

if (args.Length != 1)
{
    Console.WriteLine("Invalid arguments.");
    Console.WriteLine("Expected: path to look for assemblies at.");
}

var path = args[0];
var passed = await MyNUnitCore.RunTestsFromEachAssembly(path);
return passed ? 0 : 1;
