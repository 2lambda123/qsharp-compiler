﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Quantum.QsCompiler.SyntaxTokens;
using Microsoft.Quantum.QsCompiler.SyntaxTree;
using Microsoft.Quantum.QsCompiler.Transformations.QsCodeOutput;
using YamlDotNet.RepresentationModel;

namespace Microsoft.Quantum.QsCompiler.Documentation
{
    using QsTypeKind = QsTypeKind<ResolvedType, UserDefinedType, QsTypeParameter, CallableInformation>;

    internal static class Utils
    {
        // Various hard-coded YAML tags
        internal static readonly string UidKey = "uid";
        internal static readonly string ContentsKey = "content";
        internal static readonly string NameKey = "name";
        internal static readonly string TypeKey = "type";
        internal static readonly string TypesListKey = "types";
        internal static readonly string NamespaceKey = "namespace";
        internal static readonly string SummaryKey = "summary";
        internal static readonly string RemarksKey = "remarks";
        internal static readonly string ExamplesKey = "examples";
        internal static readonly string SyntaxKey = "syntax";
        internal static readonly string ReferencesKey = "references";
        internal static readonly string SeeAlsoKey = "seeAlso";
        internal static readonly string InputKey = "input";
        internal static readonly string OutputKey = "output";
        internal static readonly string TypeParamsKey = "typeParameters";
        internal static readonly string FunctorsKey = "functors";
        internal static readonly string ItemsKey = "items";

        internal static readonly string OperationKind = "operation";
        internal static readonly string FunctionKind = "function";
        internal static readonly string UdtKind = "newtype";

        internal static readonly string QsYamlMime = "YamlMime:QSharpType";
        internal static readonly string QsNamespaceYamlMime = "YamlMime:QSharpNamespace";

        internal static readonly string AutogenerationWarning = @"
# This file is automatically generated.
# Please do not modify this file manually, or your changes may be lost when
# documentation is rebuilt.
";

        internal static readonly string TableOfContents = "toc";
        internal static readonly string YamlExtension = ".yml";
        internal static readonly string LogExtension = ".log";

        internal static YamlNode BuildStringNode(string? item)
        {
            var node = new YamlScalarNode(item);

            // Set the style to literal (YAML |-) if the string is multi-line
            if (item?.IndexOfAny(new char[] { '\n', '\r' }) >= 0)
            {
                node.Style = YamlDotNet.Core.ScalarStyle.Literal;
            }

            return node;
        }

        internal static void AddString(this YamlSequenceNode root, string item) => root.Add(BuildStringNode(item));

        // We don't need to set the key to literal because all of our keys are simple one-line strings.
        internal static void AddStringMapping(this YamlMappingNode root, string key, string? item)
        {
            root.Children[new YamlScalarNode(key)] = BuildStringNode(item);
        }

        internal static YamlMappingNode BuildMappingNode(string key1, string val1, string key2, string val2)
        {
            var node = new YamlMappingNode();
            node.AddStringMapping(key1, val1);
            node.AddStringMapping(key2, val2);
            return node;
        }

        /// <summary>
        /// Builds a YAML sequence node holding a list of strings.
        /// </summary>
        /// <param name="items">The list of strings</param>
        /// <returns>The (new) sequence node</returns>
        internal static YamlSequenceNode BuildSequenceNode(List<string> items)
        {
            var seqNode = new YamlSequenceNode();

            foreach (var item in items)
            {
                seqNode.AddString(item);
            }

            return seqNode;
        }

        /// <summary>
        /// Builds a uid/summary TAML sequence node from a dictionary.
        /// </summary>
        /// <param name="pairs">A dictionary mapping uids to summaries</param>
        /// <returns>A new YAML sequence node containing a list of mapping nodes,
        /// each with a uid value and a summary value</returns>
        internal static YamlNode BuildSequenceMappingNode(Dictionary<string, string> pairs)
        {
            var seqNode = new YamlSequenceNode();

            foreach (var pair in pairs)
            {
                var uidKeyNode = BuildStringNode("uid");
                var uidValueNode = BuildStringNode(pair.Key);
                var summaryKeyNode = BuildStringNode("summary");
                var summaryValueNode = BuildStringNode(pair.Value);
                seqNode.Add(new YamlMappingNode(uidKeyNode, uidValueNode, summaryKeyNode, summaryValueNode));
            }

            return seqNode;
        }

        // TODO: we need to get the current namespace here somehow so that referenced UDT names
        // don't get expanded with the full namespace name

