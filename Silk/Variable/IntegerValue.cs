// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
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

        public override VarType VarType => VarType.Integer;
        public override string ToString() => Value.ToString();
        public override int ToInteger() => Value;
        public override double ToFloat() => Value;

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

        public override Variable Add(Variable value) => value.IsFloat() ? new Variable(Value + value.ToFloat()) : new Variable(Value + value.ToInteger());
        public override Variable Add(string value) => IsFloat(value) ? new Variable(Value + ToFloat(value)) : new Variable(Value + ToInteger(value));
        public override Variable Add(int value) => new(Value + value);
        public override Variable Add(double value) => new(Value + value);

        public override Variable Subtract(Variable value) => value.IsFloat() ? new Variable(Value - value.ToFloat()) : new Variable(Value - value.ToInteger());
        public override Variable Subtract(string value) => IsFloat(value) ? new Variable(Value - ToFloat(value)) : new Variable(Value - ToInteger(value));
        public override Variable Subtract(int value) => new(Value - value);
        public override Variable Subtract(double value) => new(Value - value);

        public override Variable Multiply(Variable value) => value.IsFloat() ? new Variable(Value * value.ToFloat()) : new Variable(Value * value.ToInteger());
        public override Variable Multiply(string value) => IsFloat(value) ? new Variable(Value * ToFloat(value)) : new Variable(Value * ToInteger(value));
        public override Variable Multiply(int value) => new(Value * value);
        public override Variable Multiply(double value) => new(Value * value);

        public override Variable Divide(Variable value) => value.IsFloat() ? new Variable(Div(Value, value.ToFloat())) : new Variable(Div(Value, value.ToInteger()));
        public override Variable Divide(string value) => IsFloat(value) ? new Variable(Div(Value, ToFloat(value))) : new Variable(Div(Value, ToInteger(value)));
        public override Variable Divide(int value) => new(Div(Value, value));
        public override Variable Divide(double value) => new(Div(Value, value));

        public override Variable Power(Variable value) => new(Math.Pow(Value, value.ToFloat()));
        public override Variable Power(string value) => new(Math.Pow(Value, ToFloat(value)));
        public override Variable Power(int value) => new(Math.Pow(Value, value));
        public override Variable Power(double value) => new(Math.Pow(Value, value));

        public override Variable Modulus(Variable value) => value.IsFloat() ? new Variable(Mod(Value, value.ToFloat())) : new Variable(Mod(Value, value.ToInteger()));
        public override Variable Modulus(string value) => IsFloat(value) ? new Variable(Mod(Value, ToFloat(value))) : new Variable(Mod(Value, ToInteger(value)));
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
            if (IsNumber(value, out double v, out bool isFloat))
            {
                if (isFloat)
                    return ((double)Value).CompareTo(v);
                else
                    return Value.CompareTo((int)v);
            }
            return Value.ToString().CompareTo(value);
        }
        public override int CompareTo(int value) => Value.CompareTo(value);
        public override int CompareTo(double value) => ((double)Value).CompareTo(value);

        public override bool IsTrue() => Boolean.IsTrue(Value);
        public override bool IsFalse() => Boolean.IsFalse(Value);

        public override bool Equals(Value value) => value is IntegerValue integerValue && Value == integerValue.Value;

        #endregion

    }
}
