// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Lazy;

/// <summary>
/// Class representing lazy evaluation.
/// Can be accessed by multiple threads concurrently.
/// </summary>
/// <typeparam name="T">Type of returned object.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="ThreadSafeLazy{T}"/> class.
/// </remarks>
/// <param name="supplier">Function to be evaluated lazily.</param>
public class ThreadSafeLazy<T>(Func<T> supplier)
    : ILazy<T>
{
    private readonly Func<T> supplier = supplier;
    private readonly object lockObject = new ();

    private T? result;
    private Exception? thrownException;
    private volatile bool isEvaluated = false;

    /// <summary>
    /// Gets output of the function, evaluated lazily.
    /// </summary>
    /// <returns>Output of the function.</returns>
    public T Get()
    {
        if (!this.isEvaluated)
        {
            this.EvaluateAndSaveResult();
        }

        if (this.thrownException is not null)
        {
            throw this.thrownException;
        }

        ArgumentNullException.ThrowIfNull(this.result, "Returned value was null");
        return this.result;
    }

    private void EvaluateAndSaveResult()
    {
        lock (this.lockObject)
        {
            if (this.isEvaluated)
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
                this.isEvaluated = true;
            }
        }
    }
}
