using System;
using System.Threading;

namespace OmaConsole
{
    public class Calculator : ICalculator
    {
        /// <summary>
        /// This string is used as the decimal separator in numeric results.
        /// </summary>
        public string NumberDecimalSeparator { get; set; } = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

        /// <summary>
        /// Maximum number of numerics that can appear after the decimal point.
        /// After this (e.g. in case of periodic or irrational numbers) the result will be rounded mathematically.
        /// </summary>
        public int MaxAfterDecimalNumerics { get; set; } = 100000;

        public string Add(string summandA, string summandB)
        {
            throw new NotImplementedException();
        }

        public string Sub(string minuend, string subtrahend)
        {
            throw new NotImplementedException();
        }
        public string Multiply(string factorA, string factorB)
        {
            throw new NotImplementedException();
        }

        public string Pow(string baseValue, string exponent)
        {
            throw new NotImplementedException();
        }

        public string Sqr(string baseValue)
        {
            throw new NotImplementedException();
        }

        public string Mod(string dividend, string divisor)
        {
            throw new NotImplementedException();
        }

        public string Div(string dividend, string divisor)
        {
            throw new NotImplementedException();
        }

        public bool GreaterThanOrEqual(string left, string right)
        {
            throw new NotImplementedException();
        }

        public bool GreaterThan(string left, string right)
        {
            throw new NotImplementedException();
        }
    }
}
