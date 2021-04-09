// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.ComponentModel;

namespace SoftCircuits.Silk
{
    public enum ErrorLevel
    {
        [Description("ERROR")]
        Error,
        [Description("FATAL ERROR")]
        FatalError,
    }

    public enum ErrorCode
    {
        [Description("Operation completed successfully")]
        NoError,
        [Description("Too many errors encountered")]
        TooManyErrors,
        [Description("Internal error")]
        InternalError,

        [Description("Code is not allowed outside of functions")]
        CodeOutsideFunction,
        [Description("More than one function defined with the same name")]
        DuplicateFunctionName,
        [Description("Label defined more than once")]
        DuplicateLabel,
        [Description("Function was not defined")]
        FunctionNotDefined,
        [Description("The VAR keyword can only appear within a function, or before the first function")]
        IllegalVar,
        [Description("STEP value must be a non-zero numeric literal")]
        InvalidStepValue,
        [Description("Label was referenced but never defined")]
        LabelNotDefined,
        [Description("Function main() was not defined : You must define main() as the program's starting point")]
        MainNotDefined,
        [Description("New line in string literal")]
        NewLineInString,
        [Description("Variable has already been defined")]
        VariableAlreadyDefined,
        [Description("Cannot change read-only variable")]
        AssignToReadOnlyVariable,
        [Description("Use of undefined variable")]
        VariableNotDefined,
        [Description("Wrong number of arguments")]
        WrongNumberOfArguments,

        [Description("Expected equal sign \"=\"")]
        ExpectedEquals,
        [Description("An expression was expected")]
        ExpectedExpression,
        [Description("Expected opening curly brace \"{\"")]
        ExpectedLeftBrace,
        [Description("Opening parenthesis expected \"(\"")]
        ExpectedLeftParen,
        [Description("Expected literal value")]
        ExpectedLiteral,
        [Description("Operand expected")]
        ExpectedOperand,
        [Description("Closing curly brace \"}\" expected")]
        ExpectedRightBrace,
        [Description("Closing parenthesis \")\" expected")]
        ExpectedRightParen,
        [Description("Closing square bracket \"]\" expected")]
        ExpectedRightBracket,
        [Description("Identifier name expected")]
        ExpectedSymbol,
        [Description("Expected TO keyword")]
        ExpectedTo,

        [Description("Break statement outside of loop")]
        BreakWithoutLoop,
        [Description("Continue statement outside of loop")]
        ContinueWithoutLoop,

        [Description("Unexpected character encountered")]
        UnexpectedCharacter,
        [Description("Keyword is unexpected here")]
        UnexpectedKeyword,
        [Description("Unexpected token encountered")]
        UnexpectedToken,
    }

    public class Error
    {
        public ErrorLevel Level { get; private set; }
        public ErrorCode Code { get; private set; }
        public string Description { get; private set; }
        public int Line { get; private set; }

        /// <summary>
        /// Constructs an Error instance.
        /// </summary>
        /// <param name="level">Error level.</param>
        /// <param name="code">Error code</param>
        /// <param name="line">Error line number.</param>
        internal Error(ErrorLevel level, ErrorCode code, int line)
        {
            Level = level;
            Code = code;
            Description = GetEnumDescription(code);
            Line = line;
        }

        /// <summary>
        /// Constructs an Error instance.
        /// </summary>
        /// <param name="level">Error level.</param>
        /// <param name="code">Error code</param>
        /// <param name="description">Additional description about error.</param>
        /// <param name="line">Error line number.</param>
        internal Error(ErrorLevel level, ErrorCode code, string description, int line)
        {
            if (description == null)
                throw new ArgumentNullException(nameof(description));
            Level = level;
            Code = code;
            Description = GetEnumDescription(code);
            Description += $" : {description}";
            Line = line;
        }

        /// <summary>
        /// Constructs an Error instance.
        /// </summary>
        /// <param name="level">Error level.</param>
        /// <param name="code">Error code</param>
        /// <param name="token">The token associated with this error.</param>
        internal Error(ErrorLevel level, ErrorCode code, Token token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));
            Level = level;
            Code = code;
            Description = GetEnumDescription(code);
            Description += $" : \"{token.Value}\"";
            Line = token.Line;
        }

        /// <summary>
        /// Constructs an Error instance.
        /// </summary>
        /// <param name="level">Error level.</param>
        /// <param name="code">Error code</param>
        /// <param name="description">Additional description about error.</param>
        /// <param name="token">The token associated with this error.</param>
        internal Error(ErrorLevel level, ErrorCode code, string description, Token token)
        {
            if (description == null)
                throw new ArgumentNullException(nameof(description));
            if (token == null)
                throw new ArgumentNullException(nameof(token));
            Level = level;
            Code = code;
            Description = GetEnumDescription(code);
            Description += $" : {description}";
            Description += $" : \"{token.Value}\"";
            Line = token.Line;
        }

        /// <summary>
        /// Returns a description of this error.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0} 1{1,03:D3} : {2} (Line {3})",
                GetEnumDescription(Level),
                (int)Code,
                Description,
                Line);
        }

        /// <summary>
        /// Converts an enum value to its associated description string.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <param name="value">The enum value.</param>
        private static string GetEnumDescription<T>(T value) where T : struct
        {
            var memberInfo = typeof(T).GetMember(value.ToString() ?? string.Empty);
            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                    return ((DescriptionAttribute)attributes[0]).Description;
            }
            return value.ToString() ?? $"[Unknown {typeof(T).Name}]";
        }
    }
}
