﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.Quantum.QsCompiler.CompilationBuilder;
using Microsoft.Quantum.QsCompiler.DataTypes;
using Microsoft.Quantum.QsCompiler.SyntaxTree;
using VS = Microsoft.VisualStudio.LanguageServer.Protocol;

namespace Microsoft.Quantum.QsCompiler
{
    /// <summary>
    /// Lists the priorities for built-in rewrite steps.
    /// </summary>
    public static class RewriteStepPriorities
    {
        /// <summary>
        /// Priority of the built-in transformation that replaces
        /// if-statements with the corresponding calls to built-in quantum operations if possible.
        /// </summary>
        public const int ControlFlowSubstitutions = 1100;

        /// <summary>
        /// Priority of the built-in transformation that replaces
        /// all type parametrized callables with concrete instantiations and drops any unused callables.
        /// </summary>
        public const int TypeParameterElimination = 1000;

        /// <summary>
        /// Priority of the built-in transformation that replaces
        /// all functor generation directives with the corresponding implementation.
        /// </summary>
        public const int GenerationOfFunctorSupport = 600;

        /// <summary>
        /// Priority of the built-in transformation that inlines all conjugations
        /// and thus eliminates that construct from the syntax tree.
        /// </summary>
        public const int InliningOfConjugations = 500;

        /// <summary>
        /// Priority of the built-in transformation that
        /// evaluates classical computations as much as possible.
        /// </summary>
        public const int EvaluationOfClassicalComputations = 100;

        /// <summary>
        /// Priority of the built-in transformation that infers the minimum runtime capabilities required by each
        /// callable.
        /// </summary>
        public const int CapabilityInference = 90;
    }

    public interface IRewriteStep
    {
        public enum Stage
        {
            Unknown = 0,
            PreconditionVerification = 1,
            Transformation = 2,
            PostconditionVerification = 3,
        }

        public struct Diagnostic
        {
            /// <summary>
            /// Indicates the severity of the diagnostic.
            /// Generated diagnostics may be prioritized and filtered according to their severity.
            /// </summary>
            public DiagnosticSeverity Severity { get; set; }

            /// <summary>
            /// Diagnostic message to be displayed to the user.
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// Absolute path of the file where the code that caused the generation of the diagnostic is located.
            /// The source is null if the diagnostic is not caused by a piece of source code.
            /// </summary>
            public string Source { get; set; }

            /// <summary>
            /// The stage during which the diagnostic was generated.
            /// The stage is set to Unknown if no stage is specified.
            /// </summary>
            public Stage Stage { get; set; }

            /// <summary>
            /// Zero-based range in the source file of the code that caused the generation of the diagnostic.
            /// The range is null if the diagnostic is not caused by a piece of source code.
            /// </summary>
            public Range Range { get; set; }

            /// <summary>
            /// Initializes a new diagnostic.
            /// If a diagnostic generated by the Q# compiler is given as argument, the values are initialized accordingly.
            /// </summary>
            public static Diagnostic Create(VS.Diagnostic d = null, Stage stage = Stage.Unknown) =>
                d == null ? default : new Diagnostic
                {
                    Severity = d.Severity switch
                    {
                        VS.DiagnosticSeverity.Error => DiagnosticSeverity.Error,
                        VS.DiagnosticSeverity.Warning => DiagnosticSeverity.Warning,
                        VS.DiagnosticSeverity.Information => DiagnosticSeverity.Info,
                        _ => DiagnosticSeverity.Hidden
                    },
                    Message = d.Message,
                    Source = d.Source,
                    Stage = stage,
                    Range = d.Range?.ToQSharp()
                };
        }

        /// <summary>
        /// User facing name identifying the rewrite step used for logging and in diagnostics.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The priority of the transformation relative to other transformations within the same dll or package.
        /// Steps with higher priority will be executed first.
        /// </summary>
        public int Priority { get; }

        /// <summary>
        /// Dictionary that will be populated by the Q# compiler when the rewrite step is loaded.
        /// It contains the assembly constants for the Q# compilation unit on which the rewrite step is acting.
        /// </summary>
        public IDictionary<string, string> AssemblyConstants { get; }

        /// <summary>
        /// Contains diagnostics generated by the rewrite step and intended for display to the user.
        /// Depending on the specified build configuration, the generated diagnostics may be queried
        /// after all implemented interface methods have been executed.
        /// </summary>
        public IEnumerable<Diagnostic> GeneratedDiagnostics { get; }

        /// <summary>
        /// If a precondition verification is implemented, that verification is executed prior to executing anything else.
        /// If the verification fails, nothing further is executed and the rewrite step is terminated.
        /// </summary>
        public bool ImplementsPreconditionVerification { get; }

        /// <summary>
        /// Indicates whether or not the rewrite step intends to modify the compilation in any form.
        /// If a transformation is implemented, then that transformation will be executed only if either
        /// no precondition verification is implemented, or the implemented precondition verification succeeds.
        /// </summary>
        public bool ImplementsTransformation { get; }

        /// <summary>
        /// A postcondition verification provides the means for diagnostics generation and detailed checks after transformation.
        /// The verification is executed only if the precondition verification passes and after applying the implemented transformation (if any).
        /// </summary>
        public bool ImplementsPostconditionVerification { get; }

        /// <summary>
        /// Verifies whether a given compilation satisfies the precondition for executing this rewrite step.
        /// <see cref="ImplementsPreconditionVerification"/> indicates whether or not this method is implemented.
        /// If the precondition verification succeeds, then the invocation of an implemented transformation (if any)
        /// with the given compilation should complete without throwing an exception.
        /// The precondition verification should never throw an exception,
        /// but instead indicate if the precondition is satisfied via the returned value.
        /// More detailed information can be provided via logging.
        /// </summary>
        /// <param name="compilation">Q# compilation for which to verify the precondition.</param>
        /// <returns>Whether or not the given compilation satisfies the precondition.</returns>
        public bool PreconditionVerification(QsCompilation compilation);

        /// <summary>
        /// Implements a rewrite step transforming a Q# compilation.
        /// <see cref="ImplementsTransformation"/> indicates whether or not this method is implemented.
        /// The transformation should complete without throwing an exception
        /// if no precondition verification is implemented or the implemented verification passes.
        /// </summary>
        /// <param name="compilation">Q# compilation that satisfies the implemented precondition, if any.</param>
        /// <param name="transformed">Q# compilation after transformation. This value should not be null if the transformation succeeded.</param>
        /// <returns>Whether or not the transformation succeeded.</returns>
        public bool Transformation(QsCompilation compilation, out QsCompilation transformed);

        /// <summary>
        /// Verifies whether a given compilation satisfies the postcondition after executing the implemented transformation (if any).
        /// <see cref="ImplementsPostconditionVerification"/> indicates whether or not this method is implemented.
        /// The verification may be omitted for performance reasons depending on the build configuration.
        /// The postcondition verification should never throw an exception,
        /// but instead indicate if the postcondition is satisfied via the returned value.
        /// More detailed information can be displayed to the user by generating suitable diagnostics.
        /// </summary>
        /// <param name="compilation">Q# compilation after performing the implemented transformation.</param>
        /// <returns>Whether or not the given compilation satisfies the postcondition of the transformation.</returns>
        public bool PostconditionVerification(QsCompilation compilation);
    }
}
