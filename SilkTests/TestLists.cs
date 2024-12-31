// Copyright (c) 2019-2024 Jonathan Wood (www.softcircuits.com)
// Licensed under the MIT license.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SoftCircuits.Silk;
using System.Collections.Generic;
using System.Linq;

namespace SilkTests
{
    [TestClass]
    public class TestLists
    {
        [TestMethod]
        public void TestList()
        {
            List<Variable> list = new()
            {
                new Variable(1),
                new Variable(2),
                new Variable(3),
                new Variable(4),
                new Variable(5)
            };

            // Populate list variable
            Variable var = new(list);
            Assert.AreEqual(5, var.ListCount);

            // Compare lists
            Variable var2 = new(list.Select(x => new Variable(x)));
            Assert.AreEqual(0, var.CompareTo(var2));
            Assert.IsTrue(var.Equals(var2));

            var.GetAt(4).SetValue(100);
            Assert.IsTrue(var.CompareTo(var2) > 0);
            Assert.IsFalse(var.Equals(var2));

            var.GetAt(4).SetValue(-100);
            Assert.IsTrue(var.CompareTo(var2) < 0);
            Assert.IsFalse(var.Equals(var2));

            // Comparisons
            Assert.IsTrue(var.CompareTo("abc") < 0);
            Assert.IsTrue(var.CompareTo(string.Empty) > 0);
            Assert.AreEqual(0, var.CompareTo("1"));
            Assert.AreEqual(0, var.CompareTo(1));
            Assert.AreEqual(0, var.CompareTo(1.0));

            Assert.IsTrue(var.IsTrue());
            Assert.IsFalse(var.IsFalse());

            // Remove a list item
            List<Variable> list2 = new(var.GetList());
            list2.RemoveAt(2);
            var.SetValue(list2);
            Assert.AreEqual(4, var.ListCount);
            Assert.IsFalse(var.Equals(list2));  // list2 is not Variable

            // Set to empty list
            var.SetValue(Enumerable.Empty<Variable>());
            Assert.AreEqual(0, var.ListCount);
            Assert.IsTrue(var.CompareTo(var2) < 0);
            Assert.IsFalse(var.Equals(var2));
        }

        [TestMethod]
        public void TestEmptyList()
        {
            Variable var = new(Enumerable.Empty<Variable>());
            Variable empty = new();

            Assert.AreEqual(0, var.CompareTo(empty));
            Assert.IsTrue(var.CompareTo("abc") < 0);
            Assert.IsTrue(var.CompareTo(string.Empty) > 0);
            Assert.AreEqual(0, var.CompareTo("0"));
            Assert.AreEqual(0, var.CompareTo(0));
            Assert.AreEqual(0, var.CompareTo(0.0));

            Assert.IsFalse(var.IsTrue());
            Assert.IsTrue(var.IsFalse());

            // Empty list less than list with one item
            Assert.IsTrue(var.CompareTo(new Variable(new[] { new Variable("0") })) < 0);
            Assert.IsTrue(var.CompareTo(new Variable(new[] { new Variable(0) })) < 0);
            Assert.IsTrue(var.CompareTo(new Variable(new[] { new Variable(0.0) })) < 0);

            // Empty list equal to zero
            Assert.AreEqual(0, var.CompareTo(new Variable("0")));
            Assert.AreEqual(0, var.CompareTo(new Variable(0)));
            Assert.AreEqual(0, var.CompareTo(new Variable(0.0)));
        }

        [TestMethod]
        public void TestNonList()
        {
            Variable var = new(123);

            IEnumerable<Variable> list = var.GetList();
            Assert.AreEqual(1, list.Count());
            Assert.AreEqual(123, list.First());

            Assert.AreEqual(0, var.CompareTo(123));
            Assert.IsFalse(var.IsList);
            Assert.AreEqual(0, var.GetAt(200).CompareTo(0));
        }

        [TestMethod]
        public void TestIndexes()
        {
            string script = @"var array = {
    { {  1,  2,  3 }, {  4,  5,  6 }, {  7,  8,  9 } },
    { { 10, 11, 12 }, { 13, 14, 15 }, { 16, 17, 18 } },
    { { 19, 20, 21 }, { 22, 23, 24 }, { 25, 26, 27 } }
}

main()
{
    total = 0
    for i = 1 to 3
        for j = 1 to 3
            for k = 1 to 3
                total = total + array[i][j][k]

    return total
}";

            Assert.AreEqual(378, TestHelper.RunScript(script));
        }

        [TestMethod]
        public void TestIndexes2()
        {
            string script = @"var array = {
    { {  1,  2,  3 }, {  4,  5,  6 }, {  7,  8,  9 } },
    { { 10, 11, 12 }, { 13, 14, 15 }, { 16, 17, 18 } },
    { { 19, 20, 21 }, { 22, 23, 24 }, { 25, 26, 27 } }
}

main()
{
    array[1][1][1] = 100
    array[1 + 1][1 + 1][1 + 1] = 100

    total = 0

    for i = 1 to 3
        for j = 1 to 3
            for k = 1 to 3
                total = total + array[i][j][k]

    return total
}";

            Assert.AreEqual(563, TestHelper.RunScript(script));
        }

        [TestMethod]
        public void TestIndexes3()
        {
            string script = @"var array = {
    { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } },
    { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } },
    { { 0, 0, 0 }, { 0, 0, 0 }, { 0, 0, 0 } }
}

main()
{
    counter = 1

    for i = 1 to 3
        for j = 1 to 3
            for k = 1 to 3
            {
                array[i][j][k] = counter
                counter = counter + 1
            }

    total = 0
    for i = 1 to 3
        for j = 1 to 3
            for k = 1 to 3
                total = total + array[i][j][k]

    return total
}";

            Assert.AreEqual(378, TestHelper.RunScript(script));
        }
    }
}
