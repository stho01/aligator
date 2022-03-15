using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Aligator.Generator;

[Generator]
public class AligatorGenerator :  ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) { }
    
    public void Execute(GeneratorExecutionContext context)
    {
        var cssFiles = context.AdditionalFiles.Where(x => x.Path.EndsWith(".css"));
        var files = new Queue<AdditionalText>(cssFiles);
        
        while (files.Any())
        {
            var file = files.Dequeue();
            var content = file.GetText(context.CancellationToken);
            var output = CssCompiler.Compile("Main", content);
            var sourceText = SourceText.From(output, Encoding.UTF8);
            context.AddSource("Main.css.g.cs", sourceText);  
        }
    }
}