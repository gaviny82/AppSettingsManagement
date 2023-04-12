﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            context.RegisterForSyntaxNotifications(() => new SettingsContainerSyntaxReceiver());
        }

        void GenerateSettingItem(AttributeSyntax attributeSyntax, StringBuilder memberBuilder)
        {
            var arguments = attributeSyntax.ArgumentList.Arguments;
            if (arguments.Count < 2) return;

            // Parse property type
            string propertyType = arguments[0].Expression.ToString();
            propertyType = propertyType.Substring("typeof(".Length, propertyType.Length - 1 - "typeof(".Length); // Remove typeof()

            // Parse property name
            string propertyName = arguments[1].Expression.ToString().Trim('"');

            string defaultValue = "";
            string converter = "";

            // Parse default value and converter
            for (int i = 2; i < arguments.Count; i++)
            {
                if (arguments[i].NameEquals?.Name.ToString() == "Default")
                {
                    defaultValue = $", {arguments[i].Expression.ToString().Trim()}";
                }
                else if (arguments[i].NameEquals?.Name.ToString() == "Converter")
                {
                    string converterType = arguments[i].Expression.ToString();
                    converter = $", global::AppSettingsManagement.DataTypeConverters.GetConverter(typeof({converterType}))";
                }
            }

            // If default value is not provided, the property is nullable
            string nullable = string.IsNullOrEmpty(defaultValue) ? "?" : "";

            memberBuilder.Append($$"""
                        public {{propertyType}}{{nullable}} {{propertyName}}
                        {
                            get => GetValue<{{propertyType}}{{nullable}}>(nameof({{propertyName}}){{defaultValue}}{{converter}});
                            set => SetValue<{{propertyType}}>(nameof({{propertyName}}), value, ref {{propertyName}}Changed{{converter}});
                        }

                        public event global::AppSettingsManagement.SettingChangedEventHandler? {{propertyName}}Changed;


                """);
        }

        void GenerateSettingsContainer(AttributeSyntax attributeSyntax, StringBuilder membersBuilder, StringBuilder initBuilder)
        {
            var arguments = attributeSyntax.ArgumentList.Arguments;
            if (arguments.Count < 2) return;

            // Parse property type
            string propertyType = arguments[0].Expression.ToString();
            propertyType = propertyType.Substring("typeof(".Length, propertyType.Length - 1 - "typeof(".Length); // Remove typeof()

            // Parse property name
            string propertyName = arguments[1].Expression.ToString().Trim('"');

            // Generate code
            membersBuilder.Append($$"""
                        public {{propertyType}} {{propertyName}} { get; private set; } = null!;

                """);

            initBuilder.Append($$"""
                            {{propertyName}} = new {{propertyType}}(Storage, "{{propertyName}}", this);

                """);
        }

        void GenerateSettingsCollection(AttributeSyntax attributeSyntax, StringBuilder membersBuilder, StringBuilder initBuilder)
        {
            var arguments = attributeSyntax.ArgumentList.Arguments;
            if (arguments.Count < 2) return;

            // Parse property type
            string elementType = arguments[0].Expression.ToString();
            elementType = elementType.Substring("typeof(".Length, elementType.Length - 1 - "typeof(".Length); // Remove typeof()

            // Parse property name
            string propertyName = arguments[1].Expression.ToString().Trim('"');

            // Parse converter
            string converter = "";
            if (arguments.Count > 2)
            {
                string converterType = arguments[2].Expression.ToString();
                converter = $", global::AppSettingsManagement.DataTypeConverters.GetConverter(typeof({converterType}))";
            }

            // Generate code
            membersBuilder.Append($$"""
                        public global::AppSettingsManagement.SettingsCollection<{{elementType}}> {{propertyName}} { get; private set; } = null!;

                """);

            initBuilder.Append($$"""
                            {{propertyName}} = new global::AppSettingsManagement.SettingsCollection<{{elementType}}>(Storage, "{{propertyName}}"{{converter}});

                """);
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxReceiver = (SettingsContainerSyntaxReceiver)context.SyntaxReceiver;

            foreach (var (classDeclaration, constructorAttributes) in syntaxReceiver.SettingsContainerConstructorAttributes)
            {
                INamedTypeSymbol classSymbol = 
                    (INamedTypeSymbol)context.Compilation
                    .GetSemanticModel(classDeclaration.SyntaxTree)
                    .GetDeclaredSymbol(classDeclaration);

                string className = classSymbol.Name;
                string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

                var membersBuilder = new StringBuilder();
                var initBuilder = new StringBuilder();
                foreach (var attributeInfo in constructorAttributes)
                {
                    string attributeName = attributeInfo.Name.ToString();
                    if (attributeName.EndsWith("Attribute"))
                        attributeName = attributeName.Substring(0, attributeName.Length - "Attribute".Length);

                    if (attributeName == "SettingItem")
                        GenerateSettingItem(attributeInfo, membersBuilder);
                    else if (attributeName == "SettingsContainer")
                        GenerateSettingsContainer(attributeInfo, membersBuilder, initBuilder);
                    else if (attributeName == "SettingsCollection")
                        GenerateSettingsCollection(attributeInfo, membersBuilder, initBuilder);
                }

                var source = $$"""
                    // <auto-generated/>
                    #pragma warning disable
                    #nullable enable

                    namespace {{namespaceName}}
                    {
                        partial class {{className}}
                        {
                    {{membersBuilder}}

                            protected override void InitializeContainers()
                            {
                    {{initBuilder}}
                            }
                        }
                    }
                    """;

                context.AddSource($"{className}_SettingsContainerMembersGenerated", SourceText.From(source, Encoding.UTF8));
            }
        }
    }

    internal class SettingsContainerSyntaxReceiver : ISyntaxReceiver
    {
        public List<(ClassDeclarationSyntax Node, List<AttributeSyntax> Attributes)> SettingsContainerConstructorAttributes { get; } = new List<(ClassDeclarationSyntax Node, List<AttributeSyntax> Attributes)>();

        private readonly string[] settingAttributes = new string[] 
        {
            "SettingItem", "SettingItemAttribute",
            "SettingsContainer", "SettingsContainerAttribute",
            "SettingsCollection", "SettingsCollectionAttribute"
        };

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
            {
                // Check if the class inherits SettingsContainer
                bool isSettingsContainer = 
                    classDeclarationSyntax?.BaseList?.Types
                    .Where(type => (type.Type as IdentifierNameSyntax)?
                        .Identifier
                        .Text == "SettingsContainer")
                    .Any() ?? false;

                if (!isSettingsContainer) return;

                // Find attributes of the constructor
                var constructors = classDeclarationSyntax.DescendantNodes().OfType<ConstructorDeclarationSyntax>();

                foreach (ConstructorDeclarationSyntax constructor in constructors)
                {
                    List<AttributeSyntax> attributes = constructor.AttributeLists
                        .SelectMany(attributeList => attributeList.Attributes)                      // Flatten the list of constructor attributes
                        .Where(attribute => settingAttributes.Contains(attribute.Name.ToString()))  // Filter supported attributes
                        .ToList();

                    if (attributes.Count != 0)
                    {
                        SettingsContainerConstructorAttributes.Add((classDeclarationSyntax, attributes));
                    }
                }
            }
        }
    }

}
