// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Lazy;

/// <summary>
/// Class representing lazy evaluation.
/// </summary>
/// <typeparam name="T">Type of returned object.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="SingleThreadLazy{T}"/> class.
/// </remarks>
/// <param name="supplier">Function to be evaluated lazily.</param>
public class SingleThreadLazy<T>(Func<T> supplier)
    : ILazy<T>
{
    private Func<T>? supplier = supplier;
    private T? result;
    private Exception? thrownException;

    /// <summary>
    /// Gets output of the function, evaluated lazily.
    /// </summary>
    /// <returns>Output of the function.</returns>
    public T Get()
    {
        this.EvaluateAndSaveResult();
        if (this.thrownException is not null)
        {
            throw this.thrownException;
        }

        ArgumentNullException.ThrowIfNull(this.result, "Returned value was null");
        return this.result;
    }

    private void EvaluateAndSaveResult()
    {
        if (this.supplier is null)
        {
            return;
        }

        try
        {
            this.result = this.supplier();
        }
        catch (Exception e)
        {
            this.thrownException = e;
        }
        finally
        {
            this.supplier = null;
        }
    }
}
