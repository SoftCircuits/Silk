// Copyright (c) 2019 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using SoftCircuits.Silk;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TestSilk
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"D:\Users\Jonathan\source\repos\Silk\TestSilk\Sample.txt";

            Compiler compiler = new Compiler();
            compiler.CreateLogFile = true;
            compiler.EnableLineNumbers = true;

            // Register intrinsic functions and variables
            compiler.RegisterFunction("Print", 0, Function.NoParameterLimit);
            compiler.RegisterFunction("PrintLn", 0, Function.NoParameterLimit);
            compiler.RegisterFunction("Color", 1, 2);
            compiler.RegisterFunction("ClearScreen", 0, 0);
            compiler.RegisterFunction("ReadLn", 0, 0);
            foreach (var color in Enum.GetValues(typeof(ConsoleColor)))
                compiler.RegisterVariable(color.ToString(), new Variable((int)color));

            if (compiler.Compile(path, out CompiledProgram program))
            {
                // Success
                Console.WriteLine("COMPILE SUCCEEDED!");
                Console.WriteLine();

                Runtime runtime = new Runtime();
                runtime.Begin += Runtime_Begin;
                runtime.Function += Runtime_Function;
                runtime.End += Runtime_End;

                Variable result = runtime.Execute(program);

                Console.WriteLine();
                Console.WriteLine($"Program ran successfully with exit code {result}.");
            }
            else
            {
                Console.WriteLine("COMPILE FAILED");
                Console.WriteLine();
                foreach (var error in compiler.Errors)
                    Console.WriteLine(error.ToString());
            }
        }

        private static void Runtime_Begin(object sender, BeginEventArgs e)
        {
            e.UserData = null;
        }

        private static void Runtime_Function(object sender, FunctionEventArgs e)
        {
            switch (e.Name)
            {
                case "Print":
                    foreach (var v in e.Parameters)
                        Console.Write(v.ToString());
                    break;
                case "PrintLn":
                    foreach (var v in e.Parameters)
                        Console.Write(v.ToString());
                    Console.WriteLine();
                    break;
                case "Color":
                    Debug.Assert(e.Parameters.Length >= 1);
                    Debug.Assert(e.Parameters.Length <= 2);
                    if (e.Parameters.Length >= 1)
                        Console.ForegroundColor = (ConsoleColor)e.Parameters[0].ToInteger();
                    if (e.Parameters.Length >= 2)
                        Console.BackgroundColor = (ConsoleColor)e.Parameters[1].ToInteger();
                    break;
                case "ClearScreen":
                    Console.Clear();
                    break;
                case "ReadLn":
                    e.ReturnValue.SetValue(Console.ReadLine());
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        private static void Runtime_End(object sender, EndEventArgs e)
        {
        }
    }
}
