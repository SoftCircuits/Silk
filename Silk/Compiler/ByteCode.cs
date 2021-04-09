// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;

namespace SoftCircuits.Silk
{
    [Flags]
    internal enum ByteCodeVariableType
    {
        None = 0x00000000,
        Global = 0x10000000,
        Local = 0x20000000,
        Parameter = 0x40000000,
        All = Global | Local | Parameter,
    }

    /// <summary>
    /// Byte code values.
    /// </summary>
    /// <remarks>
    /// Note: New byte codes must be added at the end to avoid changing values
    /// of existing codes, which would break running previously compiled programs.
    /// </remarks>
    internal enum ByteCode
    {
        Nop,
        ExecFunction,
        Return,
        Jump,
        Assign,
        // Retained for backwards compatibility
        // but now AssignListVariableMulti is used,
        // which supports multiple indexes.
        AssignListVariable,
        JumpIfFalse,
        EvalLiteral,
        EvalVariable,
        EvalCreateList,
        EvalInitializeList,
        // Retained for backwards compatibility
        // but now EvalListVariableMulti is used,
        // which supports multiple indexes.
        EvalListVariable,
        EvalFunction,
        EvalAdd,
        EvalSubtract,
        EvalMultiply,
        EvalDivide,
        EvalPower,
        EvalModulus,
        EvalConcat,
        EvalNegate,
        EvalAnd,
        EvalOr,
        EvalXor,
        EvalNot,
        EvalIsEqual,
        EvalIsNotEqual,
        EvalIsGreaterThan,
        EvalIsGreaterThanOrEqual,
        EvalIsLessThan,
        EvalIsLessThanOrEqual,
        AssignListVariableMulti,
        EvalListVariableMulti,
    }

    /// <summary>
    /// Static helper methods
    /// </summary>
    internal static class ByteCodes
    {
        public const int InvalidIP = -1;

        public static bool IsGlobalVariable(int value) => ((ByteCodeVariableType)value).HasFlag(ByteCodeVariableType.Global);
        public static bool IsLocalVariable(int value) => ((ByteCodeVariableType)value).HasFlag(ByteCodeVariableType.Local);
        public static bool IsFunctionParameterVariable(int value) => ((ByteCodeVariableType)value).HasFlag(ByteCodeVariableType.Parameter);
        public static int GetVariableIndex(int value) => (value & ~(int)ByteCodeVariableType.All);
    }
}
