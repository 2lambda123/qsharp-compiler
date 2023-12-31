﻿// -----------------------------------------------------------------------
// <copyright file="Cast.cs" company="Ubiquity.NET Contributors">
// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Portions Copyright (c) Microsoft Corporation
// </copyright>
// -----------------------------------------------------------------------

using LlvmBindings.Interop;
using LlvmBindings.Types;
using LlvmBindings.Values;

namespace LlvmBindings.Instructions
{
    /// <summary>Base class for cast instructions.</summary>
    public class Cast
        : UnaryInstruction
    {
        internal Cast(LLVMValueRef valueRef)
            : base(valueRef)
        {
        }

        /// <summary>Gets the source type of the cast.</summary>
        public ITypeRef FromType => this.Operands.GetOperand<Value>(0)!.NativeType;

        /// <summary>Gets the destination type of the cast.</summary>
        public ITypeRef ToType => this.NativeType;
    }
}
