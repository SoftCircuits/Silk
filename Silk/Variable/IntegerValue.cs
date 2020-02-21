// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;

namespace SoftCircuits.Silk
{
    internal class IntegerValue : Value
    {
        public int Value;

        public IntegerValue()
        {
            Value = 0;
        }

        public IntegerValue(int value)
        {
            Value = value;
        }

        public IntegerValue(string value)
        {
            if (value.Length > 2 && value[0] == '0' && value[1] == 'x')
                Value = Convert.ToInt32(value, 16);
            else
                Value = int.Parse(value);
        }

        public override ValueType Type => ValueType.Integer;
        public override string ToString() => Value.ToString();
        public override int ToInteger() => Value;
        public override double ToFloat() => Value;

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

        public override Variable Add(Variable value) => value.IsFloat() ? new Variable(Value + value.ToFloat()) : new Variable(Value + value.ToInteger());
        public override Variable Add(string value) => IsFloat(value) ? new Variable(Value + ToFloat(value)) : new Variable(Value + ToInteger(value));
        public override Variable Add(int value) => new Variable(Value + value);
        public override Variable Add(double value) => new Variable(Value + value);

        public override Variable Subtract(Variable value) => value.IsFloat() ? new Variable(Value - value.ToFloat()) : new Variable(Value - value.ToInteger());
        public override Variable Subtract(string value) => IsFloat(value) ? new Variable(Value - ToFloat(value)) : new Variable(Value - ToInteger(value));
        public override Variable Subtract(int value) => new Variable(Value - value);
        public override Variable Subtract(double value) => new Variable(Value - value);

        public override Variable Multiply(Variable value) => value.IsFloat() ? new Variable(Value * value.ToFloat()) : new Variable(Value * value.ToInteger());
        public override Variable Multiply(string value) => IsFloat(value) ? new Variable(Value * ToFloat(value)) : new Variable(Value * ToInteger(value));
        public override Variable Multiply(int value) => new Variable(Value * value);
        public override Variable Multiply(double value) => new Variable(Value * value);

        public override Variable Divide(Variable value) => value.IsFloat() ? new Variable(Value / value.ToFloat()) : new Variable(Value / value.ToInteger());
        public override Variable Divide(string value) => IsFloat(value) ? new Variable(Value / ToFloat(value)) : new Variable(Value / ToInteger(value));
        public override Variable Divide(int value) => new Variable(Value / value);
        public override Variable Divide(double value) => new Variable(Value / value);

        public override Variable Power(Variable value) => new Variable(Math.Pow(Value, value.ToFloat()));
        public override Variable Power(string value) => new Variable(Math.Pow(Value, ToFloat(value)));
        public override Variable Power(int value) => new Variable(Math.Pow(Value, value));
        public override Variable Power(double value) => new Variable(Math.Pow(Value, value));

        public override Variable Modulus(Variable value) => value.IsFloat() ? new Variable(Value % value.ToFloat()) : new Variable(Value % value.ToInteger());
        public override Variable Modulus(string value) => IsFloat(value) ? new Variable(Value % ToFloat(value)) : new Variable(Value % ToInteger(value));
        public override Variable Modulus(int value) => new Variable(Value % value);
        public override Variable Modulus(double value) => new Variable(Value % value);

        public override Variable Concat(Variable value) => new Variable(ToString() + value.ToString());
        public override Variable Concat(string value) => new Variable(ToString() + value);
        public override Variable Concat(int value) => new Variable(ToString() + value.ToString());
        public override Variable Concat(double value) => new Variable(ToString() + value.ToString());

        public override Variable Negate() => new Variable(-Value);

        #endregion

        #region Comparisons

        public override bool IsEqual(Variable value) => (Value == value.ToInteger());
        public override bool IsEqual(string value) => (Value == (int.TryParse(value, out int result) ? result : 0));
        public override bool IsEqual(int value) => (Value == value);
        public override bool IsEqual(double value) => (Value == value);

        public override bool IsNotEqual(Variable value) => (Value != value.ToInteger());
        public override bool IsNotEqual(string value) => (Value != (int.TryParse(value, out int result) ? result : 0));
        public override bool IsNotEqual(int value) => (Value != value);
        public override bool IsNotEqual(double value) => (Value != value);

        public override bool IsGreaterThan(Variable value) => (Value > value.ToInteger());
        public override bool IsGreaterThan(string value) => (Value > (int.TryParse(value, out int result) ? result : 0));
        public override bool IsGreaterThan(int value) => (Value > value);
        public override bool IsGreaterThan(double value) => (Value > value);

        public override bool IsGreaterThanOrEqual(Variable value) => (Value >= value.ToInteger());
        public override bool IsGreaterThanOrEqual(string value) => (Value >= (int.TryParse(value, out int result) ? result : 0));
        public override bool IsGreaterThanOrEqual(int value) => (Value >= value);
        public override bool IsGreaterThanOrEqual(double value) => (Value >= value);

        public override bool IsLessThan(Variable value) => (Value < value.ToInteger());
        public override bool IsLessThan(string value) => (Value < (int.TryParse(value, out int result) ? result : 0));
        public override bool IsLessThan(int value) => (Value < value);
        public override bool IsLessThan(double value) => (Value < value);

        public override bool IsLessThanOrEqual(Variable value) => (Value <= value.ToInteger());
        public override bool IsLessThanOrEqual(string value) => (Value <= (int.TryParse(value, out int result) ? result : 0));
        public override bool IsLessThanOrEqual(int value) => (Value <= value);
        public override bool IsLessThanOrEqual(double value) => (Value <= value);

        public override bool IsTrue() => Boolean.IsTrue(Value);
        public override bool IsFalse() => Boolean.IsFalse(Value);


        public override bool Equals(Value value)
        {
            var integerValue = value as IntegerValue;
            if (integerValue == null)
                return false;
            return Value == integerValue.Value;
        }

        #endregion

    }
}
