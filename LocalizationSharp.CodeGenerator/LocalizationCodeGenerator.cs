using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LocalizationSharp.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace LocalizationSharp.CodeGenerator
{
    public static class LocalizationCodeGenerator
    {
        private static Regex _invalidKeyReplaceRegex =
            new Regex(@"[-!#$%&()=~^|+{};*:<>?,./{}\[\]\`\\ ]", RegexOptions.Compiled);

        public static string Generate(CodeGenerateOptions options = null, LocalizationManager manager = null)
        {
            if (options == null)
                options = new CodeGenerateOptions();

            if (manager == null)
                manager = LocalizationManager.Instance;

            if (manager == null)
                throw new InvalidOperationException("LocalizationManagerが初期化されていません。");

            ILocalizeFile file = manager.GetFile();

            var _ = typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions);

            Workspace workspace = new AdhocWorkspace();
            SyntaxGenerator generator = SyntaxGenerator.GetGenerator(workspace, LanguageNames.CSharp);
            List<SyntaxNode> nodes = new List<SyntaxNode>();
            string[] imports = new[]
            {
                "System"
            }.Concat(options.AdditionalUsing).ToArray();
            foreach (string ns in imports)
            {
                nodes.Add(generator.NamespaceImportDeclaration(ns));
            }

            List<SyntaxNode> memberNodes = new List<SyntaxNode>();
            if (options.GenerateMemberMode == GenerateMemberMode.ExtensionMethod)
            {
                foreach (KeyValuePair<string, ILocalizeContent<object>> pair in file)
                {
                    string varName = $"{(options.UseFirstAtChar ? "@" : string.Empty)}{InvalidKeyReplace(pair.Key)}";
                    if (options.TypeAccessMode == TypeAccessMode.Default)
                        memberNodes.Add(generator.MethodDeclaration(varName,
                            new[]
                            {
                                SyntaxFactory.Parameter(SyntaxFactory.Identifier("value"))
                                    .WithType(GetTypeSyntax(options.ExtensionType))
                                    .WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
                            },
                            null, GetTypeSyntax(pair.Value.GetType()), Accessibility.Public,
                            DeclarationModifiers.Static,
                            new[]
                            {
                                generator.ReturnStatement(
                                    SyntaxFactory.ParseExpression(
                                        $"LocalizationManager.Instance.GetFile()[\"{pair.Key}\"]"))
                            }));
                    else if (options.TypeAccessMode == TypeAccessMode.QuickAccess)
                        memberNodes.Add(generator.MethodDeclaration(varName,
                            new[]
                            {
                                SyntaxFactory.Parameter(SyntaxFactory.Identifier("value"))
                                    .WithType(GetTypeSyntax(options.ExtensionType))
                                    .WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
                            },
                            null, GetTypeSyntax(pair.Value.Content.GetType()), Accessibility.Public,
                            DeclarationModifiers.Static,
                            new[]
                            {
                                generator.ReturnStatement(
                                    SyntaxFactory.ParseExpression(
                                        $"LocalizationManager.Instance.GetFile()[\"{pair.Key}\"].Content"))
                            }));
                    else if (options.TypeAccessMode == TypeAccessMode.Dynamic)
                        memberNodes.Add(generator.MethodDeclaration(varName,
                            new[]
                            {
                                SyntaxFactory.Parameter(SyntaxFactory.Identifier("value"))
                                    .WithType(GetTypeSyntax(options.ExtensionType))
                                    .WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
                            },
                            null, GetTypeSyntax(typeof(ILocalizeContent<object>)), Accessibility.Public,
                            DeclarationModifiers.Static,
                            new[]
                            {
                                generator.ReturnStatement(
                                    SyntaxFactory.ParseExpression(
                                        $"LocalizationManager.Instance.GetFile()[\"{pair.Key}\"]"))
                            }));
                }
            }
            else if (options.GenerateMemberMode == GenerateMemberMode.Property)
            {
                foreach (KeyValuePair<string, ILocalizeContent<object>> pair in file)
                {
                    string varName = $"{(options.UseFirstAtChar ? "@" : string.Empty)}{InvalidKeyReplace(pair.Key)}";
                    if (options.TypeAccessMode == TypeAccessMode.Default)
                        memberNodes.Add(generator.PropertyDeclaration(varName, GetTypeSyntax(pair.Value.GetType()),
                            Accessibility.Public, DeclarationModifiers.Static.WithIsReadOnly(true),
                            new[]
                            {
                                generator.ReturnStatement(
                                    SyntaxFactory.ParseExpression(
                                        $"LocalizationManager.Instance.GetFile()[\"{pair.Key}\"]"))
                            }));
                    else if (options.TypeAccessMode == TypeAccessMode.QuickAccess)
                        memberNodes.Add(generator.PropertyDeclaration(varName,
                            GetTypeSyntax(pair.Value.Content.GetType()),
                            Accessibility.Public, DeclarationModifiers.Static.WithIsReadOnly(true),
                            new[]
                            {
                                generator.ReturnStatement(
                                    SyntaxFactory.ParseExpression(
                                        $"LocalizationManager.Instance.GetFile()[\"{pair.Key}\"].Content"))
                            }));
                    else if (options.TypeAccessMode == TypeAccessMode.Dynamic)
                        memberNodes.Add(generator.PropertyDeclaration(varName,
                            GetTypeSyntax(typeof(ILocalizeContent<object>)),
                            Accessibility.Public, DeclarationModifiers.Static.WithIsReadOnly(true),
                            new[]
                            {
                                generator.ReturnStatement(
                                    SyntaxFactory.ParseExpression(
                                        $"LocalizationManager.Instance.GetFile()[\"{pair.Key}\"]"))
                            }));
                }
            }

            SyntaxNode classNode = generator.ClassDeclaration(options.ClassName, accessibility: Accessibility.Public,
                modifiers: DeclarationModifiers.Static, members: memberNodes);

            SyntaxNode namespaceNode = generator.NamespaceDeclaration(options.Namespace, classNode);
            nodes.Add(namespaceNode);

            return generator.CompilationUnit(nodes).NormalizeWhitespace().ToFullString();
        }

        private static TypeSyntax GetTypeSyntax(Type type)
        {
            if (type.IsGenericType)
            {
                return SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier($"{type.Namespace}.{type.Name.Remove(type.Name.Length - 2, 2)}"),
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SeparatedList(
                            type.GenericTypeArguments.Select(GetTypeSyntax)
                        )
                    ));
            }

            return SyntaxFactory.IdentifierName(SyntaxFactory.Identifier($"{type.Namespace}.{type.Name}"));
        }

        private static string InvalidKeyReplace(string key)
        {
            return _invalidKeyReplaceRegex.Replace(key, "_").Replace('"', '_').Replace('\'', '_');
        }

        public class CodeGenerateOptions
        {
            /// <summary>
            /// 生成されるコードのネームスペース
            /// </summary>
            public string Namespace { get; set; } = "LocalizationSharp.Generated";

            /// <summary>
            /// 生成されるコードの型名
            /// </summary>
            public string ClassName { get; set; } = "GeneratedClass";

            /// <summary>
            /// 拡張メソッドを生やす型名
            /// </summary>
            public Type ExtensionType { get; set; } = typeof(object);

            /// <summary>
            /// 生成されるコードに追加の 'using' を挿入します。 
            /// </summary>
            public string[] AdditionalUsing { get; set; } = new string[0];

            /// <summary>
            /// 生成されるコードの型へアクセスする方法
            /// </summary>
            public TypeAccessMode TypeAccessMode { get; set; }

            /// <summary>
            /// 生成されるコードのメンバーを生成する方法
            /// </summary>
            public GenerateMemberMode GenerateMemberMode { get; set; }

            public bool UseFirstAtChar { get; set; }
        }

        public enum TypeAccessMode
        {
            Default,
            QuickAccess,
            Dynamic
        }

        public enum GenerateMemberMode
        {
            ExtensionMethod,
            Property
        }
    }
}