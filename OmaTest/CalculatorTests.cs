using Microsoft.VisualStudio.TestTools.UnitTesting;
using OmaConsole;
using System;

namespace OmaTest
{
    [TestClass]
    public class CalculatorTests
    {
        private Calculator calculator;

        [TestInitialize]
        public void TestInit()
        {
            calculator = new Calculator
            {
                NumberDecimalSeparator = ".",  // Unittests use fixed locale
                MaxAfterDecimalNumerics = 8,   // All our test cases should be short...
            };
        }

        [DataTestMethod]
        [DataRow("0", "0", "0")]
        [DataRow("1", "0", "1")]
        [DataRow("9", "1", "10")]
        // add some big (beyond int) numbers
        [DataRow("99999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999",
                 "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901",
                "112345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678900")]
        // "add" some negative numbers
        [DataRow("-1", "1", "0")]
        [DataRow("-1", "-1", "-2")]
        [DataRow("10", "-1", "9")]
        public void TestAdd(string summandA, string summandB, string expected)
        {
            var result1 = calculator.Add(summandA, summandB);
            var result2 = calculator.Add(summandB, summandA);  // Add() must be commutative, ie. same result when inputs are swapped

            Assert.AreEqual(expected, result1);
            Assert.AreEqual(expected, result2);
        }

        // 99999.... + 1 = 100000....
        [TestMethod]
        public void TestAdd_VeryBigNumbers()
        {
            var summandA = new string('9', 1000000);
            var summandB = "1";
            var expected = "1" + new string('0', 1000000);

            var result1 = calculator.Add(summandA, summandB);
            var result2 = calculator.Add(summandB, summandA);  // Add() must be commutative, ie. same result when inputs are swapped

            Assert.AreEqual(expected, result1);
            Assert.AreEqual(expected, result2);
        }

        [DataTestMethod]
        [DataRow("0", "0", "0")]
        [DataRow("1", "0", "1")]
        [DataRow("0", "1", "-1")]
        [DataRow("1", "1", "0")]
        [DataRow("1000", "100", "900")]
        [DataRow("100", "1000", "-900")]
        // subtract some big (beyond int) numbers
        [DataRow("112345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678900",
                  "99999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999",
                  "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901")]
        [DataRow("112345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678900",
                  "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901",
                  "99999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999")]
        // "subtract" some negative numbers
        [DataRow("-1", "1", "-2")]
        [DataRow("-1", "-1", "0")]
        [DataRow("10", "-1", "11")]
        [DataRow("-10", "-1", "-12")]
        public void TestSub(string minuend, string subtrahend, string expected)
        {
            var result = calculator.Sub(minuend, subtrahend);

            Assert.AreEqual(expected, result);
        }

        // 100000.... - 1 = 99999....
        [TestMethod]
        public void TestSub_VeryBigNumbers()
        {
            var minuend = "1" + new string('0', 1000000);
            var subtrahend = "1";
            var expected = new string('9', 1000000);

            var result = calculator.Sub(minuend, subtrahend);

            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow("0", "0", "0")]
        [DataRow("1000", "1", "1000")]
        [DataRow("-1000", "1", "-1000")]
        [DataRow("-1000", "-1", "1000")]
        [DataRow("2000", "2", "4000")]
        [DataRow("2000", "-2", "-4000")]
        [DataRow("-2000", "-2", "4000")]
        [DataRow("123456789012345678901234567890123456789012345678901234567890",
                 "100000000000000000000000000000000000000000000000000000000000",
                 "12345678901234567890123456789012345678901234567890123456789000000000000000000000000000000000000000000000000000000000000")]
        public void TestMultiply(string factorA, string factorB, string expected)
        {
            var result1 = calculator.Multiply(factorA, factorB);
            var result2 = calculator.Multiply(factorB, factorA);  // Multiply() must be commutative, ie. same result when inputs are swapped

            Assert.AreEqual(expected, result1);
            Assert.AreEqual(expected, result2);
        }

        // 100000.... * 2 = 200000....
        [TestMethod]
        public void TestMultiply_VeryBigNumbers()
        {
            var factorA = "1" + new string('0', 1000000);
            var factorB = "2";
            var expected = "2" + new string('0', 1000000);

            var result1 = calculator.Multiply(factorA, factorB);
            var result2 = calculator.Multiply(factorB, factorA);  // Multiply() must be commutative, ie. same result when inputs are swapped

            Assert.AreEqual(expected, result1);
            Assert.AreEqual(expected, result2);
        }

