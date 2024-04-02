// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
namespace SoftCircuits.Silk
{
    /// <summary>
    /// Extensions to the <see cref="System.Char"/> class.
    /// </summary>
    public static class CharExtensions
    {
        /// <summary>
        /// Wraps a character in quotes.
        /// </summary>
        /// <param name="c"></param>
        public static string MakeQuoted(this char c) => $"\"{c}\"";

        /// <summary>
        /// Determines whether this character is a hexadecimal digit.
        /// </summary>
        /// <param name="c"></param>
        public static bool IsHexDigit(this char c) => char.IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');

        ///// <summary>
        ///// Determines whether this character is an octal digit.
        ///// </summary>
        ///// <param name="c"></param>
        //public static bool IsOctDigit(this char c) => c >= 0 && c <= 7;
    }
}
