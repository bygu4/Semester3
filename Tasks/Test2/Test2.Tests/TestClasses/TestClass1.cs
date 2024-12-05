namespace Test2.Tests;

public class TestClass1
{
    public int testField1 = 0;
    private static string testField2 = "a"; 

    public int TestMethod1(float attribute1) => 1;
    public static float TestMethod2() => 9090;
    private string TestMethod3(string attribute1, int attribute2)
    {
        return "Hello";
    }

    private class NestedClass
    {
        public float testField1 = 321;
        private string testField2 = "abc";

        public double TestMethod1()
        {
            throw new InvalidDataException();
        } 
    }
}