        /// <summary>
        /// Returns the Q# source representation of a resolved type.
        /// </summary>
        /// <param name="t">The resolved type</param>
        /// <returns>A string containing the source representation of the type</returns>
        internal static string? ResolvedTypeToString(ResolvedType t) =>
            SyntaxTreeToQsharp.Default.ToCode(t);

        /// <summary>
        /// Populates a YAML mapping node with information describing a Q# resolved type.
        /// </summary>
        /// <param name="t">The resolved type to describe</param>
        /// <param name="map">The YAML node to populate</param>
        internal static void ResolvedTypeToYaml(ResolvedType t, YamlMappingNode map)
        {
            IEnumerable<ResolvedType> FlattenType(ResolvedType ty)
            {
                var resol = ty.Resolution;
                if (resol.IsTupleType)
                {
                    var elements = ((QsTypeKind.TupleType)resol).Item;
                    foreach (var element in elements)
                    {
                        foreach (var subelement in FlattenType(element))
                        {
                            yield return subelement;
                        }
                    }
                }
                else
                {
                    yield return ty;
                }
            }

            void CallableCore(ResolvedType inputType, ResolvedType outputType, IEnumerable<QsFunctor> functors)
            {
                var types = new YamlSequenceNode();
                var input = new YamlMappingNode();
                input.Add("types", types);
                foreach (var argType in FlattenType(inputType))
                {
                    var argNode = new YamlMappingNode();
                    ResolvedTypeToYaml(argType, argNode);
                    types.Add(argNode);
                }

                map.Add("input", input);
                var otypes = new YamlSequenceNode();
                var output = new YamlMappingNode();
                output.Add("types", otypes);
                var otype = new YamlMappingNode();
                ResolvedTypeToYaml(outputType, otype);
                otypes.Add(otype);
                map.Add("output", output);
            }

            var resolution = t.Resolution;

            if (resolution.IsUnitType)
            {
                map.AddStringMapping("isPrimitive", "true");
                map.AddStringMapping("uid", "Unit");
            }
            else if (resolution.IsInt || resolution.IsBigInt || resolution.IsDouble || resolution.IsBool || resolution.IsString ||
                        resolution.IsQubit || resolution.IsResult || resolution.IsPauli || resolution.IsRange)
            {
                map.AddStringMapping("isPrimitive", "true");
                map.AddStringMapping("uid", ResolvedTypeToString(t));
            }
            else if (resolution.IsArrayType)
            {
                map.AddStringMapping("isArray", "true");
                var elementType = ((QsTypeKind.ArrayType)resolution).Item;
                if (elementType.Resolution.IsArrayType)
                {
                    var seq = new YamlSequenceNode();
                    var node = new YamlMappingNode();
                    ResolvedTypeToYaml(elementType, node);
                    seq.Add(node);
                    map.Add(BuildStringNode("types"), seq);
                }
                else
                {
                    ResolvedTypeToYaml(elementType, map);
                }
            }
            else if (resolution.IsTupleType)
            {
                var elements = ((QsTypeKind.TupleType)resolution).Item;
                var seq = new YamlSequenceNode();
                foreach (var element in elements)
                {
                    var node = new YamlMappingNode();
                    ResolvedTypeToYaml(element, node);
                    seq.Add(node);
                }

                map.Add(BuildStringNode("types"), seq);
            }
            else if (resolution.IsUserDefinedType)
            {
                var udtName = ((QsTypeKind.UserDefinedType)resolution).Item;
                map.AddStringMapping("uid", (udtName.Namespace + "." + udtName.Name).ToLowerInvariant());
            }
            else if (resolution.IsTypeParameter)
            {
                var typeParam = ((QsTypeKind.TypeParameter)resolution).Item;
                map.AddStringMapping("uid", "'" + typeParam.TypeName);
                map.AddStringMapping("isLocal", "true");
            }
            else if (resolution.IsOperation)
            {
                var op = (QsTypeKind.Operation)resolution;
                var inputType = op.Item1.Item1;
                var outputType = op.Item1.Item2;
                var functors = op.Item2.Characteristics.SupportedFunctors.ValueOr(ImmutableHashSet<QsFunctor>.Empty);

                map.AddStringMapping("isOperation", "true");
                CallableCore(inputType, outputType, functors);
                if (!functors.IsEmpty)
                {
                    var seq = new YamlSequenceNode();
                    map.Add("functors", seq);
                    foreach (var f in functors)
                    {
                        if (f.IsAdjoint)
                        {
                            seq.AddString("Adjoint");
                        }
                        else if (f.IsControlled)
                        {
                            seq.AddString("Controlled");
                        }
                    }
                }
            }
            else if (resolution.IsFunction)
            {
                var fct = (QsTypeKind.Function)resolution;
                var inputType = fct.Item1;
                var outputType = fct.Item2;

                map.AddStringMapping("isFunction", "true");
                CallableCore(inputType, outputType, Enumerable.Empty<QsFunctor>());
            }
        }

