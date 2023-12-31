﻿// -----------------------------------------------------------------------
// <copyright file="DIExpression.cs" company="Ubiquity.NET Contributors">
// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Portions Copyright (c) Microsoft Corporation
// </copyright>
// -----------------------------------------------------------------------

using LlvmBindings.Interop;

namespace LlvmBindings.DebugInfo
{
    /// <summary>Debug information expression</summary>
    /// <seealso href="xref:llvm_langref#diexpression">LLVM DIExpression</seealso>
    public class DIExpression
        : MDNode
    {
        internal DIExpression(LLVMMetadataRef handle)
            : base(handle)
        {
        }
    }
}
