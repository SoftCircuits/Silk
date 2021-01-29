// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Globalization;

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
            Value = double.Parse(value, new CultureInfo("en-US"));
        }

        public override ValueType Type => ValueType.Float;
        public override string ToString() => Value.ToString();
        public override int ToInteger() => (int)Math.Round(Value);
        public override double ToFloat() => Value;
        public override bool IsFloat() => true;

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + Type.GetHashCode();
                hash = hash * 31 + Value.GetHashCode();
                return hash;
            }
        }

        #region Operations

        public override Variable Add(Variable value) => new Variable(Value + value.ToFloat());
        public override Variable Add(string value) => new Variable(Value + ToFloat(value));
        public override Variable Add(int value) => new Variable(Value + value);
        public override Variable Add(double value) => new Variable(Value + value);

        public override Variable Subtract(Variable value) => new Variable(Value - value.ToFloat());
        public override Variable Subtract(string value) => new Variable(Value - ToFloat(value));
        public override Variable Subtract(int value) => new Variable(Value - value);
        public override Variable Subtract(double value) => new Variable(Value - value);

        public override Variable Multiply(Variable value) => new Variable(Value * value.ToFloat());
        public override Variable Multiply(string value) => new Variable(Value * ToFloat(value));
        public override Variable Multiply(int value) => new Variable(Value * value);
        public override Variable Multiply(double value) => new Variable(Value * value);

        public override Variable Divide(Variable value) => new Variable(Value / value.ToFloat());
        public override Variable Divide(string value) => new Variable(Value / ToFloat(value));
        public override Variable Divide(int value) => new Variable(Value / value);
        public override Variable Divide(double value) => new Variable(Value / value);

        public override Variable Power(Variable value) => new Variable(Math.Pow(Value, value.ToFloat()));
        public override Variable Power(string value) => new Variable(Math.Pow(Value, ToFloat(value)));
        public override Variable Power(int value) => new Variable(Math.Pow(Value, value));
        public override Variable Power(double value) => new Variable(Math.Pow(Value, value));

        public override Variable Modulus(Variable value) => new Variable(Value % value.ToFloat());
        public override Variable Modulus(string value) => new Variable(Value % ToFloat(value));
        public override Variable Modulus(int value) => new Variable(Value % value);
        public override Variable Modulus(double value) => new Variable(Value % value);

        public override Variable Concat(Variable value) => new Variable(ToString() + value.ToString());
        public override Variable Concat(string value) => new Variable(ToString() + value);
        public override Variable Concat(int value) => new Variable(ToString() + value.ToString());
        public override Variable Concat(double value) => new Variable(ToString() + value.ToString());

        public override Variable Negate() => new Variable(-Value);

        #endregion

        #region Comparisons

        public override bool IsEqual(Variable value) => (Value == value.ToFloat());
        public override bool IsEqual(string value) => (Value == (double.TryParse(value, out double result) ? result : 0.0));
        public override bool IsEqual(int value) => (Value == value);
        public override bool IsEqual(double value) => (Value == value);

        public override bool IsNotEqual(Variable value) => (Value != value.ToFloat());
        public override bool IsNotEqual(string value) => (Value != (double.TryParse(value, out double result) ? result : 0.0));
        public override bool IsNotEqual(int value) => (Value != value);
        public override bool IsNotEqual(double value) => (Value != value);

        public override bool IsGreaterThan(Variable value) => (Value > value.ToFloat());
        public override bool IsGreaterThan(string value) => (Value > (double.TryParse(value, out double result) ? result : 0.0));
        public override bool IsGreaterThan(int value) => (Value > value);
        public override bool IsGreaterThan(double value) => (Value > value);

        public override bool IsGreaterThanOrEqual(Variable value) => (Value >= value.ToFloat());
        public override bool IsGreaterThanOrEqual(string value) => (Value >= (double.TryParse(value, out double result) ? result : 0.0));
        public override bool IsGreaterThanOrEqual(int value) => (Value >= value);
        public override bool IsGreaterThanOrEqual(double value) => (Value >= value);

        public override bool IsLessThan(Variable value) => (Value < value.ToFloat());
        public override bool IsLessThan(string value) => (Value < (double.TryParse(value, out double result) ? result : 0.0));
        public override bool IsLessThan(int value) => (Value < value);
        public override bool IsLessThan(double value) => (Value < value);

        public override bool IsLessThanOrEqual(Variable value) => (Value <= value.ToFloat());
        public override bool IsLessThanOrEqual(string value) => (Value <= (double.TryParse(value, out double result) ? result : 0.0));
        public override bool IsLessThanOrEqual(int value) => (Value <= value);
        public override bool IsLessThanOrEqual(double value) => (Value <= value);

        public override bool IsTrue() => Boolean.IsTrue(Value);
        public override bool IsFalse() => Boolean.IsFalse(Value);


        public override bool Equals(Value value)
        {
            var floatValue = value as FloatValue;
            if (floatValue == null)
                return false;
            return Value == floatValue.Value;
        }

        #endregion
    }
}
