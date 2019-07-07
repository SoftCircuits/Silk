// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Diagnostics;

namespace SoftCircuits.Silk
{
    public enum FuntionType
    {
        Internal,
        Intrinsic,
        User
    }

    /// <summary>
    /// This class is made public so const values can be accessed externally.
    /// </summary>
    public abstract class Function
    {
        /// <summary>
        /// Signifies not restriction on the number of parameters sent to intrinsic
        /// functions.
        /// </summary>
        public const int NoParameterLimit = -1;

        /// <summary>
        /// Name of Main function. This function must be defined and is where execution
        /// starts.
        /// </summary>
        public const string Main = "main";

        /// <summary>
        /// Function name
        /// </summary>
        public string Name { get; set; }

        public virtual bool IsIntrinsic => (this is IntrinsicFunction);

        public Function(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Represents a user function (a function defined in source code).
    /// </summary>
    internal class UserFunction : Function
    {
        public int IP { get; set; }
        public int NumVariables { get; set; }
        public int NumParameters { get; set; }

        public UserFunction(string name, int ip)
            : base(name)
        {
            IP = ip;
            NumVariables = 0;
            NumParameters = 0;
        }

        public UserFunction(CompileTimeUserFunction function)
            : base(function.Name)
        {
            IP = function.IP;
            Debug.Assert(function.Variables != null);
            Debug.Assert(function.Parameters != null);
            NumVariables = function.Variables.Count;
            NumParameters = function.Parameters.Count;
        }
    }

    /// <summary>
    /// Represents a user function during compilation.
    /// </summary>
    internal class CompileTimeUserFunction : UserFunction
    {
        public OrderedDictionary<string, Label> Labels { get; set; }
        public OrderedDictionary<string, Variable> Variables { get; set; }
        public OrderedDictionary<string, Variable> Parameters { get; set; }

        public CompileTimeUserFunction(string name, int ip)
            : base(name, ip)
        {
            Labels = new OrderedDictionary<string, Label>(StringComparer.OrdinalIgnoreCase);
            Variables = new OrderedDictionary<string, Variable>(StringComparer.OrdinalIgnoreCase);
            Parameters = new OrderedDictionary<string, Variable>(StringComparer.OrdinalIgnoreCase);
        }

        public CompileTimeUserFunction(UserFunction function)
            : base(function.Name, function.IP)
        {
            NumVariables = 0;
            NumParameters = 0;
            Labels = new OrderedDictionary<string, Label>(StringComparer.OrdinalIgnoreCase);
            Variables = new OrderedDictionary<string, Variable>(StringComparer.OrdinalIgnoreCase);
            Parameters = new OrderedDictionary<string, Variable>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the ID of the local variable or parameter with the given name.
        /// The variable is added if not found.
        /// </summary>
        /// <param name="name">Variable name.</param>
        internal int GetVariableId(string name)
        {
            // First search parameters
            int index = Parameters.IndexOf(name);
            if (index >= 0)
                return index | (int)ByteCodeVariableFlag.Parameter;
            // Next search local variables
            index = Variables.IndexOf(name);
            if (index >= 0)
                return index | (int)ByteCodeVariableFlag.Local;
            // Variable not defined; create it
            return Variables.Add(name, new Variable()) | (int)ByteCodeVariableFlag.Local;
        }
    }

    /// <summary>
    /// Represents an intrinsic function defined by the host program.
    /// </summary>
    internal class IntrinsicFunction : Function
    {
        public int MinParameters { get; set; }
        public int MaxParameters { get; set; }

        public IntrinsicFunction(string name, int minParameters = NoParameterLimit, int maxParameters = NoParameterLimit)
            : base(name)
        {
            MinParameters = minParameters;
            MaxParameters = maxParameters;
        }

        /// <summary>
        /// Returns true if the given argument count is valid for this function's MinParameter
        /// and MaxParameter values. If false is returned, <paramref name="message"/> is set to
        /// a description of how the count is invalid.
        /// </summary>
        /// <param name="count">Argument count.</param>
        /// <param name="message">Set to description of how the count is invalid if this method returns false.</param>
        internal bool IsParameterCountValid(int count, out string message)
        {
            message = string.Empty;

            if (MinParameters == MaxParameters)
            {
                if (MinParameters != NoParameterLimit && MinParameters != count)
                {
                    message = $"Function \"{Name}\" requires {MinParameters} argument(s)";
                    return false;
                }
            }
            else
            {
                if (MinParameters != NoParameterLimit)
                {
                    if (count < MinParameters)
                    {
                        message = $"Function \"{Name}\" requires at least {MinParameters} argument(s)";
                        return false;
                    }
                }
                if (MaxParameters != NoParameterLimit)
                {
                    if (count > MaxParameters)
                    {
                        message = $"Function \"{Name}\" doesn't allow more than {MaxParameters} argument(s)";
                        return false;
                    }
                }
            }
            return true;
        }
    }

    /// <summary>
    /// Represents one of the functions internal within this library.
    /// </summary>
    internal class InternalFunction : IntrinsicFunction
    {
        public Action<Variable[], Variable> Action { get; set; }

        public InternalFunction(string name, Action<Variable[], Variable> action, int minParameters = NoParameterLimit, int maxParameters = NoParameterLimit)
            : base(name, minParameters, maxParameters)
        {
            Action = action;
        }
    }

    /// <summary>
    /// Used to track function information during compilation.
    /// </summary>
    internal class RuntimeFunction
    {
        public int IP { get; set; }
        public Variable[] Parameters { get; set; }
        public Variable[] Variables { get; set; }
        public Variable ReturnValue { get; set; }

        public RuntimeFunction(UserFunction function)
        {
            if (function == null)
                throw new ArgumentNullException(nameof(function));
            IP = function.IP;
            Parameters = new Variable[function.NumParameters];
            for (int i = 0; i < function.NumParameters; i++)
                Parameters[i] = new Variable();
            Variables = new Variable[function.NumVariables];
            for (int i = 0; i < function.NumVariables; i++)
                Variables[i] = new Variable();
            ReturnValue = new Variable();
        }
    }
}
