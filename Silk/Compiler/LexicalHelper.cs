// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SoftCircuits.Silk
{
    /// <summary>
    /// Provides low-level support for the lexical analyzer.
    /// </summary>
    internal class LexicalHelper
    {
        private LexicalAnalyzer Lexer;

        /// <summary>
        /// Represents an invalid character. This character is returned when a valid character
        /// is not available, such as when returning a character beyond the end of the text.
        /// The character is represented as <c>'\0'</c>.
        /// </summary>
        public const char NullChar = '\0';

        /// <summary>
        /// Returns the current text being parsed.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Returns the current position within the text being parsed.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// Returns the 1-based line number for the current position.
        /// </summary>
        public int Line { get; private set; }

        /// <summary>
        /// Indicates if the current position is at the end of the text being parsed.
        /// </summary>
        public bool EndOfText => (Index >= Text.Length);

        /// <summary>
        /// Returns the number of characters not yet parsed. This is equal to the length of the
        /// text being parsed minus the current position within that text.
        /// </summary>
        public int Remaining => (Text.Length - Index);


        /// <summary>
        /// Constructs a TextParse instance.
        /// </summary>
        /// <param name="text">Text to be parsed.</param>
        public LexicalHelper(LexicalAnalyzer lexer, string text = null)
        {
            Lexer = lexer;
            Reset(text);
        }

        /// <summary>
        /// Resets the current position to the start of the current text.
        /// </summary>
        public void Reset()
        {
            Index = 0;
            Line = 1;
        }

        /// <summary>
        /// Sets the text to be parsed and resets the current position to the start of that text.
        /// </summary>
        /// <param name="text">The text to be parsed.</param>
        public void Reset(string text)
        {
            Text = text ?? string.Empty;
            Index = 0;
            Line = 1;
        }

        /// <summary>
        /// Returns the character at the current position, or <c>NullChar</c> if we're
        /// at the end of the text being parsed.
        /// </summary>
        /// <returns>The character at the current position.</returns>
        public char Peek() => Peek(0);

        /// <summary>
        /// Returns the character at the specified number of characters beyond the current
        /// position, or <c>NullChar</c> if the specified position is at the end of the
        /// text being parsed.
        /// </summary>
        /// <param name="count">The number of characters beyond the current position.</param>
        /// <returns>The character at the specified position.</returns>
        public char Peek(int count)
        {
            int pos = (Index + count);
            return (pos < Text.Length) ? Text[pos] : NullChar;
        }
        
        /// <summary>
        /// Moves the current position ahead one character. The position will not
        /// be placed beyond the end of the text being parsed.
        /// </summary>
        public void MoveAhead()
        {
            if (Index < Text.Length)
            {
                if (Text[Index++] == '\n')
                    Line++;
            }
        }

        /// <summary>
        /// Moves the current position ahead the specified number of characters. The position
        /// will not be placed beyond the end of the text being parsed.
        /// </summary>
        /// <param name="count">The number of characters to move ahead</param>
        public void MoveAhead(int count)
        {
            int end = Math.Min(Index + count, Text.Length);
            while (Index < end)
            {
                if (Text[Index++] == '\n')
                    Line++;
            }
        }

        /// <summary>
        /// Moves to the next occurrence of the specified string within the text being parsed.
        /// Returns true if a match was found. Otherwise, false is returned.
        /// </summary>
        /// <param name="s">String to find.</param>
        public bool MoveTo(string s)
        {
            while (Index <= (Text.Length - s.Length))
            {
                if (MatchesCurrentPosition(s))
                    return true;
                if (Text[Index++] == '\n')
                    Line++;
            }
            return false;
        }

        /// <summary>
        /// Moves to the next occurrence of any one of the specified characters.
        /// Returns true if a match was found. Otherwise, false is returned.
        /// </summary>
        /// <param name="chars">Array of characters to search for.</param>
        public bool MoveTo(params char[] chars)
        {
            while (Index < Text.Length)
            {
                if (chars.Contains(Text[Index]))
                    return true;
                if (Text[Index++] == '\n')
                    Line++;
            }
            return false;
        }

        /// <summary>
        /// Moves the current position forward to the next newline character.
        /// </summary>
        public void MoveToEndOfLine() => MoveTo('\n');

        /// <summary>
        /// Moves the current position to the next character that is not whitespace. This
        /// method does not consider a new line character to be whitespace.
        /// </summary>
        public void MovePastWhitespace()
        {
            char c = Peek();
            while (char.IsWhiteSpace(c) && c != '\n')
            {
                MoveAhead();
                c = Peek();
            }
        }

        /// <summary>
        /// Parses characters until the next character that causes <paramref name="predicate"/> to
        /// return false, and then returns the characters spanned. Can return an empty string.
        /// </summary>
        /// <param name="predicate"></param>
        public string ParseWhile(Func<char, bool> predicate)
        {
            int start = Index;
            while (predicate(Peek()))
                MoveAhead();
            return Extract(start, Index);
        }

        private const char Escape = '\\';

        private static readonly Dictionary<char, char> EscapeCharacterLookup = new Dictionary<char, char>
        {
            ['t'] = '\t',
            ['r'] = '\r',
            ['n'] = '\n',
            ['\''] = '\'',
            ['"'] = '"',
            [Escape] = Escape,
        };

        /// <summary>
        /// Moves to the end of quoted text and returns the text within the quotes. Discards the
        /// quote characters. Assumes the current position is at the starting quote character.
        /// </summary>
        public string ParseQuotedText()
        {
            // Get quote character
            char quote = Peek();
            // Jump to start of quoted text
            MoveAhead();
            // Parse quoted text
            StringBuilder builder = new StringBuilder();
            while (!EndOfText)
            {
                char c = Peek();
                if (c == quote)
                {
                    // End of quoted string
                    MoveAhead();
                    break;
                }
                else if (c == '\r' || c == '\n')
                {
                    // Terminate string at newline
                    Lexer.OnError(ErrorCode.NewLineInString);
                    break;
                }
                else if (c == Escape)
                {
                    // Escaped character
                    MoveAhead();
                    if (!EndOfText)
                    {
                        c = Peek();
                        if (EscapeCharacterLookup.TryGetValue(c, out char replacement))
                        {
                            builder.Append(replacement);
                        }
                        else if (c == '\r' || c == '\n')
                        {
                            // Allow newline in string if escaped
                            builder.Append(c);
                            if (c == '\r' && Peek(1) == '\n')
                            {
                                MoveAhead();
                                builder.Append(Peek());
                            }
                        }
                        else
                        {
                            // Unknown escape sequence; just include entire sequence
                            builder.Append(Escape);
                            builder.Append(c);
                        }
                    }
                }
                else builder.Append(c);
                MoveAhead();
            }
            return builder.ToString();
        }

        /// <summary>
        /// Extracts a substring from the specified range of the text being parsed.
        /// </summary>
        /// <param name="start">0-based position of first character to extract.</param>
        /// <param name="end">0-based position of the character that follows the last
        /// character to extract.</param>
        /// <returns>Returns the extracted string</returns>
        public string Extract(int start, int end) => Text.Substring(start, end - start);

        #region Helper methods

        /// <summary>
        /// Compares the given string to text at the current position.
        /// </summary>
        /// <param name="s">String to compare.</param>
        private bool MatchesCurrentPosition(string s)
        {
            if (s.Length > Remaining)
                return false;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != Text[Index + i])
                    return false;
            }
            return true;
        }

        #endregion

        #region Operator overloads

        public static LexicalHelper operator ++(LexicalHelper parser)
        {
            parser.MoveAhead();
            return parser;
        }

        #endregion

    }
}
