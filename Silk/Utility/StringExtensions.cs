// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
namespace Silk
{
    public static class StringExtensions
    {
        public static string MakeQuoted(this string s) => $"\"{s}\"";
    }
}
