// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.Silk;

namespace SilkTests
{
    [TestClass]
    public class TestPrograms
    {
        private readonly ProgramTest[] Tests = new ProgramTest[]
        {
            new()
            {
                Result = new Variable(5),
                Source = @"main()
{
    a = 5
    a = a + 5
    a = a - 5
    a = a * 2
    a = a / 2
    return a
}"
            },
            new()
            {
                Result = new Variable(5),
                Source = @"main()
{
    a = 5
    a = SubtractFive(AddFive(a))
    a = Halve(Double(a))
    return a
}

AddFive(a)
{
    return a + 5
}

SubtractFive(a)
{
    return a - 5
}

Double(a)
{
    return a * 2
}

Halve(a)
{
    return a / 2
}"
            },
            new()
            {
                Result = new Variable(21924),
                Source = @"main()
{
    return (2 + 7) * (2 * (8 + double(5) * (100 + triple(7))))
}

double(a)
{
    return a * 2
}

triple(a)
{
    return a * 3
}"
            },
            new()
            {
                Result = new Variable(141),
                Source = @"var array = { 20, 30, ""abc"", 40, ""def"", 50, [5], { 1, 2, 3 } }

main()
{
    for i = 1 to len(array)
        a = a + array[i]
    return a
}"
            },
            new()
            {
                Result = new Variable("12345"),
                Source = @"var array = { 2, ""3"", ""4"", 5 }

main()
{
    a = 1
    for i = 1 to len(array)
        a = a & array[i]
    return a
}"
            }
        };

        [TestMethod]
        public void Test()
        {
            foreach (var test in Tests)
                Assert.AreEqual(test.Result, TestHelper.RunScript(test.Source));
        }
    }
}
