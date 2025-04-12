// Copyright (c) Alexander Bugaev 2024
//
// Use of this source code is governed by an MIT license
// that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

namespace Test2;

using System.Reflection;

/// <summary>
/// Class containing utility for printing type signatures using reflection.
/// </summary>
public static class Reflector
{
    private const string Indent = "    ";
    private const BindingFlags All = (BindingFlags)(-1);

    /// <summary>
    /// Writes the signature of the given type with all fields, methods and nested classes.
    /// Saves the resulting signature to the file with name of the type.
    /// </summary>
    /// <param name="someType">The type to write signature of.</param>
    /// <returns>A task representing the writing process.</returns>
    public static async Task PrintStructure(Type someType)
    {
        using var writer = new StreamWriter($"{someType.Name}.cs");
        await PrintStructureInternal(someType, 0, writer);
    }

    private static async Task PrintStructureInternal(
        Type someType, int indentCount, StreamWriter writer)
    {
        var indent = GetIndent(indentCount);
        await WriteClassInfo(someType, indentCount, writer);
        await writer.WriteLineAsync(indent + '{');

        var fields = someType.GetFields(All);
        var methods = someType.GetMethods(All);
        var nestedTypes = someType.GetNestedTypes(All);

        foreach (var field in fields)
        {
            await PrintField(field, indentCount + 1, writer);
        }

        foreach (var method in methods)
        {
            await PrintMethod(method, indentCount + 1, writer);
        }

        foreach (var nestedType in nestedTypes)
        {
            await PrintStructureInternal(nestedType, indentCount + 1, writer);
        }

        await writer.WriteLineAsync(indent + '}');
    }

    private static async Task WriteClassInfo(
        Type classInfo, int indentCount, StreamWriter writer)
    {
        var indent = GetIndent(indentCount);
        var signature = GetClassSignature(classInfo);
        await writer.WriteLineAsync(indent + signature);
    }

    private static async Task PrintField(
        FieldInfo field, int indentCount, StreamWriter writer)
    {
        var indent = GetIndent(indentCount);
        var signature = GetFieldSignature(field);
        await writer.WriteLineAsync(indent + signature);
    }

    private static async Task PrintMethod(
        MethodInfo method, int indentCount, StreamWriter writer)
    {
        var indent = GetIndent(indentCount);
        var signature = GetMethodSignature(method);
        await writer.WriteLineAsync(indent + signature);
    }

    private static string GetClassSignature(Type classInfo)
    {
        var signature = "";
        if (classInfo.IsPublic || classInfo.IsNestedPublic)
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
        signature += ");";
        return signature;
    }

    private static string GetIndent(int indentCount)
        => string.Concat(Enumerable.Repeat(Indent, indentCount));
}
