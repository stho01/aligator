using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.Text;

namespace Aligator.Generator;

public static class CssCompiler
{
    public static string Compile(string accessorName, SourceText? sourceText)
    {
        if (sourceText == null) return "";

        var classes = GetCssClasses(sourceText.ToString());
        var builder = new StringBuilder();

        builder.AppendLine($"public static class {accessorName}Style {{");
        foreach (var @class in classes)
        {
            builder.AppendLine($"public const string {@class} = \"{@class}\";");
        }
        builder.AppendLine("}");

        // Debugger.Launch();

        return WrapClass(builder);
    }

    private static IEnumerable<string> GetCssClasses(string css)
    {
        return css.Split('.')
            .Select(ExtractCssClassDirty)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(EscapeKeyWords)
            .Distinct();
    }
    
    private static int IndexOfEndOfClass(string cssClass)
    {
        var regex = new Regex(@"\s|{|,");
        var match = regex.Match(cssClass);
        return match.Success ? match.Index : cssClass.Length;
    }

    private static string WrapClass(StringBuilder innerSource)
    {
        const string template = @"
namespace Aligator {{
    {0}
}}
        ";

        return string.Format(template, innerSource);
    }

    public static string ExtractCssClassDirty(string segment)
    {
        return segment
            .Substring(0, IndexOfEndOfClass(segment))
            .Trim();
    }

    public static string EscapeKeyWords(string @class)
    {
        return Microsoft.CodeAnalysis.CSharp.SyntaxFacts.IsReservedKeyword(Microsoft.CodeAnalysis.CSharp.SyntaxFacts.GetKeywordKind(@class)) 
            ? $"@{@class}" 
            : @class;
    }
}