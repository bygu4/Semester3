// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Lazy;

/// <summary>
/// Interface representing lazy evaluation.
/// Inherited class should be defined by function to be evaluated.
/// </summary>
/// <typeparam name="T">Type of the object returned by evaluated function.</typeparam>
public interface ILazy<T>
{
    /// <summary>
    /// Gets output of the function, evaluated lazily.
    /// </summary>
    /// <returns>Output of the function.</returns>
    public T Get();
}
