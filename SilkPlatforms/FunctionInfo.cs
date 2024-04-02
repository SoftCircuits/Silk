// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.Silk;
using System;

namespace SilkPlatforms
{
    public class FunctionInfo
    {
        public string Name { get; set; }
        public Action<Variable[], Variable> Handler { get; set; }
        public int MinParameters { get; set; }
        public int MaxParameters { get; set; }

        public FunctionInfo(string name, Action<Variable[], Variable> handler, int minParameters, int maxParameters)
        {
            Name = name;
            Handler = handler;
            MinParameters = minParameters;
            MaxParameters = maxParameters;
        }
    }
}
