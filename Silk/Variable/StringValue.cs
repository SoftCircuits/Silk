// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;

namespace SoftCircuits.Silk
{
    internal class StringValue : Value
    {
        public string Value;

        public StringValue()
        {
            Value = string.Empty;
        }

        public StringValue(string value)
        {
            Value = value ?? string.Empty;
        }

        public override ValueType Type => ValueType.String;
        public override string ToString() => Value;
        public override int ToInteger() => int.TryParse(Value, out int value) ? value : 0;
        public override double ToFloat() => double.TryParse(Value, out double value) ? value : 0.0;
        public override bool IsFloat() => IsFloat(Value);

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

        public override Variable Add(Variable value) => (IsFloat() || value.IsFloat()) ? new Variable(ToFloat() + value.ToFloat()) : new Variable(ToInteger() + value.ToInteger());
        public override Variable Add(string value) => (IsFloat() || IsFloat(value)) ? new Variable(ToFloat() + ToFloat(value)) : new Variable(ToInteger() + ToInteger(value));
        public override Variable Add(int value) => IsFloat() ? new Variable(ToFloat() + value) : new Variable(ToInteger() + value);
        public override Variable Add(double value) => new Variable(ToFloat() + value);

        public override Variable Subtract(Variable value) => (IsFloat() || value.IsFloat()) ? new Variable(ToFloat() - value.ToFloat()) : new Variable(ToInteger() - value.ToInteger());
        public override Variable Subtract(string value) => (IsFloat() || IsFloat(value)) ? new Variable(ToFloat() - ToFloat(value)) : new Variable(ToInteger() - ToInteger(value));
        public override Variable Subtract(int value) => IsFloat() ? new Variable(ToFloat() - value) : new Variable(ToInteger() - value);
        public override Variable Subtract(double value) => new Variable(ToFloat() - value);

        public override Variable Multiply(Variable value) => (IsFloat() || value.IsFloat()) ? new Variable(ToFloat() * value.ToFloat()) : new Variable(ToInteger() * value.ToInteger());
        public override Variable Multiply(string value) => (IsFloat() || IsFloat(value)) ? new Variable(ToFloat() * ToFloat(value)) : new Variable(ToInteger() * ToInteger(value));
        public override Variable Multiply(int value) => IsFloat() ? new Variable(ToFloat() * value) : new Variable(ToInteger() * value);
        public override Variable Multiply(double value) => new Variable(ToFloat() * value);

        public override Variable Divide(Variable value) => (IsFloat() || value.IsFloat()) ? new Variable(ToFloat() / value.ToFloat()) : new Variable(ToInteger() / value.ToInteger());
        public override Variable Divide(string value) => (IsFloat() || IsFloat(value)) ? new Variable(ToFloat() / ToFloat(value)) : new Variable(ToInteger() / ToInteger(value));
        public override Variable Divide(int value) => IsFloat() ? new Variable(ToFloat() / value) : new Variable(ToInteger() / value);
        public override Variable Divide(double value) => new Variable(ToFloat() / value);

        public override Variable Power(Variable value) => new Variable(Math.Pow(ToFloat(), value.ToFloat()));
        public override Variable Power(string value) => new Variable(Math.Pow(ToFloat(), ToFloat(value)));
        public override Variable Power(int value) => new Variable(Math.Pow(ToFloat(), value));
        public override Variable Power(double value) => new Variable(Math.Pow(ToFloat(), value));

        public override Variable Modulus(Variable value) => (IsFloat() || value.IsFloat()) ? new Variable(ToFloat() % value.ToFloat()) : new Variable(ToInteger() % value.ToInteger());
        public override Variable Modulus(string value) => (IsFloat() || IsFloat(value)) ? new Variable(ToFloat() % ToFloat(value)) : new Variable(ToInteger() % ToInteger(value));
        public override Variable Modulus(int value) => IsFloat() ? new Variable(ToFloat() % value) : new Variable(ToInteger() + value); 
        public override Variable Modulus(double value) => new Variable(ToFloat() % value);

        public override Variable Concat(Variable value) => new Variable(Value + value.ToString());
        public override Variable Concat(string value) => new Variable(Value + value);
        public override Variable Concat(int value) => new Variable(Value + value.ToString());
        public override Variable Concat(double value) => new Variable(Value + value.ToString());

        public override Variable Negate() => IsFloat() ? new Variable(-ToFloat()) : new Variable(-ToInteger());

        #endregion

        #region Comparisons

        public override bool IsEqual(Variable value) => (Value.CompareTo(value.ToString()) == 0);
        public override bool IsEqual(string value) => (Value.CompareTo(value) == 0);
        public override bool IsEqual(int value) => IsFloat() ? (ToFloat() == value) : (ToInteger() == value);
        public override bool IsEqual(double value) => (ToFloat() == value);

        public override bool IsNotEqual(Variable value) => (Value.CompareTo(value.ToString()) != 0);
        public override bool IsNotEqual(string value) => (Value.CompareTo(value) != 0);
        public override bool IsNotEqual(int value) => IsFloat() ? (ToFloat() != value) : (ToInteger() != value);
        public override bool IsNotEqual(double value) => (ToFloat() != value);

        public override bool IsGreaterThan(Variable value) => (Value.CompareTo(value.ToString()) > 0);
        public override bool IsGreaterThan(string value) => (Value.CompareTo(value) > 0);
        public override bool IsGreaterThan(int value) => IsFloat() ? (ToFloat() > value) : (ToInteger() > value);
        public override bool IsGreaterThan(double value) => (ToFloat() > value);

        public override bool IsGreaterThanOrEqual(Variable value) => (Value.CompareTo(value.ToString()) >= 0);
        public override bool IsGreaterThanOrEqual(string value) => (Value.CompareTo(value) >= 0);
        public override bool IsGreaterThanOrEqual(int value) => IsFloat() ? (ToFloat() >= value) : (ToInteger() >= value);
        public override bool IsGreaterThanOrEqual(double value) => (ToFloat() >= value);

        public override bool IsLessThan(Variable value) => (Value.CompareTo(value.ToString()) < 0);
        public override bool IsLessThan(string value) => (Value.CompareTo(value) < 0);
        public override bool IsLessThan(int value) => IsFloat() ? (ToFloat() < value) : (ToInteger() < value);
        public override bool IsLessThan(double value) => (ToFloat() < value);

        public override bool IsLessThanOrEqual(Variable value) => (Value.CompareTo(value.ToString()) <= 0);
        public override bool IsLessThanOrEqual(string value) => (Value.CompareTo(value) <= 0);
        public override bool IsLessThanOrEqual(int value) => IsFloat() ? (ToFloat() <= value) : (ToInteger() <= value);
        public override bool IsLessThanOrEqual(double value) => (ToFloat() <= value);

        public override bool IsTrue() => Boolean.IsTrue(Value);
        public override bool IsFalse() => Boolean.IsFalse(Value);


        public override bool Equals(Value value)
        {
            var stringValue = value as StringValue;
            if (stringValue == null)
                return false;
            return Value == stringValue.Value;
        }

        #endregion

    }
}