        /// <summary>
        /// Returns a formatted argument string for a Q# callable.
        /// </summary>
        /// <param name="callable">The operation or function to create an argument string for</param>
        /// <returns>The argument string</returns>
        internal static string CallableToArguments(QsCallable callable)
        {
            void ProcessTuple(StringBuilder builder, QsTuple<LocalVariableDeclaration<QsLocalSymbol, ResolvedType>> tuple, bool topLevel)
            {
                if (tuple.IsQsTupleItem)
                {
                    var item = ((QsTuple<LocalVariableDeclaration<QsLocalSymbol, ResolvedType>>.QsTupleItem)tuple).Item;
                    if (topLevel)
                    {
                        builder.Append('(');
                    }

                    // Assumes that the symbol will never be invalid
                    builder.Append(((QsLocalSymbol.ValidName)item.VariableName).Item);
                    builder.Append(" : ");
                    builder.Append(ResolvedTypeToString(item.Type));
                    if (topLevel)
                    {
                        builder.Append(')');
                    }
                }
                else
                {
                    var items = ((QsTuple<LocalVariableDeclaration<QsLocalSymbol, ResolvedType>>.QsTuple)tuple).Item;
                    builder.Append('(');
                    var first = true;
                    foreach (var item in items)
                    {
                        if (!first)
                        {
                            builder.Append(", ");
                        }

                        first = false;
                        ProcessTuple(builder, item, false);
                    }

                    builder.Append(')');
                }
            }

            var args = callable.ArgumentTuple;
            var sb = new StringBuilder();
            ProcessTuple(sb, args, true);
            return sb.ToString();
        }

        /// <summary>
        /// Generates a syntax string for a callable.
        /// This is, more or less, the same as the start of the callable definition, before the
        /// first opening {.
        /// </summary>
        /// <param name="callable">The callable (operation or function) to create a syntax string for</param>
        /// <returns>The syntax string</returns>
        internal static string CallableToSyntax(QsCallable callable)
        {
            var sb = new StringBuilder();
            sb.Append(callable.Kind == QsCallableKind.Function ? "function " : "operation ");
            sb.Append(callable.FullName.Name);
            sb.Append(" ");
            sb.Append(CallableToArguments(callable));
            sb.Append(" : ");
            sb.Append(ResolvedTypeToString(callable.Signature.ReturnType));
            return sb.ToString();
        }

        /// <summary>
        /// Generates a syntax string for a user-defined type.
        /// This is more or less the same as the Q# type definition.
        /// </summary>
        /// <param name="customType">The UDT to create a syntax string for</param>
        /// <returns>The syntax string</returns>
        internal static string CustomTypeToSyntax(QsCustomType customType)
        {
            // TODO: Include modifiers.
            var sb = new StringBuilder();
            sb.Append("newtype ");
            sb.Append(customType.FullName.Name);
            sb.Append(" = ");
            sb.Append(ResolvedTypeToString(customType.Type));
            sb.Append(";");
            return sb.ToString();
        }

        /// <summary>
        /// Reads a YAML file and returns the top-level node.
        /// Returns a null if the file doesn't exist or is not a YAML file.
        /// </summary>
        /// <param name="rootPath">The directory where the file should exist.</param>
        /// <param name="fileBaseName">The base name of the file, not including the ".yml" extension.</param>
        internal static YamlNode? ReadYamlFile(string rootPath, string fileBaseName)
        {
            var yamlReader = new YamlStream();
            YamlNode? fileNode = null;
            DoIgnoringExceptions(() =>
            {
                var fileName = Path.Combine(rootPath, fileBaseName + YamlExtension);
                using (var readStream = File.OpenText(fileName))
                {
                    yamlReader.Load(readStream);
                }

                fileNode = yamlReader.Documents[0].RootNode;
            });
            return fileNode;
        }

        /// <summary>
        /// Writes a YAML node to a file.
        /// Any exceptions are caught, logged, and then ignored.
        /// </summary>
        /// <param name="rootNode">The YAML node to write.</param>
        /// <param name="rootPath">The path of the directory where the file should be written.</param>
        /// <param name="fileBaseName">The base name of the file. The correct YAML extension will be added.</param>
        internal static void WriteYamlFile(YamlNode rootNode, string rootPath, string fileBaseName)
        {
            var doc = new YamlDocument(rootNode);
            var stream = new YamlStream(doc);

            var fileName = Path.Combine(rootPath, fileBaseName + YamlExtension);
            using (var text = new StreamWriter(File.Open(fileName, FileMode.Create)))
            {
                text.WriteLine(AutogenerationWarning);
                stream.Save(text, false);
            }
        }

