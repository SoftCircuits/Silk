// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SoftCircuits.Silk
{
    public partial class Compiler
    {
        private enum ExpressionState
        {
            None,
            Operand,
            Operator,
            UnaryOperator,
        }

        /// <summary>
        /// Parses a literal value where a full expression is not allowed, such
        /// as in declarations within the header (before any functions). Logs
        /// an error and returns false if a literal value was not found.
        /// </summary>
        /// <param name="variable">Returns the resulting variable if method returns true.</param>
        /// <param name="allowLineBreak">If true, a new lines is allowed before literal.
        /// Set to true when parsing array initialization within curly braces.</param>
        internal bool ParseLiteral(out Variable variable, bool allowLineBreak = false)
        {
            variable = null;
            bool negate = false;

            Token token = allowLineBreak ? Lexer.GetNextSkipNewLines() : Lexer.GetNext();
            if (token.Type == TokenType.LeftBracket)
            {
                // List size
                if (!ParseLiteral(out variable))
                    return false;
                variable = Variable.CreateList(variable.ToInteger());
                // Parse right bracket
                token = Lexer.GetNext();
                if (token.Type != TokenType.RightBracket)
                {
                    Error(ErrorCode.ExpectedRightBracket, token);
                    NextLine();
                    return false;
                }
                return true;
            }
            if (token.Type == TokenType.LeftBrace)
            {
                // List initializers
                List<Variable> list = new List<Variable>();
                do
                {
                    if (!ParseLiteral(out variable, true))
                        return false;
                    list.Add(variable);
                    token = Lexer.GetNextSkipNewLines();
                } while (token.Type == TokenType.Comma);
                // Parse right brace
                if (token.Type != TokenType.RightBrace)
                {
                    Error(ErrorCode.ExpectedRightBrace, token);
                    NextLine();
                    return false;
                }
                variable = new Variable(list);
                return true;
            }
            if (token.Type == TokenType.Minus || token.Type == TokenType.Plus)
            {
                negate = (token.Type == TokenType.Minus);
                token = Lexer.GetNext();
            }
            if (token.IsLiteral)
            {
                variable = new Variable(token);
                if (negate)
                    variable = variable.Negate();
                return true;
            }
            Error(ErrorCode.ExpectedLiteral, token);
            return false;
        }

        /// <summary>
        /// Parses an expression. Designed to handle case where there is no expression
        /// at the current position. Returns false if error was logged or no expression
        /// was found.
        /// </summary>
        /// <param name="required">If true, an error is logged if no expression found.</param>
        /// <param name="expressionExtender">Function to add additional bytecodes to expression.
        /// Will be included in token count.</param>
        /// <param name="allowLineBreak">If true, a new line is allowed before this expression.
        /// Set to true when parsing array initialization within curly braces.</param>
        internal bool ParseExpression(bool required = true, Action expressionExtender = null, bool allowLineBreak = false)
        {
            int tokenCountIP;

            Token token = allowLineBreak ? Lexer.GetNextSkipNewLines() : Lexer.GetNext();
            if (token.Type == TokenType.LeftBracket)
            {
                // List creation: a = [20]
                Writer.Write(1);
                Writer.Write(ByteCode.EvalCreateList);
                if (!ParseExpression())
                    return false;
                token = Lexer.GetNext();
                if (token.Type != TokenType.RightBracket)
                {
                    Error(ErrorCode.ExpectedRightBracket, token);
                    return false;
                }
                return true;
            }
            if (token.Type == TokenType.LeftBrace)
            {
                // List initialization: a = { "abc", "def" }
                Writer.Write(1);
                Writer.Write(ByteCode.EvalInitializeList);
                tokenCountIP = Writer.Write(0);

                int count = 0;
                do
                {
                    if (!ParseExpression(true, null, true))
                        return false;
                    count++;
                    token = Lexer.GetNextSkipNewLines();
                } while (token.Type == TokenType.Comma);
                // Parse right brace
                if (token.Type != TokenType.RightBrace)
                {
                    Error(ErrorCode.ExpectedRightBrace, token);
                    NextLine();
                    return false;
                }
                Writer.WriteAt(tokenCountIP, count);
                return true;
            }

            Lexer.UngetToken(token);
            // Write placeholder for postfix token count
            tokenCountIP = Writer.Write(0);
            Writer.ResetCounter();
            // Build list of tokens in postfix order
            if (!WritePostfixTokens())
                return false;
            // Were any expression tokens found?
            if (Writer.Counter > 0)
            {
                // Allow caller to extend this expression
                expressionExtender?.Invoke();
                // Write actual token count over placeholder
                Writer.WriteAt(tokenCountIP, Writer.Counter);
                // Expression parsed
                return true;
            }
            else
            {
                // Delete placeholder
                Writer.UndoLastWrite();
                if (required)
                    Error(ErrorCode.ExpectedExpression, Lexer.GetNext());
                return false;
            }
        }

        /// <summary>
        /// Parses tokens and writes a list of tokens in postfix order.
        /// </summary>
        /// <returns>False if an error occurred; true otherwise.</returns>
        private bool WritePostfixTokens()
        {
            Stack<Token> operatorStack = new Stack<Token>();
            ExpressionState state = ExpressionState.None;
            int parenCount = 0;

            // Convert tokens from infix to postfix order
            while (true)
            {
                var token = Lexer.GetNext();

                if (token.IsLiteral)
                {
                    // Assume end of expression if two operands in a row
                    if (state == ExpressionState.Operand)
                    {
                        Lexer.UngetToken(token);
                        break;
                    }
                    // Literal operand
                    Writer.Write(ByteCode.EvalLiteral, GetLiteralId(new Variable(token)));
                    state = ExpressionState.Operand;
                }
                else if (token.Type == TokenType.Symbol)
                {
                    // Assume end of expression if two operands in a row
                    if (state == ExpressionState.Operand)
                    {
                        Lexer.UngetToken(token);
                        break;
                    }
                    // Variable or function operand
                    if (Lexer.PeekNext().Type == TokenType.LeftParen)
                    {
                        // Consume left parenthesis
                        Lexer.GetNext();
                        // Function call
                        int functionId = GetFunctionId(token.Value);
                        Writer.Write(ByteCode.EvalFunction, functionId);
                        // Possible recursion!
                        Writer.PushCounter();
                        Function function = Functions[functionId];
                        bool result = ParseFunctionArguments(function, true);
                        Writer.PopCounter();
                        if (!result)
                            return false;
                    }
                    else if (Lexer.PeekNext().Type == TokenType.LeftBracket)
                    {
                        // Consume left bracket
                        Lexer.GetNext();
                        // List item
                        int varId = GetVariableId(token.Value, false);
                        if (varId < 0)
                        {
                            Error(ErrorCode.VariableNotDefined, token);
                            return false;
                        }
                        Writer.Write(ByteCode.EvalListVariable, varId);
                        // Recursion
                        Writer.PushCounter();
                        bool result = ParseExpression();
                        Writer.PopCounter();
                        if (!result)
                            return false;
                        token = Lexer.GetNext();
                        if (token.Type != TokenType.RightBracket)
                        {
                            Error(ErrorCode.ExpectedRightBracket, token);
                            return false;
                        }
                    }
                    else
                    {
                        // Variable must exist when evaluating
                        int varId = GetVariableId(token.Value, false);
                        if (varId < 0)
                        {
                            Error(ErrorCode.VariableNotDefined, token);
                            return false;
                        }
                        Writer.Write(ByteCode.EvalVariable, varId);
                    }
                    state = ExpressionState.Operand;
                }
                else if (token.IsOperator)
                {
                    if (state == ExpressionState.Operand)
                    {
                        // Pop operators with precedence >= this operator
                        int precedence = OperatorPrecedence[token.Type];
                        while (operatorStack.Count > 0 && OperatorPrecedence[operatorStack.Peek().Type] >= precedence)
                            Writer.Write(operatorStack.Pop().GetOperatorByteCode());
                        operatorStack.Push(token);
                        state = ExpressionState.Operator;
                    }
                    else if (state == ExpressionState.UnaryOperator)
                    {
                        // Don't allow two unary operators together
                        Error(ErrorCode.ExpectedOperand, token);
                        return false;
                    }
                    else
                    {
                        if (token.Type == TokenType.Plus || token.Type == TokenType.Minus)
                        {
                            if (token.Type == TokenType.Minus)
                            {
                                token.Type = TokenType.UnaryMinus;
                                operatorStack.Push(token);
                            }
                            state = ExpressionState.UnaryOperator;
                        }
                        else if (token.Type == TokenType.Not)
                        {
                            operatorStack.Push(token);
                            state = ExpressionState.UnaryOperator;
                        }
                        else
                        {
                            Error(ErrorCode.ExpectedOperand, token);
                            return false;
                        }
                    }
                }
                else if (token.Type == TokenType.LeftParen)
                {
                    // Assume end of expression if following operand
                    if (state == ExpressionState.Operand)
                    {
                        Lexer.UngetToken(token);
                        break;
                    }
                    operatorStack.Push(token);
                    parenCount++;
                }
                else if (token.Type == TokenType.RightParen && parenCount > 0)
                {
                    if (state != ExpressionState.Operand)
                    {
                        Error(ErrorCode.ExpectedOperand, token);
                        return false;
                    }

                    //
                    while (operatorStack.Count > 0)
                    {
                        token = operatorStack.Pop();
                        if (token.Type == TokenType.LeftParen)
                            break;
                        Writer.Write(token.GetOperatorByteCode());
                    }
                    parenCount--;
                }
                else
                {
                    // Stop parsing expression at unexpected token
                    Lexer.UngetToken(token);
                    break;
                }
            }

            // Additional error checking
            if (state == ExpressionState.Operator || state == ExpressionState.UnaryOperator)
            {
                Error(ErrorCode.ExpectedOperand);
                return false;
            }
            if (parenCount > 0)
            {
                Error(ErrorCode.ExpectedRightParen);
                return false;
            }
            // Add any remaining operators
            while (operatorStack.Count > 0)
            {
                var token = operatorStack.Pop();
                Debug.Assert(token.Type != TokenType.LeftParen && token.Type != TokenType.RightParen);
                if (token.Type != TokenType.LeftParen && token.Type != TokenType.RightParen)
                    Writer.Write(token.GetOperatorByteCode());
            }
            return true;
        }

        private static Dictionary<TokenType, int> OperatorPrecedence = new Dictionary<TokenType, int>
        {
            [TokenType.LeftParen] = 0,
            [TokenType.RightParen] = 0,
            [TokenType.And] = 1,
            [TokenType.Or] = 1,
            [TokenType.Xor] = 1,
            [TokenType.Equal] = 2,
            [TokenType.NotEqual] = 2,
            [TokenType.GreaterThan] = 3,
            [TokenType.GreaterThanOrEqual] = 3,
            [TokenType.LessThan] = 3,
            [TokenType.LessThanOrEqual] = 3,
            [TokenType.Plus] = 4,
            [TokenType.Minus] = 4,
            [TokenType.Concat] = 4,
            [TokenType.Multiply] = 5,
            [TokenType.Divide] = 5,
            [TokenType.Modulus] = 5,
            [TokenType.Power] = 5,
            [TokenType.UnaryMinus] = 6,
            [TokenType.Not] = 6,
        };

        //private Variable EvaluateTokens(List<Token> postfixList)
        //{
        //    Stack<Variable> stack = new Stack<Variable>();

        //    foreach (var token in postfixList)
        //    {
        //        if (token.IsLiteral)
        //        {
        //            stack.Push(new Variable(token));
        //        }
        //        else if (token.Type == TokenType.UnaryMinus)
        //        {
        //            Variable var = stack.Pop();
        //            stack.Push(var.Negate());
        //        }
        //        else
        //        {
        //            var calc = CalcLookup[token.Type];
        //            Variable var2 = stack.Pop();
        //            Variable var1 = stack.Pop();
        //            stack.Push(calc(var1, var2));
        //        }
        //    }
        //    // Remaining item on stack contains result
        //    Debug.Assert(stack.Count == 1);
        //    return (stack.Count > 0) ? stack.Pop() : new Variable(0);
        //}

        //private static Dictionary<TokenType, Func<Variable, Variable, Variable>> CalcLookup = new Dictionary<TokenType, Func<Variable, Variable, Variable>>
        //{
        //    [TokenType.Plus] = (v1, v2) => v1.Add(v2),
        //    [TokenType.Minus] = (v1, v2) => v1.Subtract(v2),
        //    [TokenType.Multiply] = (v1, v2) => v1.Multiply(v2),
        //    [TokenType.Divide] = (v1, v2) => v1.Divide(v2),
        //    [TokenType.Power] = (v1, v2) => v1.Power(v2),
        //    [TokenType.Modulus] = (v1, v2) => v1.Modulus(v2),
        //    [TokenType.Concat] = (v1, v2) => v1.Concat(v2),
        //};

    }
}
