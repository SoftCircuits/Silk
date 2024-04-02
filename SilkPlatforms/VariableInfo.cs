// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.Silk;

namespace SilkPlatforms
{
    public class VariableInfo
    {
        public string Name { get; set; }
        public Variable Value { get; set; }

        public VariableInfo(string name)
        {
            Name = name;
            Value = new();
        }

        public VariableInfo(string name, int value)
        {
            Name = name;
            Value = new(value);
        }

        public VariableInfo(string name, double value)
        {
            Name = name;
            Value = new(value);
        }

        public VariableInfo(string name, string value)
        {
            Name = name;
            Value = new(value);
        }
    }
}
