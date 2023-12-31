﻿// -----------------------------------------------------------------------
// <copyright file="DILexicalBlockBase.cs" company="Ubiquity.NET Contributors">
// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Portions Copyright (c) Microsoft Corporation
// </copyright>
// -----------------------------------------------------------------------

using LlvmBindings.Interop;

namespace LlvmBindings.DebugInfo
{
    /// <summary>Base for lexical blocks</summary>
    public class DILexicalBlockBase
        : DILocalScope
    {
        internal DILexicalBlockBase(LLVMMetadataRef handle)
            : base(handle)
        {
        }
    }
}
