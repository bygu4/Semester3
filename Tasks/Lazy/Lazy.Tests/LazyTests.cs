// Copyright (c) 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Lazy.Tests;

public static class LazyTests
{
    private static Delegate[] regularTestMethods =
        [RegularTestMethod1, RegularTestMethod2, RegularTestMethod3];

    private static Delegate[] throwExceptionTestMethods =
        [ThrowExceptionTestMethod1, ThrowExceptionTestMethod2];

    private static Delegate[] nullReturnTestMethods =
        [NullReturnTestMethod1, NullReturnTestMethod2];

    private static object[] expectedValues =
        [14, "HELL0 W0RLD!", new float[] {0, 0.5f, 2, 4.5f, 8, 12.5f}];

    private static Type[] expectedExceptions =
        [typeof(InvalidOperationException), typeof(FileNotFoundException)];

    private static int[] getCounts = [1, 100, 10000];
    private static int[] threadCounts = [2, 8, 64];
    private static int numberOfEvaluations;

    private static IEnumerable<TestCaseData> OneThread_RegularCases()
    {
        for (int i = 0; i < regularTestMethods.Length; ++i)
        {
            var testMethod = regularTestMethods[i];
            var expectedValue = expectedValues[i];
            foreach (var count in getCounts)
            {
                yield return new TestCaseData(testMethod, expectedValue, count);
            }
        }
    }

    private static IEnumerable<TestCaseData> OneThread_ThrowExceptionCases()
    {
        for (int i = 0; i < throwExceptionTestMethods.Length; ++i)
        {
            var testMethod = throwExceptionTestMethods[i];
            var expectedException = expectedExceptions[i];
            foreach (var count in getCounts)
            {
                yield return new TestCaseData(testMethod, expectedException, count);
            }
        }
    }

    private static IEnumerable<TestCaseData> OneThread_NullReturnCases()
    {
        foreach (var testMethod in nullReturnTestMethods)
        {
            foreach (var count in getCounts)
            {
                yield return new TestCaseData(testMethod, count);
            }
        }
    }

    [TestCaseSource(nameof(OneThread_RegularCases))]
    public static void OneThreadTest_RegularCases<T>(
        Func<T> testMethod, T expectedValue, int numberOfGets)
        => OneThreadTest_Base(testMethod, testObject =>
        {
            for (int i = 0; i < numberOfGets; ++i)
            {
                var value = testObject.Get();
                Assert.That(value, Is.EqualTo(expectedValue));
            }

            Assert.That(numberOfEvaluations, Is.EqualTo(1));
        });

    [TestCaseSource(nameof(OneThread_ThrowExceptionCases))]
    public static void OneThreadTest_ThrowExceptionCases<T>(
        Func<T> testMethod, Type expectedException, int numberOfGets)
        => OneThreadTest_Base(testMethod, testObject =>
        {
            for (int i = 0; i < numberOfGets; ++i)
            {
                Assert.Throws(expectedException, () => testObject.Get());
            }

            Assert.That(numberOfEvaluations, Is.EqualTo(numberOfGets));
        });

    [TestCaseSource(nameof(OneThread_NullReturnCases))]
    public static void OneThreadTest_NullReturnCases<T>(
        Func<T> testMethod, int numberOfGets)
        => OneThreadTest_Base(testMethod, testObject =>
        {
            for (int i = 0; i < numberOfGets; ++i)
            {
                Assert.Throws<ArgumentNullException>(() => testObject.Get());
            }

            Assert.That(numberOfEvaluations, Is.EqualTo(1));
        });

    private static void OneThreadTest_Base<T>(
        Func<T> testMethod, Action<ILazy<T>> testAction)
    {
        ILazy<T>[] testObjects =
            [new SingleThreadLazy<T>(testMethod), new ThreadSafeLazy<T>(testMethod)];
        foreach (var testObject in testObjects)
        {
            numberOfEvaluations = 0;
            testAction(testObject);
        }
    }

    #pragma warning disable CS8603 // Possible null reference return.

    private static T BaseTestMethod<T>(Func<T> methodToEvaluate)
    {
        ++numberOfEvaluations;
        return methodToEvaluate();
    }

    private static int RegularTestMethod1()
        => BaseTestMethod(() => 89 * 33 / (315 - 110));

    private static string RegularTestMethod2()
        => BaseTestMethod(() => "Hello world!".ToUpper().Replace('O', '0'));

    private static float[] RegularTestMethod3()
        => BaseTestMethod(() =>
        {
            int length = 6;
            var result = new float[length];
            for (int i = 0; i < length; ++i)
            {
                result[i] = (float)i * i / 2;
            }
            
            return result;
        });
    
    private static (int, float) ThrowExceptionTestMethod1()
        => BaseTestMethod<(int, float)>(() => throw new InvalidOperationException());

    private static Stream ThrowExceptionTestMethod2()
        => BaseTestMethod<Stream>(() => throw new FileNotFoundException());
    
    private static Dictionary<string, int> NullReturnTestMethod1()
        => BaseTestMethod<Dictionary<string, int>>(() => null);

    private static double? NullReturnTestMethod2()
        => BaseTestMethod<double?>(() => null);

    #pragma warning restore CS8603 // Possible null reference return.
}
