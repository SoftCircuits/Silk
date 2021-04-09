// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;

namespace SoftCircuits.Silk
{
    internal class FloatValue : Value
    {
        public double Value;

        public FloatValue()
        {
            Value = 0.0;
        }

        public FloatValue(double value) 
        {
            Value = value;
        }

        public FloatValue(string value)
        {
            Value = double.Parse(value);
        }

        public override VarType VarType => VarType.Float;
        public override string ToString() => Value.ToString();
        public override int ToInteger() => (int)Math.Round(Value);
        public override double ToFloat() => Value;
        public override bool IsFloat() => true;

        public override int GetHashCode()
        {
#if NETSTANDARD2_0
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + VarType.GetHashCode();
                hash = hash * 31 + Value.GetHashCode();
                return hash;
            }
#else
            return HashCode.Combine(VarType, Value);
#endif
        }

        #region Operations

        public override Variable Add(Variable value) => new(Value + value.ToFloat());
        public override Variable Add(string value) => new(Value + ToFloat(value));
        public override Variable Add(int value) => new(Value + value);
        public override Variable Add(double value) => new(Value + value);

        public override Variable Subtract(Variable value) => new(Value - value.ToFloat());
        public override Variable Subtract(string value) => new(Value - ToFloat(value));
        public override Variable Subtract(int value) => new(Value - value);
        public override Variable Subtract(double value) => new(Value - value);

        public override Variable Multiply(Variable value) => new(Value * value.ToFloat());
        public override Variable Multiply(string value) => new(Value * ToFloat(value));
        public override Variable Multiply(int value) => new(Value * value);
        public override Variable Multiply(double value) => new(Value * value);

        public override Variable Divide(Variable value) => new(Div(Value, value.ToFloat()));
        public override Variable Divide(string value) => new(Div(Value, ToFloat(value)));
        public override Variable Divide(int value) => new(Div(Value, value));
        public override Variable Divide(double value) => new(Div(Value, value));

        public override Variable Power(Variable value) => new(Math.Pow(Value, value.ToFloat()));
        public override Variable Power(string value) => new(Math.Pow(Value, ToFloat(value)));
        public override Variable Power(int value) => new(Math.Pow(Value, value));
        public override Variable Power(double value) => new(Math.Pow(Value, value));

        public override Variable Modulus(Variable value) => new(Mod(Value, value.ToFloat()));
        public override Variable Modulus(string value) => new(Mod(Value, ToFloat(value)));
        public override Variable Modulus(int value) => new(Mod(Value, value));
        public override Variable Modulus(double value) => new(Mod(Value, value));

        public override Variable Concat(Variable value) => new(ToString() + value.ToString());
        public override Variable Concat(string value) => new(ToString() + value);
        public override Variable Concat(int value) => new(ToString() + value.ToString());
        public override Variable Concat(double value) => new(ToString() + value.ToString());

        public override Variable Negate() => new(-Value);

        #endregion

        #region Comparisons

        public override int CompareTo(string value)
        {
            if (IsNumber(value, out double v))
                return Value.CompareTo(v);
            return Value.ToString().CompareTo(value);
        }
        public override int CompareTo(int value) => Value.CompareTo(value);
        public override int CompareTo(double value) => Value.CompareTo(value);

        public override bool IsTrue() => Boolean.IsTrue(Value);
        public override bool IsFalse() => Boolean.IsFalse(Value);

        public override bool Equals(Value value) => value is FloatValue floatValue && Value == floatValue.Value;

        #endregion

    }
}