        [DataTestMethod]
        [DataRow("0", "1", "0")] // Pow() of zero remains zero
        [DataRow("0", "0", "0")]
        [DataRow("0", "10", "0")]
        [DataRow("0", "-1", "0")]
        [DataRow("10", "0", "1")]
        [DataRow("10", "1", "10")]
        [DataRow("10", "2", "100")]
        [DataRow("10", "3", "1000")]
        [DataRow("3", "0", "1")]
        [DataRow("3", "1", "3")]
        [DataRow("3", "2", "9")]
        [DataRow("3", "3", "27")]
        [DataRow("-3", "0", "1")]
        [DataRow("-3", "1", "-3")]
        [DataRow("-3", "2", "9")]
        [DataRow("-3", "3", "-27")]
        // negative exponents can lead to periodic (unending) results... Set to 8 digits 
        [DataRow("3", "-1", "0.33333333")]
        [DataRow("3", "-2", "0.11111111")]
        [DataRow("3", "-3", "0.03703704")]  // rounded result?
        [DataRow("-3", "-1", "-0.33333333")]
        [DataRow("-3", "-2", "0.11111111")]
        [DataRow("-3", "-3", "-0.03703704")]
        [DataRow("4", "-1", "0.25")]
        [DataRow("4", "-2", "0.0625")]
        [DataRow("4", "-3", "0.015625")]
        [DataRow("-4", "-1", "-0.25")]
        [DataRow("-4", "-2", "0.0625")]
        [DataRow("-4", "-3", "-0.015625")]
        public void TestPow(string baseValue, string exponent, string expected)
        {
            var result = calculator.Pow(baseValue, exponent);

            Assert.AreEqual(expected, result);
        }

        // Pow(10^1000, 400) = 10^400000
        [TestMethod]
        public void TestPow_VeryBigNumbers()
        {
            var baseValue = "1" + new string('0', 1000);
            var exponent = "400";
            var expected = "1" + new string('0', 400000);

            var result = calculator.Pow(baseValue, exponent);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestSqr_NegativeValues_ShouldThrow()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => calculator.Sqr("-1"));
        }

        [DataTestMethod]
        [DataRow("0", "0")]
        [DataRow("1", "1")]
        // most baseValues have a periodic (unending) square root... Set to 8 digits
        [DataRow("2", "1.41421356")]
        [DataRow("3", "1.73205081")]
        [DataRow("4", "2")]
        [DataRow("9", "3")]
        [DataRow("16", "4")]
        public void TestSqr(string baseValue, string expected)
        {
            var result = calculator.Sqr(baseValue);

            Assert.AreEqual(expected, result);
        }

