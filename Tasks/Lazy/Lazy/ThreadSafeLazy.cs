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
public class ThreadSafeLazy<T> : ILazy<T>
{
    private Func<T> supplier;
    private T? value;
    private bool isEvaluated = false;
    private object lockObject = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadSafeLazy{T}"/> class.
    /// </summary>
    /// <param name="supplier">Function to be evalueted lazily.</param>
    public ThreadSafeLazy(Func<T> supplier)
    {
        this.supplier = supplier;
    }

    /// <summary>
    /// Gets output of the function, evaluated lazily.
    /// </summary>
    /// <returns>Output of the function.</returns>
    public T Get()
    {
        if (this.isEvaluated)
        {
            ArgumentNullException.ThrowIfNull(this.value, "Returned value was null");
            return this.value;
        }

        lock (this.lockObject)
        {
            if (this.isEvaluated)
            {
                ArgumentNullException.ThrowIfNull(this.value, "Returned value was null");
                return this.value;
            }

            this.value = this.supplier();
            this.isEvaluated = true;
            return this.Get();
        }
    }
}
