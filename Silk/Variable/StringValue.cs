// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
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

        public override VarType VarType => VarType.String;
        public override string ToString() => Value;
        public override int ToInteger() => ToInteger(Value);
        public override double ToFloat() => ToFloat(Value);
        public override bool IsFloat() => IsFloat(Value);

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

        public override Variable Add(Variable value) => (IsFloat() || value.IsFloat()) ? new Variable(ToFloat() + value.ToFloat()) : new Variable(ToInteger() + value.ToInteger());
        public override Variable Add(string value) => (IsFloat() || IsFloat(value)) ? new Variable(ToFloat() + ToFloat(value)) : new Variable(ToInteger() + ToInteger(value));
        public override Variable Add(int value) => IsFloat() ? new Variable(ToFloat() + value) : new Variable(ToInteger() + value);
        public override Variable Add(double value) => new(ToFloat() + value);

        public override Variable Subtract(Variable value) => (IsFloat() || value.IsFloat()) ? new Variable(ToFloat() - value.ToFloat()) : new Variable(ToInteger() - value.ToInteger());
        public override Variable Subtract(string value) => (IsFloat() || IsFloat(value)) ? new Variable(ToFloat() - ToFloat(value)) : new Variable(ToInteger() - ToInteger(value));
        public override Variable Subtract(int value) => IsFloat() ? new Variable(ToFloat() - value) : new Variable(ToInteger() - value);
        public override Variable Subtract(double value) => new(ToFloat() - value);

        public override Variable Multiply(Variable value) => (IsFloat() || value.IsFloat()) ? new Variable(ToFloat() * value.ToFloat()) : new Variable(ToInteger() * value.ToInteger());
        public override Variable Multiply(string value) => (IsFloat() || IsFloat(value)) ? new Variable(ToFloat() * ToFloat(value)) : new Variable(ToInteger() * ToInteger(value));
        public override Variable Multiply(int value) => IsFloat() ? new Variable(ToFloat() * value) : new Variable(ToInteger() * value);
        public override Variable Multiply(double value) => new(ToFloat() * value);

        public override Variable Divide(Variable value) => (IsFloat() || value.IsFloat()) ? new Variable(Div(ToFloat(), value.ToFloat())) : new Variable(Div(ToInteger(), value.ToInteger()));
        public override Variable Divide(string value) => (IsFloat() || IsFloat(value)) ? new Variable(Div(ToFloat(), ToFloat(value))) : new Variable(Div(ToInteger(), ToInteger(value)));
        public override Variable Divide(int value) => IsFloat() ? new Variable(Div(ToFloat(), value)) : new Variable(Div(ToInteger(), value));
        public override Variable Divide(double value) => new(Div(ToFloat(), value));

        public override Variable Power(Variable value) => new(Math.Pow(ToFloat(), value.ToFloat()));
        public override Variable Power(string value) => new(Math.Pow(ToFloat(), ToFloat(value)));
        public override Variable Power(int value) => new(Math.Pow(ToFloat(), value));
        public override Variable Power(double value) => new(Math.Pow(ToFloat(), value));

        public override Variable Modulus(Variable value) => (IsFloat() || value.IsFloat()) ? new Variable(Mod(ToFloat(), value.ToFloat())) : new Variable(Mod(ToInteger(), value.ToInteger()));
        public override Variable Modulus(string value) => (IsFloat() || IsFloat(value)) ? new Variable(Mod(ToFloat(), ToFloat(value))) : new Variable(Mod(ToInteger(), ToInteger(value)));
        public override Variable Modulus(int value) => IsFloat() ? new Variable(Mod(ToFloat(), value)) : new Variable(Mod(ToInteger(), value));
        public override Variable Modulus(double value) => new(Mod(ToFloat(), value));

        public override Variable Concat(Variable value) => new(Value + value.ToString());
        public override Variable Concat(string value) => new(Value + value);
        public override Variable Concat(int value) => new(Value + value.ToString());
        public override Variable Concat(double value) => new(Value + value.ToString());

        public override Variable Negate() => IsFloat() ? new Variable(-ToFloat()) : new Variable(-ToInteger());

        #endregion

        #region Comparisons

        public override int CompareTo(string value) => Value.CompareTo(value);
        public override int CompareTo(int value)
        {
            if (IsNumber(Value, out double v, out bool isFloat))
            {
                if (isFloat)
                    return v.CompareTo(value);
                else
                    return ((int)v).CompareTo(value);
            }
            return Value.CompareTo(value.ToString());
        }
        public override int CompareTo(double value)
        {
            if (IsNumber(Value, out double v))
                return v.CompareTo(value);
            return Value.CompareTo(value.ToString());
        }

        public override bool IsTrue() => Boolean.IsTrue(Value);
        public override bool IsFalse() => Boolean.IsFalse(Value);

        public override bool Equals(Value value) => value is StringValue stringValue && Value == stringValue.Value;

        #endregion

    }
}
