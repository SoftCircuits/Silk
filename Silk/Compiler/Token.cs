// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SoftCircuits.Silk
{
    [Flags]
    internal enum TokenTypeFlag
    {
        Literal = 0x10000000,
        Operator = 0x20000000,
    }

    [Flags]
    internal enum TokenType
    {
        EndOfFile = 0,
        EndOfLine = 1,

        Keyword = 10,
        Symbol = 11,
        String = 12 | TokenTypeFlag.Literal,
        Integer = 13 | TokenTypeFlag.Literal,
        Float = 14 | TokenTypeFlag.Literal,

        Plus = 20 | TokenTypeFlag.Operator,
        Minus = 21 | TokenTypeFlag.Operator,
        Multiply = 22 | TokenTypeFlag.Operator,
        Divide = 23 | TokenTypeFlag.Operator,
        Power = 24 | TokenTypeFlag.Operator,
        Modulus = 25 | TokenTypeFlag.Operator,
        Concat = 26 | TokenTypeFlag.Operator,
        UnaryMinus = 27 | TokenTypeFlag.Operator,    // Used by expression parser

        And = 30 | TokenTypeFlag.Operator,
        Or = 31 | TokenTypeFlag.Operator,
        Xor = 32 | TokenTypeFlag.Operator,
        Not = 33 | TokenTypeFlag.Operator,

        Equal = 40 | TokenTypeFlag.Operator,         // Equality and assignment
        NotEqual = 41 | TokenTypeFlag.Operator,
        GreaterThan = 42 | TokenTypeFlag.Operator,
        GreaterThanOrEqual = 43 | TokenTypeFlag.Operator,
        LessThan = 44 | TokenTypeFlag.Operator,
        LessThanOrEqual = 45 | TokenTypeFlag.Operator,

        LeftParen = 60,
        RightParen = 61,
        LeftBrace = 62,
        RightBrace = 63,
        LeftBracket = 64,
        RightBracket = 65,
        Comma = 66,
        Colon = 67,

        CommentStart = 90,
        CommentEnd = 91,
        LineComment = 92,
    }

    internal class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; private set; }
        public Keyword Keyword { get; set; }
        public int Line { get; private set; }

        /// <summary>
        /// Constructs a non-keyword token.
        /// </summary>
        /// <param name="type">This token's type.</param>
        /// <param name="value">The original value for this token.</param>
        /// <param name="line">Line number where the token was found.</param>
        public Token(TokenType type, string value, int line)
        {
            Debug.Assert(type != TokenType.Keyword);
            Type = type;
            Keyword = Keyword.NullKeyword;
            Value = value;
            Line = line;
        }

        /// <summary>
        /// Constructs a keyword token.
        /// </summary>
        /// <param name="type">This token's type.</param>
        /// <param name="keyword">Corresponding keyword if <paramref name="type"/> is a keyword.</param>
        /// <param name="value">The original value for this token.</param>
        /// <param name="line">Line number where the token was found.</param>
        public Token(TokenType type, Keyword keyword, string value, int line)
        {
            Debug.Assert(type == TokenType.Keyword);
            Type = type;
            Keyword = keyword;
            Value = value;
            Line = line;
        }

        /// <summary>
        /// Returns true if this token is a literal.
        /// </summary>
        public bool IsLiteral => Type.HasFlag((TokenType)TokenTypeFlag.Literal);

        /// <summary>
        /// Returns true if this token is an operator.
        /// </summary>
        public bool IsOperator => Type.HasFlag((TokenType)TokenTypeFlag.Operator);

        /// <summary>
        /// TokenType to ByteCode lookup. Should include all token types that are operators.
        /// </summary>
        private static readonly Dictionary<TokenType, ByteCode> ByteCodeLookup = new()
        {
            [TokenType.Plus] = ByteCode.EvalAdd,
            [TokenType.Minus] = ByteCode.EvalSubtract,
            [TokenType.Multiply] = ByteCode.EvalMultiply,
            [TokenType.Divide] = ByteCode.EvalDivide,
            [TokenType.Power] = ByteCode.EvalPower,
            [TokenType.Modulus] = ByteCode.EvalModulus,
            [TokenType.Concat] = ByteCode.EvalConcat,
            [TokenType.UnaryMinus] = ByteCode.EvalNegate,
            [TokenType.And] = ByteCode.EvalAnd,
            [TokenType.Or] = ByteCode.EvalOr,
            [TokenType.Xor] = ByteCode.EvalXor,
            [TokenType.Not] = ByteCode.EvalNot,
            [TokenType.Equal] = ByteCode.EvalIsEqual,
            [TokenType.NotEqual] = ByteCode.EvalIsNotEqual,
            [TokenType.GreaterThan] = ByteCode.EvalIsGreaterThan,
            [TokenType.GreaterThanOrEqual] = ByteCode.EvalIsGreaterThanOrEqual,
            [TokenType.LessThan] = ByteCode.EvalIsLessThan,
            [TokenType.LessThanOrEqual] = ByteCode.EvalIsLessThanOrEqual,
        };

        public ByteCode GetOperatorByteCode()
        {
            if (!IsOperator)
                throw new Exception($"GetOperatorByteCode() called on non-operator ({Type}) token.");
            if (ByteCodeLookup.TryGetValue(Type, out ByteCode bytecode))
                return bytecode;
            throw new Exception($"GetOperatorByteCode() : No bytecode for operator {Type}");
        }

        /// <summary>
        /// Primarily used for debugging purposes.
        /// </summary>
        public override string ToString() => $"\"{Value}\" ({Type} on line {Line})";
    }
}
