﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Microsoft.Quantum.QsCompiler.SyntaxProcessing.CapabilityInference

open System
open Microsoft.Quantum.QsCompiler
open Microsoft.Quantum.QsCompiler.DataTypes
open Microsoft.Quantum.QsCompiler.SyntaxTree
open Microsoft.Quantum.QsCompiler.Transformations.Core

type Target =
    {
        capability: RuntimeCapability
        architecture: string
    }

    member target.Capability = target.capability

    member target.Architecture = target.architecture

module Target =
    [<CompiledName "Create">]
    let create capability architecture =
        { capability = capability; architecture = architecture }

type 'props Pattern =
    {
        Capability: RuntimeCapability
        Diagnose: Target -> QsCompilerDiagnostic option
        Properties: 'props // TODO: This should be removed in the future.
    }

module Pattern =
    let discard pattern =
        {
            Capability = pattern.Capability
            Diagnose = pattern.Diagnose
            Properties = ()
        }

    let max patterns =
        Seq.fold
            (fun capability pattern -> RuntimeCapability.merge pattern.Capability capability)
            RuntimeCapability.bottom
            patterns

type Analyzer<'subject, 'props> = 'subject -> 'props Pattern seq

module Analyzer =
    let concat (analyzers: Analyzer<_, _> seq) subject = Seq.collect ((|>) subject) analyzers

type LocatingTransformation(options) =
    inherit SyntaxTreeTransformation(options)

    let mutable absolute = Null
    let mutable relative = Null

    member _.Offset =
        match absolute, relative with
        | Value a, Value r -> a + r |> Value
        | Value a, Null -> Value a
        | Null, _ -> Null

    override _.OnAbsoluteLocation location =
        absolute <- location |> QsNullable<_>.Map (fun l -> l.Offset)
        relative <- Null
        location

    override _.OnRelativeLocation location =
        relative <- location |> QsNullable<_>.Map (fun l -> l.Offset)
        location

module ContextRef =
    let local value (context: _ ref) =
        let old = context.Value
        context.Value <- value

        { new IDisposable with
            member _.Dispose() = context.Value <- old
        }
