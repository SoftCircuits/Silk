// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.Silk;
using System;
using System.IO;
using System.Text;

namespace SilkTests
{
    [TestClass]
    public class TestCompileMethods
    {
        private readonly string Source = @"main()
{
    return (10 + 5) / 2 + 3
}";

        [TestMethod]
        public void Test()
        {
            Compiler compiler = new();
            string tempFile = Path.GetTempFileName();
            bool result;

            try
            {
                // Write source to temp file
                File.WriteAllText(tempFile, Source);

                // Test as file
                result = compiler.Compile(tempFile, out CompiledProgram? program);
                Assert.IsTrue(result);
                Assert.AreEqual(10, RunProgram(program!));

                result = compiler.Compile(tempFile, Encoding.UTF8, out program);
                Assert.IsTrue(result);
                Assert.AreEqual(10, RunProgram(program!));

                result = compiler.Compile(tempFile, true, out program);
                Assert.IsTrue(result);
                Assert.AreEqual(10, RunProgram(program!));

                result = compiler.Compile(tempFile, Encoding.UTF8, true, out program);
                Assert.IsTrue(result);
                Assert.AreEqual(10, RunProgram(program!));

                using (FileStream stream = File.OpenRead(tempFile))
                {
                    result = compiler.Compile(stream, out program);
                    Assert.IsTrue(result);
                    Assert.AreEqual(10, RunProgram(program!));
                }

                using (FileStream stream = File.OpenRead(tempFile))
                {
                    result = compiler.Compile(stream, Encoding.UTF8, out program);
                    Assert.IsTrue(result);
                    Assert.AreEqual(10, RunProgram(program!));
                }

                using (FileStream stream = File.OpenRead(tempFile))
                {
                    result = compiler.Compile(stream, true, out program);
                    Assert.IsTrue(result);
                    Assert.AreEqual(10, RunProgram(program!));
                }

                using (FileStream stream = File.OpenRead(tempFile))
                {
                    result = compiler.Compile(stream, Encoding.UTF8, true, out program);
                    Assert.IsTrue(result);
                    Assert.AreEqual(10, RunProgram(program!));
                }

                result = compiler.CompileSource(Source, out program);
                Assert.IsTrue(result);
                Assert.AreEqual(10, RunProgram(program!));

                // Exception when no logfile or source file given
                compiler.CreateLogFile = true;
                Assert.ThrowsException<InvalidOperationException>(() => compiler.CompileSource(Source, out program));

                compiler.LogFile = tempFile;
                result = compiler.CompileSource(Source, out program);
                Assert.IsTrue(result);
                Assert.IsTrue(File.Exists(tempFile));
                Assert.AreEqual(10, RunProgram(program!));
            }
            finally
            {
                File.Delete(tempFile);
            }
        }

        private static Variable RunProgram(CompiledProgram program) => new Runtime(program).Execute();
    }
}
