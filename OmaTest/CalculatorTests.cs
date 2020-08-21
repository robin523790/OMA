using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Serialization;
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
            calculator = new Calculator();
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
            var summandA = new string('9', 10000000);
            var summandB = "1";
            var expected = "1" + new string('0', 10000000);

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
        public void TestSub(string minuend, string subtrahend, string expected)
        {
            var result = calculator.Sub(minuend, subtrahend);

            Assert.AreEqual(expected, result);
        }

        // 100000.... - 1 = 99999....
        [TestMethod]
        public void TestSub_VeryBigNumbers()
        {
            var minuend = "1" + new string('0', 10000000);
            var subtrahend = "1";
            var expected = new string('9', 10000000);

            var result = calculator.Sub(minuend, subtrahend);

            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow("0", "0", "0")]
        [DataRow("1000", "1", "1000")]
        [DataRow("2000", "2", "4000")]
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

        // 100000.... * 1234 = 123400000....
        [TestMethod]
        public void TestMultiply_VeryBigNumbers()
        {
            var factorA = "1" + new string('0', 1000);
            var factorB = "12345678901234567890123456789012345678901234567890";
            var expected = "12345678901234567890123456789012345678901234567890" + new string('0', 1000);

            var result1 = calculator.Multiply(factorA, factorB);
            var result2 = calculator.Multiply(factorB, factorA);  // Multiply() must be commutative, ie. same result when inputs are swapped

            Assert.AreEqual(expected, result1);
            Assert.AreEqual(expected, result2);
        }

        [DataTestMethod]
        [DataRow("0", "1", "0")] // Pow() of zero remains zero
        [DataRow("0", "0", "1")]
        [DataRow("0", "10", "0")]
        [DataRow("10", "0", "1")]
        [DataRow("10", "1", "10")]
        [DataRow("10", "2", "100")]
        [DataRow("10", "3", "1000")]
        [DataRow("3", "0", "1")]
        [DataRow("3", "1", "3")]
        [DataRow("3", "2", "9")]
        [DataRow("3", "3", "27")]
        public void TestPow(string baseValue, string exponent, string expected)
        {
            var result = calculator.Pow(baseValue, exponent);

            Assert.AreEqual(expected, result);
        }

        // Pow(10^1000, 400) = 10^400000
        [TestMethod]
        public void TestPow_VeryBigNumbers()
        {
            var baseValue = "2" + new string('0', 50);
            var exponent = "40";
            // is the same as 2^40 * (10^50)^40
            var expected = "1099511627776" + new string('0', 2000);

            var result = calculator.Pow(baseValue, exponent);

            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow("0", "0")] // Pow() of zero remains zero
        [DataRow("1", "1")]
        [DataRow("2", "4")]
        [DataRow("3", "9")]
        [DataRow("4", "16")]
        [DataRow("5", "25")]
        [DataRow("6", "36")]
        [DataRow("7", "49")]
        [DataRow("8", "64")]
        [DataRow("9", "81")]
        [DataRow("9", "81")]
        [DataRow("10", "100")]
        [DataRow("11", "121")]
        [DataRow("12", "144")]
        [DataRow("100", "10000")]
        [DataRow("10000", "100000000")]
        [DataRow("100000000", "10000000000000000")]
        [DataRow("1000000000000", "1000000000000000000000000")]
        [DataRow("10000000000000000", "100000000000000000000000000000000")]
        [DataRow("100000000000000000000", "10000000000000000000000000000000000000000")]
        [DataRow("10000000000000000000000000000", "100000000000000000000000000000000000000000000000000000000")]
        public void TestSqr(string baseValue, string expected)
        {
            var result = calculator.Sqr(baseValue);

            Assert.AreEqual(expected, result);
        }

        // Sqr(10^500) = 10^(500*2) = 10^1000
        [TestMethod]
        public void TestSqr_VeryBigNumbers()
        {
            var baseValue = "1" + new string('0', 500);
            var expected = "1" + new string('0', 1000);

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
        [DataRow("1234567890000000000000000000000000000000", "1234567890", "0")]
        [DataRow("1234567890000000000000000000000000000001", "1234567890", "1")]
        public void TestMod(string dividend, string divisor, string expected)
        {
            var result = calculator.Mod(dividend, divisor);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestMod_VeryBigNumbers()
        {
            var dividend = "12345678901234567890123456789012345678901234567890" + new string('0', 100000);
            var divisor1 = "12345678901234567890123456789012345678901234567890";
            var divisor2 = "12345678901234567890123456789012345678901234567891";
            var expected1 = "0";
            var expected2 = "2272325713533227610035641853109142442186208450773";

            var result1 = calculator.Mod(dividend, divisor1);
            var result2 = calculator.Mod(dividend, divisor2);

            Assert.AreEqual(expected1, result1);
            Assert.AreEqual(expected2, result2);
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
        [DataRow("10", "3", "3")]
        [DataRow("10", "6", "1")]
        [DataRow("10", "10", "1")]
        [DataRow("10", "20", "0")]
        [DataRow("1234567890000000000000000000000000000000", "1234567890", "1000000000000000000000000000000")]
        public void TestDiv(string dividend, string divisor, string expected)
        {
            var result = calculator.Div(dividend, divisor);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestDiv_VeryBigNumbers()
        {
            var dividend = "12345678901234567890123456789012345678901234567890" + new string('0', 1000000);
            var divisor = "12345678901234567890123456789012345678901234567890";
            var expected = "1" + new string('0', 1000000);

            var result = calculator.Div(dividend, divisor);

            Assert.AreEqual(expected, result);
        }

        [DataTestMethod]
        [DataRow("0", "0", true)]
        [DataRow("1", "0", true)]
        [DataRow("0", "1", false)]
        [DataRow("2", "1", true)]
        [DataRow("89999999999999999999999999999999999999999999999999999999999999999990",
                  "9999999999999999999999999999999999999999999999999999999999999999999",
                 true)]
        [DataRow( "8999999999999999999999999999999999999999999999999999999999999999999",
                 "99999999999999999999999999999999999999999999999999999999999999999990",
                 false)]
        [DataRow("8999999999999999999999999999999999999999999999999999999999999999999",
                 "9999999999999999999999999999999999999999999999999999999999999999999",
                 false)]
        [DataRow("9999999999999999999999999999999999999999999999999999999999999999999",
                 "8999999999999999999999999999999999999999999999999999999999999999999",
                 true)]
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
        [DataRow("89999999999999999999999999999999999999999999999999999999999999999990",
                  "9999999999999999999999999999999999999999999999999999999999999999999",
                 true)]
        [DataRow( "8999999999999999999999999999999999999999999999999999999999999999999",
                 "99999999999999999999999999999999999999999999999999999999999999999990",
                 false)]
        [DataRow("8999999999999999999999999999999999999999999999999999999999999999999",
                 "9999999999999999999999999999999999999999999999999999999999999999999",
                 false)]
        [DataRow("9999999999999999999999999999999999999999999999999999999999999999999",
                 "8999999999999999999999999999999999999999999999999999999999999999999",
                 true)]
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
