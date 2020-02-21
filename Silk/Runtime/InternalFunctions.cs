// Copyright (c) 2019-2020 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SoftCircuits.Silk
{
    internal class InternalFunctionInfo
    {
        public Action<Variable[], Variable> Action { get; private set; }
        public int MinParameters { get; set; }
        public int MaxParameters { get; set; }

        public InternalFunctionInfo(Action<Variable[], Variable> action, int minParameters = Function.NoParameterLimit, int maxParameters = Function.NoParameterLimit)
        {
            Action = action;
            MinParameters = minParameters;
            MaxParameters = maxParameters;
        }
    }

    /// <summary>
    /// Internal intrinsic functions.
    /// </summary>
    internal static class InternalFunctions
    {
        public static Dictionary<string, InternalFunctionInfo> InternalFunctionLookup = new Dictionary<string, InternalFunctionInfo>(StringComparer.OrdinalIgnoreCase)
        {
            ["Abs"] = new InternalFunctionInfo(Abs, 1, 1), 
            ["Acos"] = new InternalFunctionInfo(Acos, 1, 1),
            ["Asc"] = new InternalFunctionInfo(Asc, 1, 1),
            ["Atn"] = new InternalFunctionInfo(Atn, 1, 1),
            ["Avg"] = new InternalFunctionInfo(Avg, 1, Function.NoParameterLimit),
            ["Bin"] = new InternalFunctionInfo(Bin, 1, 1),
            ["Chr"] = new InternalFunctionInfo(Chr, 1, 1),
            ["Cos"] = new InternalFunctionInfo(Cos, 1, 1),
            ["Date"] = new InternalFunctionInfo(Date, 0, 0),
            ["Environ"] = new InternalFunctionInfo(Environ, 1, 1),
            ["Exp"] = new InternalFunctionInfo(Exp, 1, 1),
            ["Float"] = new InternalFunctionInfo(Float, 1, 1),
            ["Hex"] = new InternalFunctionInfo(Hex, 1, 1),
            ["InStr"] = new InternalFunctionInfo(InStr, 2, 3),
            ["Int"] = new InternalFunctionInfo(Int, 1, 1),
            ["IsList"] = new InternalFunctionInfo(IsList, 1, 1),
            ["Len"] = new InternalFunctionInfo(Len, 1, 1),
            ["Left"] = new InternalFunctionInfo(Left, 2, 2),
            ["Max"] = new InternalFunctionInfo(Max, 1, Function.NoParameterLimit),
            ["Mid"] = new InternalFunctionInfo(Mid, 2, 3),
            ["Min"] = new InternalFunctionInfo(Min, 1, Function.NoParameterLimit),
            ["Oct"] = new InternalFunctionInfo(Oct, 1, 1),
            ["Right"] = new InternalFunctionInfo(Right, 2, 2),
            ["Round"] = new InternalFunctionInfo(Round, 1, 1),
            ["Sin"] = new InternalFunctionInfo(Sin, 1, 1),
            ["Sqr"] = new InternalFunctionInfo(Sqr, 1, 1),
            ["String"] = new InternalFunctionInfo(String, 2, 2),
            ["Tan"] = new InternalFunctionInfo(Tan, 1, 1),
            ["Time"] = new InternalFunctionInfo(Time, 0, 0),
        };

        /// <summary>
        /// Internal intrinsic variables.
        /// </summary>
        public static Dictionary<string, Variable> InternalVariableLookup = new Dictionary<string, Variable>(StringComparer.OrdinalIgnoreCase)
        {
            ["E"] = new Variable(Math.E),
            ["PI"] = new Variable(Math.PI),
        };

        /// <summary>
        /// Adds internal functions and variables to the given collections. Does not overwrite any existing
        /// items with the same name.
        /// </summary>
        public static void AddInternalFunctionsAndVariables(OrderedDictionary<string, Function> functions, OrderedDictionary<string, Variable> variables)
        {
            // Add internal functions (don't override host app's functions)
            foreach (var key in InternalFunctions.InternalFunctionLookup.Keys)
            {
                if (!functions.ContainsKey(key))
                {
                    var info = InternalFunctions.InternalFunctionLookup[key];
                    functions.Add(key, new InternalFunction(key, info.Action, info.MinParameters, info.MaxParameters));
                }
            }
            // Add internal variables (don't override host app's variables)
            foreach (var key in InternalFunctions.InternalVariableLookup.Keys)
            {
                if (!variables.ContainsKey(key))
                {
                    var v = InternalFunctions.InternalVariableLookup[key];
                    variables.Add(key, new Variable(v));
                }
            }
        }

        /// <summary>
        /// Returns the absolute value of the given number.
        /// </summary>
        private static void Abs(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            if (parameters[0].IsFloat())
                returnValue.SetValue(Math.Abs(parameters[0].ToFloat()));
            else
                returnValue.SetValue(Math.Abs(parameters[0].ToInteger()));
        }

        /// <summary>
        /// Returns the angle whose cosine is the specified number.
        /// </summary>
        private static void Acos(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(Math.Acos(parameters[0].ToFloat()));
        }

        /// <summary>
        /// Returns the ASCII/Unicode value of the first character in
        /// the given string.
        /// </summary>
        private static void Asc(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            string s = parameters[0].ToString();
            returnValue.SetValue(s.Length > 0 ? s[0] : 0);
        }

        /// <summary>
        /// Returns an angle whose tangent is the specified number.
        /// </summary>
        private static void Atn(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(Math.Atan(parameters[0].ToFloat()));
        }

        /// <summary>
        /// Returns the average of the given arguments.
        /// </summary>
        private static void Avg(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length >= 1);
            var variables = FlattenVariableList(parameters);
            if (variables.Count() > 0)
            {
                if (variables.Any(p => p.IsFloat()))
                {
                    double total = 0;
                    foreach (Variable variable in variables)
                        total += variable.ToFloat();
                    returnValue.SetValue(total / variables.Count());
                }
                else
                {
                    int total = 0;
                    foreach (Variable variable in variables)
                        total += variable.ToInteger();
                    returnValue.SetValue(total / variables.Count());
                }
            }
        }

        /// <summary>
        /// Converts the given value to a binary string.
        /// </summary>
        private static void Bin(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(Convert.ToString(parameters[0].ToInteger(), 2));
        }

        /// <summary>
        /// Creates a string with one character with the specified ASCII/
        /// Unicode value.
        /// </summary>
        private static void Chr(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(((char)parameters[0].ToInteger()).ToString());
        }

        /// <summary>
        /// Returns the cosine of the specified value.
        /// </summary>
        private static void Cos(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(Math.Cos(parameters[0].ToFloat()));
        }

        /// <summary>
        /// Returns a string with the current date.
        /// </summary>
        private static void Date(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 0);
            returnValue.SetValue(DateTime.Now.ToString("D"));
        }

        /// <summary>
        /// Returns the value of the specified environment variable.
        /// </summary>
        private static void Environ(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(Environment.GetEnvironmentVariable(parameters[0].ToString() ?? string.Empty));
        }

        /// <summary>
        /// Returns e raised to the specified power.
        /// </summary>
        private static void Exp(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(Math.Exp(parameters[0].ToFloat()));
        }

        /// <summary>
        /// Converts the given value to a floating point number.
        /// </summary>
        private static void Float(Variable[] parameters, Variable returnValue)
        {
            returnValue.SetValue(parameters[0].ToFloat());
        }

        /// <summary>
        /// Returns a hexedecimal string equal to the given value.
        /// </summary>
        private static void Hex(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(parameters[0].ToInteger().ToString("X"));
        }

        /// <summary>
        /// Returns the first 1-based position where the second string appears within the first
        /// string. An optional 3rd argument specifies the 1-based position to begin the search.
        /// Returns 0 if the string is not found.
        /// </summary>
        private static void InStr(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length >= 2);
            int start = (parameters.Length >= 3) ? Math.Max(parameters[2].ToInteger() - 1, 0) : 0;
            int match = parameters[0].ToString().IndexOf(parameters[1].ToString(), start);
            returnValue.SetValue(match + 1);
        }

        /// <summary>
        /// Converts the given value to an integer, truncating any fractional portion.
        /// </summary>
        private static void Int(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue((int)parameters[0].ToFloat());
        }

        /// <summary>
        /// Returns true if the given variable is a list.
        /// </summary>
        private static void IsList(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(parameters[0].IsList ? Boolean.True : Boolean.False);
        }

        /// <summary>
        /// Returns the left-most number of characters specified by the second argument
        /// from the string specified by the first argument.
        /// </summary>
        private static void Left(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 2);
            string s = parameters[0].ToString();
            int len = parameters[1].ToInteger();
            if (s.Length > len)
                returnValue.SetValue(s.Substring(0, len));
            returnValue.SetValue(s);
        }

        /// <summary>
        /// Returns the string length of the given value. If the argument is a list,
        /// the number of items in the list is returned. Otherwise, the number of
        /// characters in the value converted to a string is returned.
        /// </summary>
        private static void Len(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            if (parameters[0].IsList)
                returnValue.SetValue(parameters[0].ListCount);
            else
                returnValue.SetValue(parameters[0].ToString().Length);
        }

        /// <summary>
        /// Returns the maximum value of the given arguments.
        /// </summary>
        private static void Max(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length >= 1);
            var variables = FlattenVariableList(parameters);
            if (variables.Count() > 0)
            {
                if (variables.Any(p => p.IsFloat()))
                {
                    double max = int.MinValue;
                    foreach (Variable variable in variables)
                    {
                        double value = variable.ToFloat();
                        if (value > max)
                            max = value;
                    }
                    returnValue.SetValue(max);
                }
                else
                {
                    int max = int.MinValue;
                    foreach (Variable variable in variables)
                    {
                        int value = variable.ToInteger();
                        if (value > max)
                            max = value;
                    }
                    returnValue.SetValue(max);
                }
            }
        }

        /// <summary>
        /// Returns a section of the string given as the first argument. The second
        /// argument is the 1-based position where the string should be extracted. If
        /// a third argument is given, it specifies the maximum number of string to
        /// return.
        /// </summary>
        private static void Mid(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length >= 2);
            string s = parameters[0].ToString();
            int start = Math.Max(parameters[1].ToInteger() - 1, 0);
            int count = (parameters.Length > 2) ? Math.Min(parameters[2].ToInteger(), s.Length - start) : (s.Length - start);
            returnValue.SetValue(s.Substring(start, count));
        }

        /// <summary>
        /// Returns the minimum of the given arguments.
        /// </summary>
        private static void Min(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length >= 1);
            var variables = FlattenVariableList(parameters);
            if (variables.Count() > 0)
            {
                if (variables.Any(p => p.IsFloat()))
                {
                    double min = int.MaxValue;
                    foreach (Variable variable in variables)
                    {
                        double value = variable.ToFloat();
                        if (value < min)
                            min = value;
                    }
                    returnValue.SetValue(min);
                }
                else
                {
                    int min = int.MaxValue;
                    foreach (Variable variable in variables)
                    {
                        int value = variable.ToInteger();
                        if (value < min)
                            min = value;
                    }
                    returnValue.SetValue(min);
                }
            }
        }

        /// <summary>
        /// Converts the given value to an octal string.
        /// </summary>
        private static void Oct(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(Convert.ToString(parameters[0].ToInteger(), 8));
        }

        /// <summary>
        /// Returns the right-most number of characters specified by the second argument
        /// from the string specified by the first argument.
        /// </summary>
        private static void Right(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 2);
            string s = parameters[0].ToString();
            returnValue.SetValue(s.Substring(s.Length - parameters[1].ToInteger()));
        }

        /// <summary>
        /// Rounds the given value to the nearest integer.
        /// </summary>
        private static void Round(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(parameters[0].ToInteger());
        }

        /// <summary>
        /// Returns the sign of the specified angle.
        /// </summary>
        private static void Sin(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(Math.Sin(parameters[0].ToFloat()));
        }

        /// <summary>
        /// Returns the square root of the given argument.
        /// </summary>
        private static void Sqr(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(Math.Sqrt(parameters[0].ToFloat()));
        }

        /// <summary>
        /// Returns a string consisting of the first argument repeated the number of times
        /// specified by the second argument. If the first argument is not a string, the
        /// result will consist of characters with the ASCII/Unicode value of this argument.
        /// </summary>
        private static void String(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 2);
            string s = (parameters[0].Type == ValueType.String) ?
                parameters[0].ToString() :
                ((char)parameters[0].ToInteger()).ToString();
            StringBuilder builder = new StringBuilder(s.Length * Math.Max(0, parameters[1].ToInteger()));
            for (int i = 0; i < parameters[1].ToInteger(); i++)
                builder.Append(s);
            returnValue.SetValue(builder.ToString());
        }

        /// <summary>
        /// Returns the tanget of the specified angle.
        /// </summary>
        private static void Tan(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 1);
            returnValue.SetValue(Math.Tan(parameters[0].ToFloat()));
        }

        /// <summary>
        /// Returns a string with the current time.
        /// </summary>
        private static void Time(Variable[] parameters, Variable returnValue)
        {
            Debug.Assert(parameters.Length == 0);
            returnValue.SetValue(DateTime.Now.ToString("T"));
        }

        #region Helper methods

        /// <summary>
        /// Returns the given collection as a flattened list of variables. Unpacks
        /// variables that are lists, and list variables that contain other lists
        /// into a single list.
        /// </summary>
        /// <param name="variables">The list to flatten.</param>
        private static IEnumerable<Variable> FlattenVariableList(IEnumerable<Variable> variables)
        {
            foreach (var variable in variables)
            {
                if (variable.IsList)
                {
                    foreach (var variable2 in FlattenVariableList(variable.GetList()))
                        yield return variable2;
                }
                else yield return variable;
            }
        }

        #endregion

    }
}
