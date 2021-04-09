// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;

namespace SoftCircuits.Silk
{
    public class BeginEventArgs : EventArgs
    {
        public object? UserData { get; set; }
    }

    public class EndEventArgs : EventArgs
    {
        public object? UserData { get; set; }
    }

    public class FunctionEventArgs : EventArgs
    {
        public string Name { get; internal set; }
        public Variable[] Parameters { get; internal set; }
        public Variable ReturnValue { get; internal set; }
        public object? UserData { get; set; }

        public FunctionEventArgs(string name, Variable[] parameters, Variable returnValue, object? userData = null)
        {
            Name = name;
            Parameters = parameters;
            ReturnValue = returnValue;
            UserData = userData;
        }
    }

    /// <summary>
    /// Error event arguments.
    /// </summary>
    internal class ErrorEventArgs : EventArgs
    {
        public ErrorCode ErrorCode { get; set; }
        public string? Token { get; set; }
    }
}
