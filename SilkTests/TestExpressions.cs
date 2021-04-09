// Copyright (c) 2019-2021 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.Silk;
using System;

namespace SilkTests
{
    [TestClass]
    public class TestExpressions
    {
        [TestMethod]
        public void BasicExpressions()
        {
            // Operand evaluation
            Assert.AreEqual(5, EvaluateExpression("5"));
            Assert.AreEqual(-34, EvaluateExpression("-34"));
            Assert.AreEqual(12345.6, EvaluateExpression("12345.6"));
            Assert.AreEqual("abc", EvaluateExpression("\"abc\""));
            Assert.AreEqual("abc", EvaluateExpression("'abc'"));

            // Operators
            Assert.AreEqual(5, EvaluateExpression("2 + 3"));
            Assert.AreEqual(-1, EvaluateExpression("2 - 3"));
            Assert.AreEqual(6, EvaluateExpression("2 * 3"));
            Assert.AreEqual(0, EvaluateExpression("2 / 3"));
            double result = Math.Round((double)EvaluateExpression("2.0 / 3"), 5);
            Assert.AreEqual("0.66667", result.ToString());
            Assert.AreEqual(2, EvaluateExpression("2 % 3"));
            Assert.AreEqual(8, EvaluateExpression("2 ^ 3"));
            Assert.AreEqual(23, EvaluateExpression("2 & 3"));

            // Expressions
            Assert.AreEqual(17, EvaluateExpression("2 + 3 * 5"));
            Assert.AreEqual(25, EvaluateExpression("(2 + 3) * 5"));
            Assert.AreEqual(-25, EvaluateExpression("(2 + 3) * -5"));
            Assert.AreEqual(22.0, EvaluateExpression("(2 + 3) * (-5 + 14) / 2"));
            Assert.AreEqual(22.5, EvaluateExpression("(2.0 + 3) * (-5 + 14) / 2"));
            Assert.AreEqual(20.0, EvaluateExpression("(2 + 3) * ((-5 + 14) / 2)"));
            Assert.AreEqual(20.0, EvaluateExpression("(2.0 + 3) * ((-5 + 14) / 2)"));

            Assert.AreEqual(-123, EvaluateExpression("-123"));
            Assert.AreEqual(123, EvaluateExpression("-(-(123))"));

            // Excess whitespace
            Assert.AreEqual(5, EvaluateExpression("     5    "));
            Assert.AreEqual(12345.6, EvaluateExpression("     12345.6    "));
            Assert.AreEqual(5, EvaluateExpression("     2     +     3     "));
            Assert.AreEqual(5, EvaluateExpression(" \t 2 \t + \t 3 \t "));
        }

        [TestMethod]
        public void StringExpressions()
        {
            Assert.AreEqual(123, EvaluateExpression("123"));
            Assert.AreEqual(123.0, EvaluateExpression("123"));
            Assert.AreEqual("123", EvaluateExpression("123"));
            Assert.AreEqual(123, EvaluateExpression("'123'"));
            Assert.AreEqual(123.0, EvaluateExpression("'123'"));
            Assert.AreEqual("123", EvaluateExpression("'123'"));
            Assert.AreEqual(123, EvaluateExpression("123.0"));
            Assert.AreEqual(123.0, EvaluateExpression("123.0"));
            Assert.AreEqual("123", EvaluateExpression("123.0"));
            Assert.AreEqual(123, EvaluateExpression("'123.0'"));
            Assert.AreEqual(123.0, EvaluateExpression("'123.0'"));
            Assert.AreEqual("123.0", EvaluateExpression("'123.0'"));

            Assert.AreEqual(VarType.Integer, EvaluateExpression("\"2\" + \"2\"").Type);
            Assert.AreEqual(VarType.Float, EvaluateExpression("\"2.5\" + \"2.6\"").Type);
            Assert.AreEqual(VarType.String, EvaluateExpression("\"2\" & \"2\"").Type);
            Assert.AreEqual(VarType.String, EvaluateExpression("2.5 & 2.6").Type);

            Assert.AreEqual(4, EvaluateExpression("\"2\" + \"2\""));
            Assert.AreEqual(5.1, EvaluateExpression("\"2.5\" + \"2.6\""));
            Assert.AreEqual("22", EvaluateExpression("\"2\" & \"2\""));
            Assert.AreEqual("2.52.6", EvaluateExpression("2.5 & 2.6"));

            Assert.IsTrue(EvaluateExpression("'abc' + 5") == 5);
            Assert.IsTrue(EvaluateExpression("'abc' - 5") == -5);
            Assert.IsTrue(EvaluateExpression("'abc' * 5") == 0);
            Assert.IsTrue(EvaluateExpression("'abc' / 5") == 0);
            Assert.IsTrue(EvaluateExpression("'abc' % 5") == 0);
            Assert.IsTrue(EvaluateExpression("-'abc'") == 0);
            Assert.IsTrue(EvaluateExpression("'abc' ^ 5") == 0);
            Assert.IsTrue(EvaluateExpression("'abc' & 5") == "abc5");

            Assert.IsTrue(EvaluateExpression("5 + 'abc'") == 5);
            Assert.IsTrue(EvaluateExpression("5 - 'abc'") == 5);
            Assert.IsTrue(EvaluateExpression("5 * 'abc'") == 0);
            Assert.IsTrue(EvaluateExpression("5 / 'abc'") == 0);
            Assert.IsTrue(EvaluateExpression("5 % 'abc'") == 0);
            Assert.IsTrue(EvaluateExpression("5 & 'abc'") == "5abc");
        }

        private static Variable EvaluateExpression(string expression)
        {
            string source = $@"main()
{{
    return {expression}
}}";

            Compiler compiler = new();
            Assert.IsTrue(compiler.CompileSource(source, out CompiledProgram? program));

            Runtime runtime = new(program!);
            return runtime.Execute();
        }
    }
}
