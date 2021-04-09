// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SoftCircuits.Silk
{
    /// <summary>
    /// Enum of supported keywords.
    /// </summary>
    internal enum Keyword
    {
        NullKeyword,    // Not a keyword
        Var,
        GoTo,
        Return,
        If,
        Else,
        While,
        For,
        Break,
        Continue,
        // Keywords part of other statements/expressions
        And,
        Or,
        Xor,
        Not,
        To,
        Step,
    }

    /// <summary>
    /// Static class for tracking and looking up keywords.
    /// </summary>
    internal static class Keywords
    {
        private static readonly Dictionary<string, Keyword> KeywordLookup = new(StringComparer.OrdinalIgnoreCase)
        {
            ["var"] = Keyword.Var,
            ["goto"] = Keyword.GoTo,
            ["return"] = Keyword.Return,
            ["if"] = Keyword.If,
            ["else"] = Keyword.Else,
            ["while"] = Keyword.While,
            ["for"] = Keyword.For,
            ["break"] = Keyword.Break,
            ["continue"] = Keyword.Continue,
            ["to"] = Keyword.To,
            ["step"] = Keyword.Step,
        };

        /// <summary>
        /// Returns true if the given string is a keyword.
        /// </summary>
        public static bool IsKeyword(string name)
        {
            Debug.Assert(IsValidSymbolName(name));
            return KeywordLookup.ContainsKey(name);
        }

        /// <summary>
        /// If <paramref name="name"/> is a keyword, this method returns true and sets
        /// <paramref name="keyword"/> to the corresponding <see>Keyword</see> value.
        /// Otherwise, this method returns false.
        /// </summary>
        public static bool IsKeyword(string name, out Keyword keyword)
        {
            Debug.Assert(IsValidSymbolName(name));
            return KeywordLookup.TryGetValue(name, out keyword);
        }

        /// <summary>
        /// Returns the keyword ID associated with the given keyword.
        /// Throws an exception if the string is not a keyword.
        /// </summary>
        public static Keyword LookupKeyword(string name)
        {
            Debug.Assert(IsValidSymbolName(name));
            Debug.Assert(KeywordLookup.ContainsKey(name));
            return KeywordLookup[name];
        }

        /// <summary>
        /// Tests if all characters in the given name are valid identifier characters.
        /// </summary>
        public static bool IsValidSymbolName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            if (!LexicalAnalyzer.SymbolFirstChars.Contains(name[0]))
                return false;
            for (int i = 1; i < name.Length; i++)
            {
                if (!LexicalAnalyzer.SymbolChars.Contains(name[i]))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Tests if a name string is a keyword or equal to "main".
        /// </summary>
        public static bool IsReservedSymbol(string name)
        {
            Debug.Assert(IsValidSymbolName(name));
            return IsKeyword(name) ||
                (name.Equals(Function.Main, StringComparison.OrdinalIgnoreCase));
        }
    }
}
