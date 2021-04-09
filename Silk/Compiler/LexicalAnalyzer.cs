// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;

namespace SoftCircuits.Silk
{
    /// <summary>
    /// Converts a string of characters to tokens.
    /// </summary>
    internal class LexicalAnalyzer
    {
        // Symbol characters
        public static readonly HashSet<char> SymbolFirstChars = new("_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");
        public static readonly HashSet<char> SymbolChars = new("_abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

        // Operator lookup table
        public static readonly Dictionary<char, Operator> OperatorLookup = new()
        {
            ['+'] = new Operator('+', TokenType.Plus),
            ['-'] = new Operator('-', TokenType.Minus),
            ['*'] = new Operator('*', TokenType.Multiply),
            ['/'] = new Operator('/', TokenType.Divide,
                new Operator('/', TokenType.LineComment),
                new Operator('*', TokenType.CommentStart)),
            ['^'] = new Operator('^', TokenType.Power),
            ['%'] = new Operator('%', TokenType.Modulus),
            ['&'] = new Operator('&', TokenType.Concat),
            ['='] = new Operator('=', TokenType.Equal),
            ['<'] = new Operator('<', TokenType.LessThan,
                new Operator('>', TokenType.NotEqual),
                new Operator('=', TokenType.LessThanOrEqual)),
            ['>'] = new Operator('>', TokenType.GreaterThan,
                new Operator('=', TokenType.GreaterThanOrEqual)),
            ['('] = new Operator('(', TokenType.LeftParen),
            [')'] = new Operator(')', TokenType.RightParen),
            ['{'] = new Operator('{', TokenType.LeftBrace),
            ['}'] = new Operator('}', TokenType.RightBrace),
            ['['] = new Operator('[', TokenType.LeftBracket),
            [']'] = new Operator(']', TokenType.RightBracket),
            [','] = new Operator(',', TokenType.Comma),
            [':'] = new Operator(':', TokenType.Colon),
        };

        //
        public static readonly Dictionary<string, TokenType> SymbolOperatorLookup = new(StringComparer.OrdinalIgnoreCase)
        {
            ["and"] = TokenType.And,
            ["or"] = TokenType.Or,
            ["xor"] = TokenType.Xor,
            ["not"] = TokenType.Not,
        };

        // Multiline comment terminator
        private static readonly string CommentEnd = "*/";

        private LexicalHelper LexHelper;
        private Token? SavedUngetToken;

        public int LastTokenLine;
        public int CurrentLine => LexHelper.Line;

        /// <summary>
        /// Returns the text being parsed.
        /// </summary>
        public string Text => LexHelper.Text;

        public LexicalAnalyzer(string? source = null)
        {
            LexHelper = new LexicalHelper(this);
            Reset(source);
        }

        public void Reset(string? source = null)
        {
            LexHelper.Reset(source);
            SavedUngetToken = null;
        }

        public Token PeekNext()
        {
            SavedUngetToken = GetNext();
            return SavedUngetToken;
        }

        /// <summary>
        /// Returns the next token that is not a newline.
        /// </summary>
        /// <returns></returns>
        public Token GetNextSkipNewLines()
        {
            Token token;
            do
            {
                token = GetNext();
            } while (token.Type == TokenType.EndOfLine);
            return token;
        }

        public Token GetNext()
        {
            TokenType tokenType;
            string tokenValue;
            int tokenStart;
            int tokenLine;

            // Return saved "unget" token, if any
            if (SavedUngetToken != null)
            {
                var token = SavedUngetToken;
                SavedUngetToken = null;
                return token;
            }

        ParseNextToken:

            // Skip any whitespace (does not skip newlines)
            LexHelper.MovePastWhitespace();

            //
            LastTokenLine = CurrentLine;

            // Test for end of input text
            if (LexHelper.EndOfText)
                return new Token(TokenType.EndOfFile, "<EndOfFile>", LexHelper.Line);

            // Peek next character
            char c = LexHelper.Peek();
            tokenStart = LexHelper.Index;
            tokenLine = LexHelper.Line;

            // Symbol (keyword, symbol, function or character-based operator)
            if (SymbolFirstChars.Contains(c))
            {
                tokenValue = LexHelper.ParseWhile(ch => SymbolChars.Contains(ch));
                if (SymbolOperatorLookup.TryGetValue(tokenValue, out tokenType))
                    return new Token(tokenType, tokenValue, tokenLine);
                else if (Keywords.IsKeyword(tokenValue, out Keyword tokenKeyword))
                    return new Token(TokenType.Keyword, tokenKeyword, tokenValue, tokenLine);
                else
                    return new Token(TokenType.Symbol, tokenValue, tokenLine);
            }

            // Number
            if (char.IsDigit(c) || c == '.')
            {
                int count;
                bool gotDigit = false;
                tokenType = TokenType.Integer;

                if (c == '0' && LexHelper.Peek(1) == 'x')
                {
                    // Hexadecimal literal
                    count = 2;
                    while (LexHelper.Peek(count).IsHexDigit())
                        count++;
                    if (count > 2)
                    {
                        gotDigit = true;
                        tokenType = TokenType.Integer;
                    }
                }
                else
                {
                    // Decimal literal
                    count = 1;
                    bool gotDecimal = (c == '.');
                    gotDigit = char.IsDigit(c);

                    while (true)
                    {
                        char ch = LexHelper.Peek(count);
                        if (ch == '.')
                        {
                            if (gotDecimal)
                                break;
                            gotDecimal = true;
                        }
                        else if (char.IsDigit(ch))
                        {
                            gotDigit = true;
                        }
                        else break;
                        count++;
                    }
                    if (gotDigit)
                        tokenType = gotDecimal ? TokenType.Float : TokenType.Integer;
                }
                if (gotDigit)
                {
                    LexHelper.MoveAhead(count);
                    return new Token(tokenType, LexHelper.Extract(tokenStart, LexHelper.Index), tokenLine);
                }
            }

            // String
            if (c == '"' || c == '\'')
                return new Token(TokenType.String, LexHelper.ParseQuotedText(), tokenLine);

            // Operator
            if (OperatorLookup.TryGetValue(c, out Operator? info))
            {
                LexHelper++;
                char ch = LexHelper.Peek();
                foreach (var secondaryInfo in info.SecondaryChars)
                {
                    if (ch == secondaryInfo.Char)
                    {
                        LexHelper++;
                        // If comments, just consume them
                        if (ConsumeComment(secondaryInfo.Type))
                            goto ParseNextToken;
                        return new Token(secondaryInfo.Type, LexHelper.Extract(tokenStart, LexHelper.Index), tokenLine);
                    }
                }
                return new Token(info.Type, LexHelper.Extract(tokenStart, LexHelper.Index), tokenLine);
            }

            if (c == '\n')
            {
                LexHelper++;
                return new Token(TokenType.EndOfLine, "<EndOfLine>", tokenLine);
            }

            // We don't know what to do with this character
            LexHelper++;
            OnError(ErrorCode.UnexpectedCharacter, c.MakeQuoted());
            goto ParseNextToken;
        }

        /// <summary>
        /// If the specified token type is a comment, this method consumes the comment
        /// and returns true. Otherwise, false is returned.
        /// </summary>
        /// <param name="type">The type of the current token.</param>
        private bool ConsumeComment(TokenType type)
        {
            // Consume comments
            if (type == TokenType.CommentStart)
            {
                LexHelper.MoveTo(CommentEnd);
                LexHelper.MoveAhead(CommentEnd.Length);
                return true;
            }
            else if (type == TokenType.LineComment)
            {
                LexHelper.MoveToEndOfLine();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Ungets the given token so that it will be returned on the next call to GetNext().
        /// Only one token is cached. If this method is called multiple times without
        /// intervening calls to GetNext(), only the last specified will be saved.
        /// </summary>
        /// <param name="token">Token to return on next call to GetNext().</param>
        public void UngetToken(Token token)
        {
            SavedUngetToken = token;
        }

        public override string ToString() => $"Index {LexHelper.Index}, Line {LexHelper.Line}";

        #region Events

        public event EventHandler<ErrorEventArgs>? Error;

        internal void OnError(ErrorCode code, string? token = null)
        {
            ErrorEventArgs e = new()
            {
                ErrorCode = code,
                Token = token
            };
            Error?.Invoke(this, e);
        }

        #endregion

    }
}
