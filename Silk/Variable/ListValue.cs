// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoftCircuits.Silk
{
    internal class ListValue : Value
    {
        public List<Variable> Value;

        public ListValue()
        {
            Value = new List<Variable>();
        }

        public ListValue(int size)
        {
            Value = new List<Variable>(size);
            for (int i = 0; i < size; i++)
                Value.Add(new Variable());
        }

        public ListValue(IEnumerable<Variable> values)
        {
            Value = new List<Variable>(values.Count());
            foreach (Variable value in values)
                Value.Add(new Variable(value));
        }

        public override ValueType Type => ValueType.List;
        public override int ListCount => Value.Count;
        public override IEnumerable<Variable> GetList() => Value;
        public override string ToString() => $"{{ {string.Join(", ", Value)} }}";
        public override int ToInteger() => IsValidIndex(0) ? Value[0].ToInteger() : 0;
        public override double ToFloat() => IsValidIndex(0) ? Value[0].ToFloat() : 0.0;

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Type.GetHashCode();
                foreach (Variable v in Value)
                    hash = hash * 31 + Value.GetHashCode();
                return hash;
            }
        }

        public Variable GetAt(int index) => IsValidIndex(index) ? Value[index] : new Variable();
        public bool IsValidIndex(int index) => index >= 0 && index < Value.Count;

        #region Operations

        public override Variable Add(Variable value) => (Value.Count > 0) ? Value[0].Add(value) : new Variable();
        public override Variable Add(string value) => (Value.Count > 0) ? Value[0].Add(value) : new Variable();
        public override Variable Add(int value) => (Value.Count > 0) ? Value[0].Add(value) : new Variable();
        public override Variable Add(double value) => (Value.Count > 0) ? Value[0].Add(value) : new Variable();

        public override Variable Subtract(Variable value) => (Value.Count > 0) ? Value[0].Subtract(value) : new Variable();
        public override Variable Subtract(string value) => (Value.Count > 0) ? Value[0].Subtract(value) : new Variable();
        public override Variable Subtract(int value) => (Value.Count > 0) ? Value[0].Subtract(value) : new Variable();
        public override Variable Subtract(double value) => (Value.Count > 0) ? Value[0].Subtract(value) : new Variable();

        public override Variable Multiply(Variable value) => (Value.Count > 0) ? Value[0].Multiply(value) : new Variable();
        public override Variable Multiply(string value) => (Value.Count > 0) ? Value[0].Multiply(value) : new Variable();
        public override Variable Multiply(int value) => (Value.Count > 0) ? Value[0].Multiply(value) : new Variable();
        public override Variable Multiply(double value) => (Value.Count > 0) ? Value[0].Multiply(value) : new Variable();

        public override Variable Divide(Variable value) => (Value.Count > 0) ? Value[0].Divide(value) : new Variable();
        public override Variable Divide(string value) => (Value.Count > 0) ? Value[0].Divide(value) : new Variable();
        public override Variable Divide(int value) => (Value.Count > 0) ? Value[0].Divide(value) : new Variable();
        public override Variable Divide(double value) => (Value.Count > 0) ? Value[0].Divide(value) : new Variable();

        public override Variable Power(Variable value) => (Value.Count > 0) ? Value[0].Power(value) : new Variable();
        public override Variable Power(string value) => (Value.Count > 0) ? Value[0].Power(value) : new Variable();
        public override Variable Power(int value) => (Value.Count > 0) ? Value[0].Power(value) : new Variable();
        public override Variable Power(double value) => (Value.Count > 0) ? Value[0].Power(value) : new Variable();

        public override Variable Modulus(Variable value) => (Value.Count > 0) ? Value[0].Modulus(value) : new Variable();
        public override Variable Modulus(string value) => (Value.Count > 0) ? Value[0].Modulus(value) : new Variable();
        public override Variable Modulus(int value) => (Value.Count > 0) ? Value[0].Modulus(value) : new Variable();
        public override Variable Modulus(double value) => (Value.Count > 0) ? Value[0].Modulus(value) : new Variable();

        public override Variable Concat(Variable value) => (Value.Count > 0) ? Value[0].Concat(value) : new Variable();
        public override Variable Concat(string value) => (Value.Count > 0) ? Value[0].Concat(value) : new Variable();
        public override Variable Concat(int value) => (Value.Count > 0) ? Value[0].Concat(value) : new Variable();
        public override Variable Concat(double value) => (Value.Count > 0) ? Value[0].Concat(value) : new Variable();

        public override Variable Negate() => (Value.Count > 0) ? Value[0].Negate() : new Variable();

        #endregion

        #region Comparisons

        public override bool IsEqual(Variable value) => (Value.Count > 0) ? Value[0].IsEqual(value) : false;
        public override bool IsEqual(string value) => (Value.Count > 0) ? Value[0].IsEqual(value) : false;
        public override bool IsEqual(int value) => (Value.Count > 0) ? Value[0].IsEqual(value) : false;
        public override bool IsEqual(double value) => (Value.Count > 0) ? Value[0].IsEqual(value) : false;

        public override bool IsNotEqual(Variable value) => (Value.Count > 0) ? Value[0].IsNotEqual(value) : true;
        public override bool IsNotEqual(string value) => (Value.Count > 0) ? Value[0].IsNotEqual(value) : true;
        public override bool IsNotEqual(int value) => (Value.Count > 0) ? Value[0].IsNotEqual(value) : true;
        public override bool IsNotEqual(double value) => (Value.Count > 0) ? Value[0].IsNotEqual(value) : true;

        public override bool IsGreaterThan(Variable value) => (Value.Count > 0) ? Value[0].IsEqual(value) : false;
        public override bool IsGreaterThan(string value) => (Value.Count > 0) ? Value[0].IsGreaterThan(value) : false;
        public override bool IsGreaterThan(int value) => (Value.Count > 0) ? Value[0].IsGreaterThan(value) : false;
        public override bool IsGreaterThan(double value) => (Value.Count > 0) ? Value[0].IsGreaterThan(value) : false;

        public override bool IsGreaterThanOrEqual(Variable value) => (Value.Count > 0) ? Value[0].IsEqual(value) : false;
        public override bool IsGreaterThanOrEqual(string value) => (Value.Count > 0) ? Value[0].IsGreaterThanOrEqual(value) : false;
        public override bool IsGreaterThanOrEqual(int value) => (Value.Count > 0) ? Value[0].IsGreaterThanOrEqual(value) : false;
        public override bool IsGreaterThanOrEqual(double value) => (Value.Count > 0) ? Value[0].IsGreaterThanOrEqual(value) : false;

        public override bool IsLessThan(Variable value) => (Value.Count > 0) ? Value[0].IsEqual(value) : false;
        public override bool IsLessThan(string value) => (Value.Count > 0) ? Value[0].IsLessThan(value) : false;
        public override bool IsLessThan(int value) => (Value.Count > 0) ? Value[0].IsLessThan(value) : false;
        public override bool IsLessThan(double value) => (Value.Count > 0) ? Value[0].IsLessThan(value) : false;

        public override bool IsLessThanOrEqual(Variable value) => (Value.Count > 0) ? Value[0].IsLessThanOrEqual(value) : false;
        public override bool IsLessThanOrEqual(string value) => (Value.Count > 0) ? Value[0].IsLessThanOrEqual(value) : false;
        public override bool IsLessThanOrEqual(int value) => (Value.Count > 0) ? Value[0].IsLessThanOrEqual(value) : false;
        public override bool IsLessThanOrEqual(double value) => (Value.Count > 0) ? Value[0].IsLessThanOrEqual(value) : false;

        public override bool IsTrue() => (Value.Count > 0) ? Value[0].IsTrue() : false;
        public override bool IsFalse() => (Value.Count > 0) ? Value[0].IsFalse() : false;


        public override bool Equals(Value value)
        {
            var arrayValue = value as ListValue;
            if (arrayValue == null)
                return false;
            return Value == arrayValue.Value;
        }

        #endregion

    }
}
