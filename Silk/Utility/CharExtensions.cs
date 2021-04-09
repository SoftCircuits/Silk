// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
namespace SoftCircuits.Silk
{
    public static class CharExtensions
    {
        public static string MakeQuoted(this char c) => $"\"{c}\"";

        public static bool IsHexDigit(this char c) => char.IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');

        //public static bool IsOctDigit(this char c) => c >= 0 && c <= 7;
    }
}
