// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System.Collections.Generic;
using System.Linq;

namespace SoftCircuits.Silk
{
    public enum ValueType
    {
        String,
        Integer,
        Float,
        List,
    }

    /// <summary>
    /// Base class for value types.
    /// </summary>
    internal abstract class Value
    {
        public abstract ValueType Type { get; }
        public virtual int ListCount => 1;
        public virtual IEnumerable<Variable> GetList() => Enumerable.Empty<Variable>();
        public abstract int ToInteger();
        public abstract double ToFloat();

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

        public abstract bool IsEqual(Variable value);
        public abstract bool IsEqual(string value);
        public abstract bool IsEqual(int value);
        public abstract bool IsEqual(double value);

        public abstract bool IsNotEqual(Variable value);
        public abstract bool IsNotEqual(string value);
        public abstract bool IsNotEqual(int value);
        public abstract bool IsNotEqual(double value);

        public abstract bool IsGreaterThan(Variable value);
        public abstract bool IsGreaterThan(string value);
        public abstract bool IsGreaterThan(int value);
        public abstract bool IsGreaterThan(double value);

        public abstract bool IsGreaterThanOrEqual(Variable value);
        public abstract bool IsGreaterThanOrEqual(string value);
        public abstract bool IsGreaterThanOrEqual(int value);
        public abstract bool IsGreaterThanOrEqual(double value);

        public abstract bool IsLessThan(Variable value);
        public abstract bool IsLessThan(string value);
        public abstract bool IsLessThan(int value);
        public abstract bool IsLessThan(double value);

        public abstract bool IsLessThanOrEqual(Variable value);
        public abstract bool IsLessThanOrEqual(string value);
        public abstract bool IsLessThanOrEqual(int value);
        public abstract bool IsLessThanOrEqual(double value);

        public abstract bool IsTrue();
        public abstract bool IsFalse();

        /// <summary>
        /// Intended for use with IEquatable. Less forgiving than IsEqual.
        /// </summary>
        public abstract bool Equals(Value value);

        #endregion

        #region Helper methods

        protected double ToFloat(string s) => double.TryParse(s, out double result) ? result : 0.0;
        protected int ToInteger(string s) => int.TryParse(s, out int result) ? result : 0;

        /// <summary>
        /// Returns true if the given string contains a double value. Returns false
        /// if it contains an integer value or no number at all.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected bool IsFloat(string s)
        {
            if (double.TryParse(s, out double result))
                return s.Contains('.');
            return false;
        }

        #endregion

    }
}
