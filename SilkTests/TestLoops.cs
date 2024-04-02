// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.Silk;

namespace SilkTests
{
    [TestClass]
    public class TestLoops
    {
        public static readonly ProgramTest[] Tests = new ProgramTest[]
        {
            new ProgramTest
            {
                Description = "For/Break",
                Result = new(10),
                Source = @"main()
{
    for i = 1 to 100
    {
        if (i = 11)
            break
        counter = counter + 1
    }
    return counter
}"
            },
            new ProgramTest
            {
                Description = "For/Continue",
                Result = new Variable(97),
                Source = @"main()
{
    for i = 1 to 100
    {
        if (i = 10 or i = 20 or i = 30)
            continue
        counter = counter + 1
    }
    return counter
}"
            },
            new ProgramTest
            {
                Description = "For/Continue 2",
                Result = new Variable(0),
                Source = @"main()
{
    for i = 1 to 100
    {
        continue
        counter = counter + 1
    }
    return counter
}"
            },
            new ProgramTest
            {
                Description = "While/Break",
                Result = new Variable(10),
                Source = @"main()
{
    i = 1
    while i <= 100
    {
        if (i = 11)
            break
        i = i + 1
        counter = counter + 1
    }
    return counter
}"
            },
            new ProgramTest
            {
                Description = "While/Continue",
                Result = new Variable(97),
                Source = @"main()
{
    i = 1
    while i <= 100
    {
        i = i + 1
        if (i = 10 or i = 20 or i = 30)
            continue
        counter = counter + 1
    }
    return counter
}"
            },
            new ProgramTest
            {
                Description = "While/Continue 2",
                Result = new Variable(0),
                Source = @"main()
{
    i = 0
    while i <= 100
    {
        i = i + 1
        continue
        counter = counter + 1
    }
    return counter
}"
            },
            new ProgramTest
            {
                Description = "For/Nested Break",
                Result = new Variable(50),
                Source = @"main()
{
    for i = 1 to 10
    {
        for j = 1 to 10
        {
            if j > 5
                break
            counter = counter + 1
        }
    }
    return counter
}"
            },
            new ProgramTest
            {
                Description = "For/Nested Continue",
                Result = new Variable(90),
                Source = @"main()
{
    for i = 1 to 10
    {
        for j = 1 to 10
        {
            if j = 5
                continue
            counter = counter + 1
        }
    }
    return counter
}"
            },
            new ProgramTest
            {
                Description = "While/Nested Break",
                Result = new Variable(50),
                Source = @"main()
{
    i = 1
    while i <= 10
    {
        i = i + 1
        j = 1
        while j <= 10
        {
            if j > 5
                break
            j = j + 1
            counter = counter + 1
        }
    }
    return counter
}"
            },

            new ProgramTest
            {
                Description = "While/Nested Continue",
                Result = new Variable(90),
                Source = @"main()
{
    i = 1
    while i <= 10
    {
        i = i + 1
        j = 1
        while j <= 10
        {
            j = j + 1
            if j = 5
                continue
            counter = counter + 1
        }
    }
    return counter
}"
            },
        };

        [TestMethod]
        public void RunTests()
        {
            foreach (ProgramTest test in Tests)
                Assert.AreEqual(test.Result, TestHelper.RunScript(test.Source));
        }
    }
}
