// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.Silk;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SilkPlatforms
{
    public class RunProgram
    {
        private readonly Platform Platform;
        private readonly Dictionary<string, Action<Variable[], Variable>> FunctionLookup;

        public readonly List<Error> Errors;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="platform"></param>
        public RunProgram(SilkPlatform platform)
        {
            Platform = GetPlatform(platform);
            FunctionLookup = Platform.Functions.ToDictionary(f => f.Name, f => f.Handler, StringComparer.OrdinalIgnoreCase);
            Errors = new();
        }

        /// <summary>
        /// Returns the description for the specified <see cref="SilkPlatform"/>.
        /// </summary>
        /// <param name="platform"></param>
        /// <returns></returns>
        public static string GetPlatformDescription(SilkPlatform platform) => GetEnumDescription(platform);

        /// <summary>
        /// Compiles and runs the given source code using the current platform.
        /// </summary>
        /// <param name="source">The program source code.</param>
        /// <returns>True if successful, false and sets <see cref="Errors"/>
        /// if compile errors were detected.</returns>
        public bool Run(string source)
        {
            Compiler compiler = new();

            //compiler.CreateLogFile = true;
            //compiler.LogFile = @"D:\Users\jwood\Desktop\Logfile.txt";

            foreach (FunctionInfo info in Platform.Functions)
                compiler.RegisterFunction(info.Name, info.MinParameters, info.MaxParameters);
            foreach (VariableInfo info in Platform.Variables)
                compiler.RegisterVariable(info.Name, info.Value);

            Errors.Clear();
            if (!compiler.CompileSource(source, out CompiledProgram? program))
            {
                Errors.AddRange(compiler.Errors);
                return false;
            }

            Runtime runtime = new(program);
            runtime.Begin += Runtime_Begin;
            runtime.End += Runtime_End;
            runtime.Function += Runtime_Function;

            if (Platform.Form != null)
            {
                Platform.Form.Runtime = runtime;
                Platform.Form.ShowDialog(); // Performs clean up on unload
            }
            else
            {
                runtime.Execute();
                Platform.CleanUp();
            }
            return true;
        }

        private void Runtime_Begin(object? sender, BeginEventArgs e)
        {
            Platform.Begin();
        }

        private void Runtime_End(object? sender, EndEventArgs e)
        {
            Platform.End();
        }

        private void Runtime_Function(object? sender, FunctionEventArgs e)
        {
            if (FunctionLookup.TryGetValue(e.Name, out Action<Variable[], Variable>? action))
                action(e.Parameters, e.ReturnValue);
            else
                Debug.Assert(false);    // Unknown function
        }

        #region Helper methods

        private static Platform GetPlatform(SilkPlatform platform)
        {
            return platform switch
            {
                SilkPlatform.Console => new ConsolePlatform(),
                SilkPlatform.Graphics => new GraphicsPlatform(),
                _ => throw new InvalidOperationException("Unknown platform specified."),
            };
        }

        private static string GetEnumDescription<T>(T value) where T : Enum
        {
            Type type = value.GetType();
            FieldInfo? fieldInfo = type.GetField(value.ToString());
            if (fieldInfo != null)
            {
                IEnumerable<DescriptionAttribute> attributes = fieldInfo.GetCustomAttributes<DescriptionAttribute>();
                if (attributes.Any())
                    return attributes.First().Description;
            }
            return value.ToString();
        }

        #endregion

    }
}
