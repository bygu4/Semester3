namespace Test2;

using System.Reflection;

public static class Reflector
{
    private const string Tab = "    ";

    public static async Task PrintStructure(Type someClass)
    {
        using var writer = new StreamWriter($"{someClass.Name}.cs");
        await PrintStructureInternal(someClass, 0, writer);
    }

    private static async Task PrintStructureInternal(
        Type someClass, int numberOfTabs, StreamWriter writer)
    {
        var indent = GetIndent(numberOfTabs);
        await WriteClassInfo(someClass, numberOfTabs, writer);
        await writer.WriteAsync(indent + '{');
        foreach (var field in someClass.GetFields())
        {
            await PrintField(field, numberOfTabs + 1, writer);
        }

        await writer.WriteAsync('\n');
        foreach (var method in someClass.GetMethods())
        {
            await PrintMethod(method, numberOfTabs + 1, writer);
        }

        await writer.WriteAsync('\n');
        foreach (var nestedClass in someClass.GetNestedTypes())
        {
            await PrintStructureInternal(nestedClass, numberOfTabs + 1, writer);
        }

        await writer.WriteAsync(indent + '}');
    }

    private static async Task WriteClassInfo(
        Type classInfo, int numberOfTabs, StreamWriter writer)
    {
        var indent = GetIndent(numberOfTabs);
        var signature = GetClassSignature(classInfo);
        await writer.WriteLineAsync(indent + signature);
    }

    private static async Task PrintField(
        FieldInfo field, int numberOfTabs, StreamWriter writer)
    {
        var indent = GetIndent(numberOfTabs);
        var signature = GetFieldSignature(field);
        await writer.WriteLineAsync(indent + signature);
    }

    private static async Task PrintMethod(
        MethodInfo method, int numberOfTabs, StreamWriter writer)
    {
        var indent = GetIndent(numberOfTabs);
        var signature = GetMethodSignature(method);
        await writer.WriteLineAsync(indent + signature);
    }

    private static string GetClassSignature(Type classInfo)
    {
        var signature = "";
        if (classInfo.IsPublic)
        {
            signature += "public ";
        }
        else if (classInfo.IsNestedPrivate)
        {
            signature += "private ";
        }
        else if (!classInfo.IsVisible)
        {
            signature += "internal ";
        }
        if (classInfo.IsAbstract)
        {
            signature += "abstract ";
        }
        if (classInfo.IsSealed)
        {
            signature += "sealed ";
        }
        if (classInfo.IsClass)
        {
            signature += "class ";
        }
        else if (classInfo.IsInterface)
        {
            signature += "interface ";
        }

        signature += classInfo.Name;
        return signature;
    }

    private static string GetFieldSignature(FieldInfo field)
    {
        var signature = "";
        if (field.IsPublic)
        {
            signature += "public ";
        }
        else if (field.IsPrivate)
        {
            signature += "private ";
        }
        if (field.IsStatic)
        {
            signature += "static ";
        }
        
        signature += $"{field.FieldType} {field.Name};";
        return signature;
    }

    private static string GetMethodSignature(MethodInfo method)
    {
        var signature = "";
        if (method.IsPublic)
        {
            signature += "public ";
        }
        else if (method.IsPrivate)
        {
            signature += "private ";
        }
        if (method.IsStatic)
        {
            signature += "static ";
        }
        
        signature += $"{method.ReturnType} {method.Name}";
        signature += '(';
        var parameters = new List<string>();
        foreach (var parameter in method.GetParameters())
        {
            parameters.Add($"{parameter.ParameterType} {parameter.Name}");
        }

        signature += string.Join(", ", parameters);
        signature += ')';
        return signature;
    }

    private static string GetIndent(int numberOfTabs)
        => string.Concat(Enumerable.Repeat(Tab, numberOfTabs));
}