        /// <summary>
        /// Merges data from a YAML map into an existing YAML file, which should be a map at the top level.
        /// The file contents are rewritten.
        /// Keys that exist in only one map get moved to the new contents as-is.
        /// Keys that exist in both maps get the value from the passed-in node, overwriting the value in the file.
        /// If the file is not a YAML map, then its contents are blindly overwritten.
        /// </summary>
        /// <param name="map">The YAML mapping node to merge into the data in the file.</param>
        /// <param name="fileBaseName">The full path name of the file to read and rewrite.</param>
        internal static void MergeYamlFile(YamlMappingNode map, string rootPath, string fileBaseName)
        {
            var yamlReader = new YamlStream();
            var fileMap = new YamlMappingNode();
            DoIgnoringExceptions(() =>
            {
                var fileName = Path.Combine(rootPath, fileBaseName + YamlExtension);
                using (var readStream = File.OpenText(fileName))
                {
                    yamlReader.Load(readStream);
                }

                fileMap = yamlReader.Documents[0].RootNode as YamlMappingNode ?? fileMap;
            });
            foreach (var entry in map.Children)
            {
                var key = (YamlScalarNode)entry.Key;
                fileMap.Children[key] = entry.Value;
            }

            WriteYamlFile(fileMap, rootPath, fileBaseName);
        }

        /// <summary>
        /// Runs an action, logging any exception to a file and then eating the exception.
        /// </summary>
        /// <param name="act">The action to run.</param>
        /// <param name="logFileName">The name of the file to log exceptions to.
        /// If omitted, the exception is not logged at all.</param>
        internal static void DoIgnoringExceptions(Action act, string? logFileName = null)
        {
            try
            {
                act();
            }
            catch (Exception ex)
            {
                // Ignore all exceptions after logging
                if (logFileName != null)
                {
                    File.AppendAllText(logFileName, $"{DateTime.Now.ToString("u")}: Unexpected error ignored: ${ex.Message}\nat:\n{ex.StackTrace}\n");
                }
            }
        }

        /// <summary>
        /// Runs an action, adding any exception to a list and then eating the exception.
        /// </summary>
        /// <param name="act">The action to run.</param>
        /// <param name="errors">The list that exceptions should be added to.</param>
        internal static void DoTrackingExceptions(Action act, List<Exception> errors)
        {
            try
            {
                act();
            }
            catch (Exception ex)
            {
                // Ignore all exceptions after recording
                if (ex is AggregateException agg)
                {
                    errors.AddRange(agg.InnerExceptions);
                }
                else
                {
                    errors.Add(ex);
                }
            }
        }

        internal static string AsObsoleteUid(this string qualifiedName) =>
            qualifiedName.ToLowerInvariant();

        internal static string AsUid(this string qualifiedName) =>
            qualifiedName;

        /// <summary>
        /// Returns the text as a Markdown warning block.
        /// </summary>
        internal static string Warning(string text)
        {
            var lines = text.Split('\r', '\n').Select(line => ("> " + line).TrimEnd());
            return "> [!WARNING]\n" + string.Join('\n', lines);
        }
    }

    // See https://stackoverflow.com/a/5037815/267841.
    internal static class SortExtensions
    {
        internal static void AddRange<T>(this IList<T> list, IEnumerable<T> source)
        {
            if (list is List<T> concreteList)
            {
                concreteList.AddRange(source);
            }
            else
            {
                foreach (var element in list)
                {
                    list.Add(element);
                }
            }
        }

        // Sorts an IList<T> in place.
        internal static void Sort<T>(this IList<T> list, Comparison<T> comparison)
        {
            ArrayList.Adapter((IList)list).Sort(new ComparisonComparer<T>(comparison));
        }

        // Convenience method on IEnumerable<T> to allow passing of a
        // Comparison<T> delegate to the OrderBy method.
        internal static IEnumerable<T> OrderBy<T>(this IEnumerable<T> list, Comparison<T> comparison) =>
            list.OrderBy(t => t, new ComparisonComparer<T>(comparison));
    }

    internal class ComparisonComparer<T> : IComparer<T>, IComparer
    {
        private readonly Comparison<T> comparison;

        internal ComparisonComparer(Comparison<T> comparison)
        {
            this.comparison = comparison;
        }

        public int Compare(T x, T y) => this.comparison(x, y);

        public int Compare(object o1, object o2) => this.comparison((T)o1, (T)o2);
    }
}
