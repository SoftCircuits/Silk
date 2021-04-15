// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
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
            Value = new List<Variable>(values);
        }

        public override VarType VarType => VarType.List;
        public override int ListCount => Value.Count;
        public override IEnumerable<Variable> GetList() => Value;
        public override string ToString() => $"{{ {string.Join(", ", Value)} }}";
        public override int ToInteger() => GetAt(0).ToInteger();
        public override double ToFloat() => GetAt(0).ToFloat();

        public override int GetHashCode()
        {
#if NETSTANDARD2_0
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + VarType.GetHashCode();
                foreach (Variable v in Value)
                    hash = hash * 31 + Value.GetHashCode();
                return hash;
            }
#else
            HashCode hash = new();
            hash.Add(VarType);
            foreach (Variable variable in Value)
                hash.Add(variable.GetHashCode());
            return hash.ToHashCode();
#endif
        }

        public bool IsValidIndex(int index) => index >= 0 && index < Value.Count;
        public Variable GetAt(int index) => IsValidIndex(index) ? Value[index] : new Variable();

        #region Operations

        public override Variable Add(Variable value) => GetAt(0).Add(value);
        public override Variable Add(string value) => GetAt(0).Add(value);
        public override Variable Add(int value) => GetAt(0).Add(value);
        public override Variable Add(double value) => GetAt(0).Add(value);

        public override Variable Subtract(Variable value) => GetAt(0).Subtract(value);
        public override Variable Subtract(string value) => GetAt(0).Subtract(value);
        public override Variable Subtract(int value) => GetAt(0).Subtract(value);
        public override Variable Subtract(double value) => GetAt(0).Subtract(value);

        public override Variable Multiply(Variable value) => GetAt(0).Multiply(value);
        public override Variable Multiply(string value) => GetAt(0).Multiply(value);
        public override Variable Multiply(int value) => GetAt(0).Multiply(value);
        public override Variable Multiply(double value) => GetAt(0).Multiply(value);

        public override Variable Divide(Variable value) => GetAt(0).Divide(value);
        public override Variable Divide(string value) => GetAt(0).Divide(value);
        public override Variable Divide(int value) => GetAt(0).Divide(value);
        public override Variable Divide(double value) => GetAt(0).Divide(value);

        public override Variable Power(Variable value) => GetAt(0).Power(value);
        public override Variable Power(string value) => GetAt(0).Power(value);
        public override Variable Power(int value) => GetAt(0).Power(value);
        public override Variable Power(double value) => GetAt(0).Power(value);

        public override Variable Modulus(Variable value) => GetAt(0).Modulus(value);
        public override Variable Modulus(string value) => GetAt(0).Modulus(value);
        public override Variable Modulus(int value) => GetAt(0).Modulus(value);
        public override Variable Modulus(double value) => GetAt(0).Modulus(value);

        public override Variable Concat(Variable value) => GetAt(0).Concat(value);
        public override Variable Concat(string value) => GetAt(0).Concat(value);
        public override Variable Concat(int value) => GetAt(0).Concat(value);
        public override Variable Concat(double value) => GetAt(0).Concat(value);

        public override Variable Negate() => GetAt(0).Negate();

        #endregion

        #region Comparisons

        public override int CompareTo(string value) => GetAt(0).CompareTo(value);
        public override int CompareTo(int value) => GetAt(0).CompareTo(value);
        public override int CompareTo(double value) => GetAt(0).CompareTo(value);
        public override int CompareTo(ListValue value)
        {
            List<Variable> list = value.Value;
            for (int i = 0; i < Value.Count && i < list.Count; i++)
            {
                int compare = Value[i].CompareTo(list[i]);
                if (compare != 0)
                    return compare;
            }
            return Value.Count - list.Count;
        }

        public override bool IsTrue() => GetAt(0).IsTrue();
        public override bool IsFalse() => GetAt(0).IsFalse();

        public override bool Equals(Value value) => value is ListValue arrayValue && Enumerable.SequenceEqual(Value, arrayValue.Value);

        #endregion

    }
}
