using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;


namespace AppSettingsManagement.Generators
{

    [Generator(LanguageNames.CSharp)]
    public class SettingsItemSourceGenerator : ISourceGenerator
    {
        public SettingsItemSourceGenerator()
        {
#if DEBUG
            if (!Debugger.IsAttached)
            {
                //Debugger.Launch();
            }
#endif
        }
        public void Initialize(GeneratorInitializationContext context)
        {
            Console.WriteLine("SOURCE GENERATOR LOADED");
            context.RegisterForSyntaxNotifications(() => new SettingsItemSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxReceiver = (SettingsItemSyntaxReceiver)context.SyntaxReceiver;

            foreach (var constructorInfo in syntaxReceiver.ConstructorsWithSettingsItemAttribute)
            {
                INamedTypeSymbol classSymbol = (INamedTypeSymbol)context.Compilation
                    .GetSemanticModel(constructorInfo.Node.SyntaxTree)
                    .GetDeclaredSymbol(constructorInfo.Node);

                string classAccessibility = classSymbol.DeclaredAccessibility.ToString().ToLower();
                string className = classSymbol.Name;
                string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

                var propertiesBuilder = new StringBuilder();

                foreach (var attributeInfo in constructorInfo.Attributes)
                {
                    propertiesBuilder.AppendLine($@"public {attributeInfo.PropertyType} {attributeInfo.Name} {{ get; set; }}");
                }

                var source = $@"namespace {namespaceName}
{{
    {classAccessibility} partial class {className}
    {{
        {propertiesBuilder}
    }}
}}";
                context.AddSource($"{className}_SettingsItemGenerated", SourceText.From(source, Encoding.UTF8));
            }
        }
    }

    internal class SettingsItemSyntaxReceiver : ISyntaxReceiver
    {
        public List<(ClassDeclarationSyntax Node, List<(string Name, string PropertyType)> Attributes)> ConstructorsWithSettingsItemAttribute { get; } = new List<(ClassDeclarationSyntax Node, List<(string Name, string PropertyType)> Attributes)>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
            {
                var constructors = classDeclarationSyntax.DescendantNodes().OfType<ConstructorDeclarationSyntax>();
                foreach (var constructor in constructors)
                {
                    var settingsItemAttributes = new List<(string Name, string PropertyType)>();

                    foreach (var attributeList in constructor.AttributeLists)
                    {
                        foreach (var attribute in attributeList.Attributes)
                        {
                            if (attribute.Name.ToString().Contains("SettingItem"))
                            {
                                var attributeArguments = attribute.ArgumentList.Arguments;
                                if (attributeArguments.Count == 2)
                                {
                                    string propertyType = attributeArguments[0].Expression.ToString();
                                    propertyType = propertyType.Substring("typeof(".Length, propertyType.Length - 1 - "typeof(".Length); // Remove typeof()
                                    string name = attributeArguments[1].Expression.ToString().Trim('"');
                                    settingsItemAttributes.Add((name, propertyType));
                                }
                            }
                        }
                    }

                    if (settingsItemAttributes.Any())
                    {
                        ConstructorsWithSettingsItemAttribute.Add((classDeclarationSyntax, settingsItemAttributes));
                    }
                }
            }
        }
    }

}
