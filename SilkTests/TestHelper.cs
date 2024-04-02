// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.Silk;

namespace SilkTests
{
    public class ProgramTest
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Description { get; set; }
        public Variable Result { get; set; }
        public string Source { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }

    public static class TestHelper
    {
        public static Variable RunScript(string script)
        {
            Compiler compiler = new()
            {
                CreateLogFile = true,
                LogFile = @"C:\Users\jwood\OneDrive\Desktop\Logfile.txt"
            };

            Assert.IsTrue(compiler.CompileSource(script, out CompiledProgram? program));
            Runtime runtime = new(program!);
            return runtime.Execute();
        }

        //public static bool TestCompile(string script, out List<Error> errors)
        //{
        //    Compiler compiler = new();
        //    bool result = compiler.CompileSource(script, out CompiledProgram? _);
        //    errors = compiler.Errors;
        //    return result;
        //}
    }
}
