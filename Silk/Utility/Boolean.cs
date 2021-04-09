// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;

namespace SoftCircuits.Silk
{
    /// <summary>
    /// Support for SILK language Boolean values.
    /// </summary>
    /// <remarks>
    /// Like older compiled BASICs (and unlike C#), this language has only one version of
    /// AND, OR, etc. operators, and they are all bitwise operators. So when using them
    /// in logical expressions, we get better results if TRUE is -1 (all bits set).
    /// </remarks>
    internal class Boolean
    {
        public const int True = -1;
        public const int False = 0;

        public const string TrueString = "True";
        public const string FalseString = "False";

        public static bool IsTrue(int value) => value != False;
        public static bool IsFalse(int value) => value == False;

        public static bool IsTrue(double value) => value != False;
        public static bool IsFalse(double value) => value == False;

        public static bool IsTrue(string value)
        {
            if (value.Equals(TrueString, StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                return true;
            if (int.TryParse(value, out int result) && result != 0)
                return true;
            return false;
        }
        public static bool IsFalse(string value) => !IsTrue(value);
    }
}
