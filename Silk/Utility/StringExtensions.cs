// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
namespace SoftCircuits.Silk
{
    /// <summary>
    /// Extensions to the <see cref="System.String"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Wraps this string in quotes.
        /// </summary>
        /// <param name="s"></param>
        public static string MakeQuoted(this string s) => $"\"{s}\"";
    }
}
