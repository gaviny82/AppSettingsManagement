using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AppSettingsManagement.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public class SettingsViewModelGenerator : ISourceGenerator
{
    public SettingsViewModelGenerator()
    {
#if DEBUG
        if (!Debugger.IsAttached)
        {
            Debugger.Launch();
        }
#endif
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SettingsViewModelSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not SettingsViewModelSyntaxReceiver receiver)
            return;

        foreach (var classDeclaration in receiver.CandidateClasses)
        {
            var model = new SettingsViewModelInformation(classDeclaration);
            var source = "//"; // TODO: generate source

            context.AddSource($"{model.ClassName}_Generated.cs", source);
        }
    }

}

public class SettingsViewModelSyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> CandidateClasses { get; } = new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclaration &&
            classDeclaration.BaseList?.Types.Any(type => type.ToString() == "ISettingsViewModel") == true)
        {
            CandidateClasses.Add(classDeclaration);
        }
    }
}


public class SettingsViewModelInformation
{
    public string ClassName { get; }
    public string SettingsProviderMemberName { get; private set; }
    public List<(string Name, string Path, string Type)> BindToSettingMembers { get; } = new();

    public SettingsViewModelInformation(ClassDeclarationSyntax classDeclaration)
    {
        ClassName = classDeclaration.Identifier.ValueText;

        // Find the name of the member with [SettingsProvider] attribute
        var settingsProviderSyntax = 
            classDeclaration.Members.Select(member => member.AttributeLists
                .SelectMany(a => a.Attributes)
                .FirstOrDefault(a => a.Name.ToString().StartsWith("SettingsProvider"))
            ).FirstOrDefault();

        // If contains multiple [SettingsProvider], the first is taken.
        if (settingsProviderSyntax is null)
            return;

        SettingsProviderMemberName = settingsProviderSyntax.Name.ToString();

        // Extract setting binding information
        foreach (var member in classDeclaration.Members)
        {
            // Restrict to members with [BindToSetting] property
            var bindToSettingAttribute = member.AttributeLists
                .SelectMany(a => a.Attributes)
                .FirstOrDefault(a => a.Name.ToString().StartsWith("BindToSetting"));

            if (bindToSettingAttribute is null)
                continue;

            // Get member name and type
            var memberName = member switch
            {
                FieldDeclarationSyntax fieldDeclarationSyntax => fieldDeclarationSyntax.Declaration.Variables.First().Identifier.ValueText,
                PropertyDeclarationSyntax propertyDeclaractionSyntax => propertyDeclaractionSyntax.Identifier.ValueText,
                _ => throw new NotSupportedException()
            };

            var type = member switch
            {
                FieldDeclarationSyntax fieldDeclarationSyntax => fieldDeclarationSyntax.Declaration.Type.ToString(),
                PropertyDeclarationSyntax propertyDeclaractionSyntax => propertyDeclaractionSyntax.Type.ToString(),
                _ => throw new NotSupportedException()
            };

            // Get binding path
            var pathArgument = bindToSettingAttribute.ArgumentList?.Arguments
                .FirstOrDefault(a => a.NameEquals?.Name.Identifier.ValueText == "Path");

            if (pathArgument is null)
                continue;

            var path = pathArgument.Expression switch
            {
                LiteralExpressionSyntax literalExpression => literalExpression.Token.ValueText,
                IdentifierNameSyntax identifierName => identifierName.Identifier.ValueText,
                InvocationExpressionSyntax invocationExpression => invocationExpression.ToString(),
                _ => pathArgument.Expression.ToString()
            };

            // Add to list
            var bindingEntry = (memberName, path, type);
            BindToSettingMembers.Add(bindingEntry);
        }
    }
}
