// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SoftCircuits.Silk
{
    /// <summary>
    /// Represents a data variable.
    /// </summary>
    /// <remarks>
    /// To handle variables of different types, the actual value is stored
    /// in a Value-derived object.
    /// </remarks>
    public class Variable : IEquatable<Variable>
    {
        // Internal value
        private Value Value;

        /// <summary>
        /// Constructs a variable with the value 0.
        /// </summary>
        public Variable() => SetValue(0);

        /// <summary>
        /// Constructs a variable with a string value.
        /// </summary>
        public Variable(string value) => SetValue(value);

        /// <summary>
        /// Constructs a variable with an integer value.
        /// </summary>
        public Variable(int value) => SetValue(value);

        /// <summary>
        /// Constructs a variable with a floating-point value.
        /// </summary>
        public Variable(double value) => SetValue(value);

        /// <summary>
        /// Constructs a variable with the same value as another variable.
        /// </summary>
        public Variable(Variable var) => SetValue(var);

        /// <summary>
        /// Constructs a variable from a token.
        /// </summary>
        internal Variable(Token token) => SetValue(token);

        /// <summary>
        /// Constructs a variable from a value.
        /// </summary>
        internal Variable(Value value) => SetValue(value);

        /// <summary>
        /// Constructs a list variable that contains the given items.
        /// </summary>
        public Variable(IEnumerable<Variable> variables) => SetValue(variables);

        /// <summary>
        /// Creates an array variable of the specified size.
        /// </summary>
        /// <param name="size">Size of this array.</param>
        public static Variable CreateList(int size) => new Variable(new ListValue(size));

        #region SetValue

        /// <summary>
        /// Sets this variable to a string value.
        /// </summary>
        public void SetValue(string value)
        {
            if (Value is StringValue stringValue)
                stringValue.Value = value;
            else
                Value = new StringValue(value);
        }

        /// <summary>
        /// Sets this variable to an integer value.
        /// </summary>
        public void SetValue(int value)
        {
            if (Value is IntegerValue integerValue)
                integerValue.Value = value;
            else
                Value = new IntegerValue(value);
        }

        /// <summary>
        /// Sets this variable to a floating-point value.
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(double value)
        {
            if (Value is FloatValue floatValue)
                floatValue.Value = value;
            else
                Value = new FloatValue(value);
        }

        /// <summary>
        /// Sets this variable to a list with the given items.
        /// </summary>
        public void SetValue(IEnumerable<Variable> values)
        {
            if (Value is ListValue listValue)
            {
                listValue.Value.Clear();
                listValue.Value.AddRange(values);
            }
            else Value = new ListValue(values);
        }

        /// <summary>
        /// Sets this variable's value to the value of another variable.
        /// </summary>
        public void SetValue(Variable var)
        {
            switch (var.Type)
            {
                case ValueType.String:
                    SetValue(var.ToString());
                    break;
                case ValueType.Integer:
                    SetValue(var.ToInteger());
                    break;
                case ValueType.Float:
                    SetValue(var.ToFloat());
                    break;
                case ValueType.List:
                    SetValue(var.GetList());
                    break;
                default:
                    throw new Exception($"Attempting to convert Variable with unknown data type ({var.Type})");
            }
        }

        /// <summary>
        /// Sets this variable's value from a token.
        /// </summary>
        internal void SetValue(Token token)
        {
            switch (token.Type)
            {
                case TokenType.String:
                    Value = new StringValue(token.Value);
                    break;
                case TokenType.Integer:
                    Value = new IntegerValue(token.Value);
                    break;
                case TokenType.Float:
                    Value = new FloatValue(token.Value);
                    break;
                default:
                    throw new Exception($"Attempted to convert {token.Type} token ({token.Value}) to variable.");
            }
        }

        /// <summary>
        /// Sets this variable to the given value.
        /// </summary>
        internal void SetValue(Value value)
        {
            Debug.Assert(value != null);
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        #endregion

        #region Operations

        /// <summary>
        /// Returns the this variable's current type.
        /// </summary>
        public ValueType Type => Value.Type;

        /// <summary>
        /// Returns true if this variable contains a list.
        /// </summary>
        public bool IsList => Value.Type == ValueType.List;

        /// <summary>
        /// Returns the number of items in this list. Returns 1 if this variable
        /// is not a list.
        /// </summary>
        public int ListCount => Value.ListCount;

        /// <summary>
        /// Gets the variable at the specified position of a list.
        /// </summary>
        /// <param name="index">0-based list position.</param>
        public Variable GetAt(int index) => (Value is ListValue listValue) ? listValue.GetAt(index) : (index == 0) ? this : new Variable();

        /// <summary>
        /// Returns the items from this variable's list. Returns a single item if
        /// this variable does not contain a list.
        /// </summary>
        public IEnumerable<Variable> GetList() => IsList ? Value.GetList() : new[] { this };

        /// <summary>
        /// Converts this variable to a string.
        /// </summary>
        public override string ToString() => Value.ToString();

        /// <summary>
        /// Converts this variable to an integer.
        /// </summary>
        public int ToInteger() => Value.ToInteger();

        /// <summary>
        /// Converts this variable to a floating-point value.
        /// </summary>
        public double ToFloat() => Value.ToFloat();

        /// <summary>
        /// Returns true if this variable contains a floating point value, or a text
        /// representation of a floating-point value. Returns false for a text value
        /// that represents an integer value.
        /// </summary>
        public bool IsFloat() => Value.IsFloat();

        public Variable Add(Variable value) => Value.Add(value);
        public Variable Add(string value) => Value.Add(value);
        public Variable Add(int value) => Value.Add(value);
        public Variable Add(double value) => Value.Add(value);

        public Variable Subtract(Variable value) => Value.Subtract(value);
        public Variable Subtract(string value) => Value.Subtract(value);
        public Variable Subtract(int value) => Value.Subtract(value);
        public Variable Subtract(double value) => Value.Subtract(value);

        public Variable Multiply(Variable value) => Value.Multiply(value);
        public Variable Multiply(string value) => Value.Multiply(value);
        public Variable Multiply(int value) => Value.Multiply(value);
        public Variable Multiply(double value) => Value.Multiply(value);

        public Variable Divide(Variable value) => Value.Divide(value);
        public Variable Divide(string value) => Value.Divide(value);
        public Variable Divide(int value) => Value.Divide(value);
        public Variable Divide(double value) => Value.Divide(value);

        public Variable Power(Variable value) => Value.Power(value);
        public Variable Power(string value) => Value.Power(value);
        public Variable Power(int value) => Value.Power(value);
        public Variable Power(double value) => Value.Power(value);

        public Variable Modulus(Variable value) => Value.Modulus(value);
        public Variable Modulus(string value) => Value.Modulus(value);
        public Variable Modulus(int value) => Value.Modulus(value);
        public Variable Modulus(double value) => Value.Modulus(value);

        public Variable Concat(Variable value) => Value.Concat(value);
        public Variable Concat(string value) => Value.Concat(value);
        public Variable Concat(int value) => Value.Concat(value);
        public Variable Concat(double value) => Value.Concat(value);

        public Variable Negate() => Value.Negate();

        #endregion

        #region Comparisons

        public bool IsEqual(Variable var) => Value.IsEqual(var);
        public bool IsEqual(string var) => Value.IsEqual(var);
        public bool IsEqual(int var) => Value.IsEqual(var);
        public bool IsEqual(double var) => Value.IsEqual(var);

        public bool IsNotEqual(Variable var) => Value.IsNotEqual(var);
        public bool IsNotEqual(string var) => Value.IsNotEqual(var);
        public bool IsNotEqual(int var) => Value.IsNotEqual(var);
        public bool IsNotEqual(double var) => Value.IsNotEqual(var);

        public bool IsGreaterThan(Variable var) => Value.IsGreaterThan(var);
        public bool IsGreaterThan(string var) => Value.IsGreaterThan(var);
        public bool IsGreaterThan(int var) => Value.IsGreaterThan(var);
        public bool IsGreaterThan(double var) => Value.IsGreaterThan(var);

        public bool IsGreaterThanOrEqual(Variable var) => Value.IsGreaterThanOrEqual(var);
        public bool IsGreaterThanOrEqual(string var) => Value.IsGreaterThanOrEqual(var);
        public bool IsGreaterThanOrEqual(int var) => Value.IsGreaterThanOrEqual(var);
        public bool IsGreaterThanOrEqual(double var) => Value.IsGreaterThanOrEqual(var);

        public bool IsLessThan(Variable var) => Value.IsLessThan(var);
        public bool IsLessThan(string var) => Value.IsLessThan(var);
        public bool IsLessThan(int var) => Value.IsLessThan(var);
        public bool IsLessThan(double var) => Value.IsLessThan(var);

        public bool IsLessThanOrEqual(Variable var) => Value.IsLessThanOrEqual(var);
        public bool IsLessThanOrEqual(string var) => Value.IsLessThanOrEqual(var);
        public bool IsLessThanOrEqual(int var) => Value.IsLessThanOrEqual(var);
        public bool IsLessThanOrEqual(double var) => Value.IsLessThanOrEqual(var);

        public bool IsTrue() => Value.IsTrue();
        public bool IsFalse() => Value.IsFalse();

        #endregion

        #region Operator overloads

        public static bool operator ==(Variable var1, Variable var2) => var1.IsEqual(var2);
        public static bool operator ==(Variable var1, string var2) => var1.IsEqual(var2);
        public static bool operator ==(Variable var1, int var2) => var1.IsEqual(var2);
        public static bool operator ==(Variable var1, double var2) => var1.IsEqual(var2);

        public static bool operator !=(Variable var1, Variable var2) => var1.IsNotEqual(var2);
        public static bool operator !=(Variable var1, string var2) => var1.IsNotEqual(var2);
        public static bool operator !=(Variable var1, int var2) => var1.IsNotEqual(var2);
        public static bool operator !=(Variable var1, double var2) => var1.IsNotEqual(var2);

        public static bool operator >(Variable var1, Variable var2) => var1.IsGreaterThan(var2);
        public static bool operator >(Variable var1, string var2) => var1.IsGreaterThan(var2);
        public static bool operator >(Variable var1, int var2) => var1.IsGreaterThan(var2);
        public static bool operator >(Variable var1, double var2) => var1.IsGreaterThan(var2);

        public static bool operator >=(Variable var1, Variable var2) => var1.IsGreaterThanOrEqual(var2);
        public static bool operator >=(Variable var1, string var2) => var1.IsGreaterThanOrEqual(var2);
        public static bool operator >=(Variable var1, int var2) => var1.IsGreaterThanOrEqual(var2);
        public static bool operator >=(Variable var1, double var2) => var1.IsGreaterThanOrEqual(var2);

        public static bool operator <(Variable var1, Variable var2) => var1.IsLessThan(var2);
        public static bool operator <(Variable var1, string var2) => var1.IsLessThan(var2);
        public static bool operator <(Variable var1, int var2) => var1.IsLessThan(var2);
        public static bool operator <(Variable var1, double var2) => var1.IsLessThan(var2);

        public static bool operator <=(Variable var1, Variable var2) => var1.IsLessThanOrEqual(var2);
        public static bool operator <=(Variable var1, string var2) => var1.IsLessThanOrEqual(var2);
        public static bool operator <=(Variable var1, int var2) => var1.IsLessThanOrEqual(var2);
        public static bool operator <=(Variable var1, double var2) => var1.IsLessThanOrEqual(var2);

        public static Variable operator +(Variable var1, Variable var2) => var1.Add(var2);
        public static Variable operator +(Variable var1, string value) => var1.Add(value);
        public static Variable operator +(Variable var1, int value) => var1.Add(value);
        public static Variable operator +(Variable var1, double value) => var1.Add(value);

        public static Variable operator -(Variable var1, Variable var2) => var1.Subtract(var2);
        public static Variable operator -(Variable var1, string value) => var1.Subtract(value);
        public static Variable operator -(Variable var1, int value) => var1.Subtract(value);
        public static Variable operator -(Variable var1, double value) => var1.Subtract(value);

        public static Variable operator *(Variable var1, Variable var2) => var1.Multiply(var2);
        public static Variable operator *(Variable var1, string value) => var1.Multiply(value);
        public static Variable operator *(Variable var1, int value) => var1.Multiply(value);
        public static Variable operator *(Variable var1, double value) => var1.Multiply(value);

        public static Variable operator /(Variable var1, Variable var2) => var1.Divide(var2);
        public static Variable operator /(Variable var1, string value) => var1.Divide(value);
        public static Variable operator /(Variable var1, int value) => var1.Divide(value);
        public static Variable operator /(Variable var1, double value) => var1.Divide(value);

        public static Variable operator %(Variable var1, Variable var2) => var1.Modulus(var2);
        public static Variable operator %(Variable var1, string value) => var1.Modulus(value);
        public static Variable operator %(Variable var1, int value) => var1.Modulus(value);
        public static Variable operator %(Variable var1, double value) => var1.Modulus(value);

        public static Variable operator -(Variable var1) => var1.Negate();

        public Variable this[int index] => GetAt(index);

        #endregion

        #region IEquatable

        public override bool Equals(object value)
        {
            return Equals(value as Variable);
        }

        public bool Equals(Variable value)
        {
            if (ReferenceEquals(value, null))
                return false;
            return Value.Equals(value.Value);
        }

        public override int GetHashCode() => Value.GetHashCode();

        #endregion

    }
}
