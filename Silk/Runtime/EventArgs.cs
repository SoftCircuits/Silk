// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;

namespace SoftCircuits.Silk
{
    /// <summary>
    /// Represents the arguments for the <see cref="Runtime.Begin"/> event.
    /// </summary>
    public class BeginEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets user-defined data.
        /// </summary>
        public object? UserData { get; set; }
    }

    /// <summary>
    /// Represents the arguments for the <see cref="Runtime.End"/> event.
    /// </summary>
    public class EndEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets user-defined data.
        /// </summary>
        public object? UserData { get; set; }
    }

    /// <summary>
    /// Represents the arguments for the <see cref="Runtime.Function"/> event.
    /// </summary>
    /// <remarks>
    /// Constructs a new <see cref="FunctionEventArgs"/> instance.
    /// </remarks>
    /// <param name="name">The function name.</param>
    /// <param name="parameters">The function parameters.</param>
    /// <param name="returnValue">Holds the function return value.</param>
    /// <param name="userData">Optional user data.</param>
    public class FunctionEventArgs(string name, Variable[] parameters, Variable returnValue, object? userData = null) : EventArgs
    {
        /// <summary>
        /// Gets the function name.
        /// </summary>
        public string Name { get; internal set; } = name;
        /// <summary>
        /// Gets the function parameters.
        /// </summary>
        public Variable[] Parameters { get; internal set; } = parameters;
        /// <summary>
        /// Holds the function return value.
        /// </summary>
        public Variable ReturnValue { get; internal set; } = returnValue;
        /// <summary>
        /// Gets or sets user-defined data.
        /// </summary>
        public object? UserData { get; set; } = userData;
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
