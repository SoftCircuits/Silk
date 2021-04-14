// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftCircuits.Silk
{
    /// <summary>
    /// Base class for value types.
    /// </summary>
    internal abstract class Value
    {
        public abstract VarType VarType { get; }
        public virtual int ListCount => 1;
        public virtual IEnumerable<Variable> GetList() => Enumerable.Empty<Variable>();
        public abstract int ToInteger();
        public abstract double ToFloat();

        public static int Div(int val1, int val2) => (val2 != 0) ? val1 / val2 : 0;

        public static double Div(double val1, double val2) => (val2 != 0) ? val1 / val2 : 0.0;

        public static int Mod(int val1, int val2) => (val2 != 0) ? val1 % val2 : 0;

        public static double Mod(double val1, double val2) => (val2 != 0) ? val1 % val2 : 0.0;

        /// <summary>
        /// Returns true if this variable contains a FloatValue, or a StringValue representation
        /// of a floating-point value. Returns false for a StringValue contains that contains
        /// an integer string.
        /// </summary>
        public virtual bool IsFloat() => false;

        #region Operations

        public abstract Variable Add(Variable value);
        public abstract Variable Add(string value);
        public abstract Variable Add(int value);
        public abstract Variable Add(double value);

        public abstract Variable Subtract(Variable value);
        public abstract Variable Subtract(string value);
        public abstract Variable Subtract(int value);
        public abstract Variable Subtract(double value);

        public abstract Variable Multiply(Variable value);
        public abstract Variable Multiply(string value);
        public abstract Variable Multiply(int value);
        public abstract Variable Multiply(double value);

        public abstract Variable Divide(Variable value);
        public abstract Variable Divide(string value);
        public abstract Variable Divide(int value);
        public abstract Variable Divide(double value);

        public abstract Variable Power(Variable value);
        public abstract Variable Power(string value);
        public abstract Variable Power(int value);
        public abstract Variable Power(double value);

        public abstract Variable Modulus(Variable value);
        public abstract Variable Modulus(string value);
        public abstract Variable Modulus(int value);
        public abstract Variable Modulus(double value);

        public abstract Variable Concat(Variable value);
        public abstract Variable Concat(string value);
        public abstract Variable Concat(int value);
        public abstract Variable Concat(double value);

        public abstract Variable Negate();

        #endregion

        #region Comparisons

        public bool IsEqual(Variable value) => CompareTo(value) == 0;
        public bool IsEqual(string value) => CompareTo(value) == 0;
        public bool IsEqual(int value) => CompareTo(value) == 0;
        public bool IsEqual(double value) => CompareTo(value) == 0;

        public bool IsNotEqual(Variable value) => CompareTo(value) != 0;
        public bool IsNotEqual(string value) => CompareTo(value) != 0;
        public bool IsNotEqual(int value) => CompareTo(value) != 0;
        public bool IsNotEqual(double value) => CompareTo(value) != 0;

        public bool IsGreaterThan(Variable value) => CompareTo(value) > 0;
        public bool IsGreaterThan(string value) => CompareTo(value) > 0;
        public bool IsGreaterThan(int value) => CompareTo(value) > 0;
        public bool IsGreaterThan(double value) => CompareTo(value) > 0;

        public bool IsGreaterThanOrEqual(Variable value) => CompareTo(value) >= 0;
        public bool IsGreaterThanOrEqual(string value) => CompareTo(value) >= 0;
        public bool IsGreaterThanOrEqual(int value) => CompareTo(value) >= 0;
        public bool IsGreaterThanOrEqual(double value) => CompareTo(value) >= 0;

        public bool IsLessThan(Variable value) => CompareTo(value) < 0;
        public bool IsLessThan(string value) => CompareTo(value) < 0;
        public bool IsLessThan(int value) => CompareTo(value) < 0;
        public bool IsLessThan(double value) => CompareTo(value) < 0;

        public bool IsLessThanOrEqual(Variable value) => CompareTo(value) <= 0;
        public bool IsLessThanOrEqual(string value) => CompareTo(value) <= 0;
        public bool IsLessThanOrEqual(int value) => CompareTo(value) <= 0;
        public bool IsLessThanOrEqual(double value) => CompareTo(value) <= 0;

        public abstract bool IsTrue();
        public abstract bool IsFalse();

        public int CompareTo(Variable value)
        {
            return value.Type switch
            {
                VarType.String => CompareTo(value.ToString()),
                VarType.Integer => CompareTo(value.ToInteger()),
                VarType.Float => CompareTo(value.ToFloat()),
                VarType.List => CompareTo((ListValue)value.Value),
                _ => throw new InvalidOperationException(),
            };
        }
        public abstract int CompareTo(string value);
        public abstract int CompareTo(int value);
        public abstract int CompareTo(double value);
        public virtual int CompareTo(ListValue value) => CompareTo(value.GetAt(0));

        /// <summary>
        /// Intended for use with IEquatable. Less forgiving than IsEqual.
        /// </summary>
        public abstract bool Equals(Value value);

        #endregion

        #region Helper methods

        /// <summary>
        /// Converts a string to a double value.
        /// </summary>
        /// <param name="s">The string to be converted.</param>
        /// <returns>A double value that corresponds to <paramref name="s"/>, or 0 if it could not be converted.</returns>
        protected static double ToFloat(string s) => double.TryParse(s, out double result) ? result : 0.0;

        /// <summary>
        /// Converts a string to an integer value.
        /// </summary>
        /// <param name="s">The string to be converted.</param>
        /// <returns>An integer value that corresponds to <paramref name="s"/>, or 0 if it could not be converted.</returns>
        protected static int ToInteger(string s)
        {
            if (IsNumber(s, out double value, out bool isFloat))
                return isFloat ? (int)Math.Round(value) : (int)value;
            return 0;
        }

        /// <summary>
        /// Returns true if the given string contains a double value. Returns false
        /// if it contains an integer or non-numeric value.
        /// </summary>
        /// <param name="s">The string to test.</param>
        /// <returns>True if <paramref name="s"/> can be converted to a double value.</returns>
        protected static bool IsFloat(string s)
        {
            if (double.TryParse(s, out _))
                return s.Contains('.');
            return false;
        }

        /// <summary>
        /// Parses <paramref name="s"/> and indicates if it's a number, and a floating
        /// point number.
        /// </summary>
        protected static bool IsNumber(string s, out double value, out bool isFloat)
        {
            if (double.TryParse(s, out value))
            {
                isFloat = s.Contains('.');
                return true;
            }
            isFloat = false;
            return false;
        }

        /// <summary>
        /// Parses <paramref name="s"/> and indicates if it's a number.
        /// </summary>
        protected static bool IsNumber(string s, out double value) => double.TryParse(s, out value);

        #endregion

    }
}
