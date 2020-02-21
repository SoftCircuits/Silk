// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;

namespace SoftCircuits.Silk
{
    [Flags]
    internal enum ByteCodeVariableFlag
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
    internal enum ByteCode
    {
        Nop,
        ExecFunction,
        Return,
        Jump,
        Assign,
        AssignListVariable,
        JumpIfFalse,
        EvalLiteral,
        EvalVariable,
        EvalCreateList,
        EvalInitializeList,
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
    }

    /// <summary>
    /// Static helper methods
    /// </summary>
    internal static class ByteCodes
    {
        public const int InvalidIP = -1;

        public static bool IsGlobalVariable(int value) =>
            ((ByteCodeVariableFlag)value).HasFlag(ByteCodeVariableFlag.Global);
        public static bool IsLocalVariable(int value) =>
            ((ByteCodeVariableFlag)value).HasFlag(ByteCodeVariableFlag.Local);
        public static bool IsFunctionParameterVariable(int value) =>
            ((ByteCodeVariableFlag)value).HasFlag(ByteCodeVariableFlag.Parameter);
        public static int GetVariableIndex(int value) => (value & ~(int)ByteCodeVariableFlag.All);
    }
}