        // Srq(10^100000) = 10^50000
        [TestMethod]
        public void TestSqr_VeryBigNumbers()
        {
            var baseValue = "1" + new string('0', 100000);
            var expected = "1" + new string('0', 50000);

            var result = calculator.Sqr(baseValue);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMod_DivideByNull_ShouldThrow()
        {
            Assert.ThrowsException<DivideByZeroException>(() => calculator.Mod("0"/*irrelevant*/, "0"));
        }

        [DataTestMethod]
        [DataRow("0", "1", "0")]
        [DataRow("10", "1", "0")]
        [DataRow("10", "2", "0")]
        [DataRow("10", "3", "1")]
        [DataRow("10", "6", "4")]
        [DataRow("10", "10", "0")]
        [DataRow("10", "11", "10")]

        [DataRow("-10", "1", "0")]
        [DataRow("-10", "2", "0")]
        [DataRow("-10", "3", "2")]
        [DataRow("-10", "6", "2")]
        [DataRow("-10", "10", "0")]
        [DataRow("-10", "11", "1")]

        [DataRow("10", "-1", "0")]
        [DataRow("10", "-2", "0")]
        [DataRow("10", "-3", "-2")]
        [DataRow("10", "-6", "-2")]
        [DataRow("10", "-10", "0")]
        [DataRow("10", "-11", "-1")]

        [DataRow("-10", "-1", "0")]
        [DataRow("-10", "-2", "0")]
        [DataRow("-10", "-3", "-1")]
        [DataRow("-10", "-6", "-4")]
        [DataRow("-10", "-10", "0")]
        [DataRow("-10", "-11", "-10")]

        [DataRow("1234567890000000000000000000000000000001", "1234567890", "0")]
        public void TestMod(string dividend, string divisor, string expected)
        {
            var result = calculator.Mod(dividend, divisor);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDiv_DivideByNull_ShouldThrow()
        {
            Assert.ThrowsException<DivideByZeroException>(() => calculator.Div("0"/*irrelevant*/, "0"));
        }

        [DataTestMethod]
        [DataRow("0", "1", "0")]
        [DataRow("10", "1", "10")]
        [DataRow("10", "2", "5")]
        [DataRow("10", "3", "3.33333333")]
        [DataRow("10", "6", "1.66666666")]
        [DataRow("10", "10", "1")]
        [DataRow("10", "20", "0.5")]

        [DataRow("-10", "1", "-10")]
        [DataRow("-10", "2", "-5")]
        [DataRow("-10", "3", "-3.33333333")]
        [DataRow("-10", "6", "-1.66666666")]
        [DataRow("-10", "10", "-1")]
        [DataRow("-10", "20", "-0.5")]

        [DataRow("10", "-1", "-10")]
        [DataRow("10", "-2", "-5")]
        [DataRow("10", "-3", "-3.33333333")]
        [DataRow("10", "-6", "-1.66666666")]
        [DataRow("10", "-10", "-1")]
        [DataRow("10", "-20", "-0.5")]

        [DataRow("-10", "-1", "10")]
        [DataRow("-10", "-2", "5")]
        [DataRow("-10", "-3", "3.33333333")]
        [DataRow("-10", "-6", "1.66666666")]
        [DataRow("-10", "-10", "1")]
        [DataRow("-10", "-20", "0.5")]

        [DataRow("1234567890000000000000000000000000000000", "1234567890", "1000000000000000000000000000000")]
        public void TestDiv(string dividend, string divisor, string expected)
        {
            var result = calculator.Div(dividend, divisor);

            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow("0", "0", true)]
        [DataRow("1", "0", true)]
        [DataRow("0", "1", false)]
        [DataRow("2", "1", true)]
        [DataRow("0", "-1", true)]
        [DataRow("-2", "-1", false)]
        [DataRow("-2", "-2", true)]
        [DataRow("-2", "3", false)]
        [DataRow("2", "-3", true)]
        [DataRow("9999999999999999999999999999999999999999999999999999999999999999998",
                 "9999999999999999999999999999999999999999999999999999999999999999999",
                 false)]
        [DataRow("9999999999999999999999999999999999999999999999999999999999999999999",
                 "9999999999999999999999999999999999999999999999999999999999999999999",
                 true)]
        [DataRow("10000000000000000000000000000000000000000000000000000000000000000000",
                  "9999999999999999999999999999999999999999999999999999999999999999999",
                  true)]
        public void TestGreaterThanOrEqual(string left, string right, bool expected)
        {
            var result = calculator.GreaterThanOrEqual(left, right);

            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow("0", "0", false)]
        [DataRow("1", "0", true)]
        [DataRow("0", "1", false)]
        [DataRow("2", "1", true)]
        [DataRow("0", "-1", true)]
        [DataRow("-2", "-1", false)]
        [DataRow("-2", "-2", false)]
        [DataRow("-2", "3", false)]
        [DataRow("2", "-3", true)]
        [DataRow("9999999999999999999999999999999999999999999999999999999999999999998",
                 "9999999999999999999999999999999999999999999999999999999999999999999",
                 false)]
        [DataRow("9999999999999999999999999999999999999999999999999999999999999999999",
                 "9999999999999999999999999999999999999999999999999999999999999999999",
                 false)]
        [DataRow("10000000000000000000000000000000000000000000000000000000000000000000",
                  "9999999999999999999999999999999999999999999999999999999999999999999",
                  true)]
        public void TestGreaterThan(string left, string right, bool expected)
        {
            var result = calculator.GreaterThan(left, right);

            Assert.AreEqual(expected, result);
        }
    }
}