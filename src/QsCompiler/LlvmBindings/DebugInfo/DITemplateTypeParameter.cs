﻿// -----------------------------------------------------------------------
// <copyright file="DITemplateTypeParameter.cs" company="Ubiquity.NET Contributors">
// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Portions Copyright (c) Microsoft Corporation
// </copyright>
// -----------------------------------------------------------------------

using LlvmBindings.Interop;

namespace LlvmBindings.DebugInfo
{
    /// <summary>Template type parameter</summary>
    /// <seealso href="xref:llvm_langref#ditemplatetypeparameter">LLVM DITemplateTypeParameter</seealso>
    public class DITemplateTypeParameter
        : DITemplateParameter
    {
        internal DITemplateTypeParameter(LLVMMetadataRef handle)
            : base(handle)
        {
        }
    }
}
