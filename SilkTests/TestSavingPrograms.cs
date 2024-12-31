// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.Silk;
using System.IO;

namespace SilkTests
{
    [TestClass]
    public class TestSavingPrograms
    {
        private readonly string Source = @"///////////////////////////////////////////////////////////////
// Silk - Sample script
//

// Variables declared here are global to all functions
var array = { 20, 30, ""abc"", 40, ""def"", 50, [5], { 1, 2, 3 } }

main()
        {
            // Set colors and clear screen
            color white, blue
    clearscreen

    // Print header
    printline string(""*"", 78)
    printline ""* SILK Example Program""
    printline string(""*"", 78)
    printline

    // Print global array
    printline ""Array: "" & array
    printline

    // Print local array
    months = {
        ""January"",
        ""February"",
        ""March"",
        ""April"",
        ""May"",
        ""June"",
        ""July"",
        ""August"",
        ""September"",
        ""October"",
        ""November"",
        ""December""
    }
    printline ""Months: "", months
    printline

    // Print each element in array
            printline ""Array Elements:""
    for i = 1 to len(array)
        printline ""  \"""" & array[i] & ""\""""
    printline

    // Expressions
    printline ""Expressions:""
    printline ""  2 + 2 = "", 2 + 2
    printline ""  50 / (2 + 3) = "", 50 / (2 + 3)
    printline ""  (2 + 7) * (2 * (8 + double(5) * (100 + triple(7)))) = "", (2 + 7) * (2 * (8 + double(5) * (100 + triple(7))))
    printline

    // Characters
    printline ""Characters:""
    for i = 48 to 126
        print chr(i)
    printline

    return 5000
}

double (x)
{
    return x* 2
}

triple(x)
{
    return x * 3
}";

        [TestMethod]
        public void Test()
        {
            string path = Path.GetTempFileName();

            try
            {
                Compiler compiler = new();

                // Define symbols used in source to prevent errors
                compiler.RegisterFunction("Color", 1, 2);
                compiler.RegisterFunction("ClearScreen", 0, 0);
                compiler.RegisterFunction("Print", 0, 99);
                compiler.RegisterFunction("PrintLine", 0, 99);
                compiler.RegisterVariable("White", new Variable());
                compiler.RegisterVariable("Blue", new Variable());
                Assert.IsTrue(compiler.CompileSource(Source, out CompiledProgram? program));

                program!.Save(path);
                program = CompiledProgram.FromFile(path);

                Runtime runtime = new(program!);

                Assert.AreEqual(5000, runtime.Execute());
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
