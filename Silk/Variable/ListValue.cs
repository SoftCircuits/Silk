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
            Value = new List<Variable>(values.Count());
            Value.AddRange(values.Select(v => new Variable(v)));
        }

        public override VarType VarType => VarType.List;
        public override int ListCount => Value.Count;
        public override IEnumerable<Variable> GetList() => Value;
        public override string ToString() => $"{{ {string.Join(", ", Value)} }}";
        public override int ToInteger() => IsValidIndex(0) ? Value[0].ToInteger() : 0;
        public override double ToFloat() => IsValidIndex(0) ? Value[0].ToFloat() : 0.0;

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

        public override int CompareTo(Variable value)
        {
            switch (value.Type)
            {
                case VarType.String:
                    return CompareTo(value.ToString());
                case VarType.Integer:
                    return CompareTo(value.ToInteger());
                case VarType.Float:
                    return CompareTo(value.ToFloat());
                case VarType.List:
                default:
                    if (value.Value is ListValue listValue)
                    {
                        int i = 0;

                        while (i < Value.Count && i < listValue.Value.Count)
                        {
                            int compare = Value[i].CompareTo(listValue.Value[i]);
                            if (compare != 0)
                                return compare;
                            i++;
                        }
                        return Value.Count.CompareTo(listValue.Value.Count);
                    }
                    throw new InvalidOperationException();
            }
        }
        public override int CompareTo(string value) => (Value.Count > 0) ? Value[0].CompareTo(value) : new Variable().CompareTo(value);
        public override int CompareTo(int value) => (Value.Count > 0) ? Value[0].CompareTo(value) : new Variable().CompareTo(value);
        public override int CompareTo(double value) => (Value.Count > 0) ? Value[0].CompareTo(value) : new Variable().CompareTo(value);

        public override bool IsTrue() => (Value.Count > 0) && Value[0].IsTrue();
        public override bool IsFalse() => (Value.Count <= 0) || Value[0].IsFalse();

        public override bool Equals(Value value) => value is ListValue arrayValue && Value == arrayValue.Value;

        #endregion

    }
}
