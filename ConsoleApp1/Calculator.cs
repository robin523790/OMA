using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
        /// After this (e.g. in case of periodic or irrational numbers) the result will be cut off (no rounding)
        /// </summary>
        public int MaxAfterDecimalNumerics { get; set; } = 100000;

        public string Add(string summandA, string summandB)
        {
            var negativeA = summandA.StartsWith('-');
            var negativeB = summandB.StartsWith('-');

            // if only one of the addends is negative, subtract it from the positive
            if (negativeA && !negativeB)
            {
                return Sub(summandB, summandA.Substring(1));
            }
            else if (!negativeA && negativeB)
            {
                return Sub(summandA, summandB.Substring(1));
            }

            // if both addends are negative, let's get rid of the '-'...
            if (negativeA && negativeB)
            {
                // For example: -5 + -10 = -1 * (10 + 5)
                return "-" + Add(summandA.Substring(1), summandB.Substring(1));
            }


            var len = Math.Max(summandA.Length, summandB.Length);
            char[] result = new char[len + 1];

            int carry = 0; // Carry over from one digit to the next...
            for (int i = 0; i < len; i++)
            {
                // Get the last digits of summandA and summandB
                // - by now the code ensures that both are positive
                int valA = i < summandA.Length ? summandA[summandA.Length - 1 - i] - '0' : 0;
                int valB = i < summandB.Length ? summandB[summandB.Length - 1 - i] - '0' : 0;
                int res = valA + valB;
                res += carry;     // Add old carry-over value
                carry = 0;
                while (res > 9)
                {
                    carry += 1;   // Carry over the 10s to the next digit
                    res -= 10;    // Ignore the 10s here
                }
                res += '0';
                result[len - i] = (char)res;
            }

            // insert last carry-over value, if any
            result[0] = (char)(carry + '0');
            int startpos = carry != 0 ? 0 : 1;
            return new string(result, startpos, len + 1 - startpos);
        }

        public string Sub(string minuend, string subtrahend)
        {
            // a little performance boost when it's obvious...
            if (minuend == subtrahend)
                return "0";

            var negativeA = minuend.StartsWith('-');
            var negativeB = subtrahend.StartsWith('-');

            // if the subtrahend OR the minuend is negative, this is actually an Add()! 
            if (negativeB)
            {
                // For example: 10 - -5 = 10 + 5
                //  - If the minuend is negative, this turns into a Sub() again (e.g. -10 - -5 = -10 + 5 = 5 - 10)
                //  - This is handled inside Add()
                return Add(minuend, subtrahend.Substring(1));
            }
            else if (negativeA)
            {
                // For example:  -5 - 10 = -1 * (5 + 10)
                //               -10 - 5 = -1 * (10 + 5)
                return "-" + Add(minuend.Substring(1), subtrahend);
            }

            // if both minuend and subtrahend are positive, but the subtrahend is greater than the minuend, turn them around
            if (GreaterThan(subtrahend, minuend))
            {
                // For example: 5 - 10 = -1 * (10 - 5)
                return "-" + Sub(subtrahend, minuend);
            }


            var len = Math.Max(minuend.Length, subtrahend.Length);
            char[] result = new char[len];

            int carry = 0; // Carry over from one digit to the next...
            for (int i = 0; i < len; i++)
            {
                // Get the last digits of minuend and subtrahend
                // - by now the code ensures that both are positive
                int valA = i < minuend.Length ? minuend[minuend.Length - 1 - i] - '0' : 0;
                int valB = i < subtrahend.Length ? subtrahend[subtrahend.Length - 1 - i] - '0' : 0;
                int res = valA - valB;
                res -= carry;     // Add old carry-over value
                carry = 0;
                while (res < 0)
                {
                    res += 10;
                    carry = 1;    // Take 10 from the next digit
                }
                res += '0';
                result[len - 1 - i] = (char)res;
            }


            // remove any leading zeros, if any
            int startpos = 0;
            for (; result[startpos] == '0'; startpos++)
                ;
            return new string(result, startpos, len - startpos);
        }

        public string Multiply(string factorA, string factorB)
        {
            var negativeA = factorA.StartsWith('-');
            var negativeB = factorB.StartsWith('-');

            // if both factors are negative, think positive
            if (negativeA && negativeB)
            {
                return Multiply(factorA.Substring(1), factorB.Substring(1));
            }

            // if only one factor is negative, the result is always negative
            if (negativeA)
            {
                return "-" + Multiply(factorA.Substring(1), factorB);
            }
            else if (negativeB)
            {
                return "-" + Multiply(factorA, factorB.Substring(1));
            }


            string result = "0";

            object localLockObject = new object();
            Parallel.For(0, factorA.Length, a =>
            {
                string rowResult = "0";

                for (int b = factorB.Length - 1; b >= 0; b--)
                {
                    // Get the last digits of factorA and factorB
                    // - by now the code ensures that both are positive
                    int valA = factorA[a] - '0';
                    int valB = factorB[b] - '0';
                    int res = valA * valB;
                    string row = res.ToString();

                    // add a '0' for every digit left of the last
                    int zerosB = factorB.Length - 1 - b;
                    if (zerosB > 0)
                    {
                        row += new string('0', zerosB);
                    }

                    rowResult = Add(rowResult, row);
                }

                // add another '0' for every digit left of the last
                int zerosA = factorA.Length - 1 - a;
                if (zerosA > 0)
                {
                    rowResult += new string('0', zerosA);
                }

                lock (localLockObject)
                {
                    result = Add(rowResult, result);
                }
            });

            return result;
        }

        public string Pow(string baseValue, string exponent)
        {
            if (exponent == "0")
            {
                return "1";
            }
            else if (exponent == "1")
            {
                return baseValue;
            }

            if (baseValue == "0")
            {
                return "0";  // exponent == "0" ==> "1" takes precedence!
            }

            var negativeA = baseValue.StartsWith('-');
            var negativeB = exponent.StartsWith('-');

            // negative exponent: x ^ -y = 1 / x ^ y
            if (negativeB)
            {
                return Div("1", Pow(baseValue, exponent.Substring(1)));
            }

            // negative baseValue with even exponent ==> positive result
            // negative baseValue with odd exponent ==> negative result
            if (negativeA)
            {
                char lastCharExp = exponent[exponent.Length - 1];
                if (lastCharExp % 2 == 0)
                {
                    return Pow(baseValue.Substring(1), exponent);
                }
                else
                {
                    return "-" + Pow(baseValue.Substring(1), exponent);
                }
            }


            string result = baseValue;

            // Multiply baseValue as often as it takes...
            // x^y = x*x*x* ....y times
            while (exponent != "1")
            {
                result = Multiply(result, baseValue);
                exponent = Sub(exponent, "1");
            }

            return result;
        }

        public string Sqr(string baseValue)
        {
            return Multiply(baseValue, baseValue);
        }

        /// <summary>
        /// Finds the square root of radicant.
        /// </summary>
        /// <param name="radicant">The number to find the square root for.</param>
        /// <returns>Square root of radicant.</returns>
        public string Sqrt(string radicant)
        {
            if (radicant == "0" || radicant == "1")
            {
                return radicant;
            }

            var negative = radicant.StartsWith('-');

            // we're not implementing unreal numbers
            // alternative:  return "i * " + Sqrt(baseValue.Substring(1));  with i = Sqrt(-1) according to Euler
            if (negative)
                throw new ArgumentOutOfRangeException();


            var result = new StringBuilder(radicant.Length);
            var resultNoDecimalPoint = new StringBuilder(radicant.Length);

            // parse baseValue in groups of two, beginning at the back
            int len = radicant.Length % 2 == 0 ? 2 : 1;  // for odd lengths, start with just the first digit...
            string group = radicant.Substring(0, len);
            int afterDecimalNumerics = 0;

            // C# strings cannot have more than 2^32-1 characters, so using int for offset is fine
            int offset = -1;
            while (offset < radicant.Length || group != "0")
            {
                // special treatment of first group
                if (offset == -1)
                {
                    offset = len;
                }
                else if (offset < radicant.Length)
                {
                    group += radicant.Substring(offset, 2);
                    offset += 2;
                }
                else if (afterDecimalNumerics < MaxAfterDecimalNumerics)
                {
                    group += "00";
                    if (afterDecimalNumerics == 0)
                    {
                        if (result.Length == 0)
                            result.Append("0");
                        result.Append(NumberDecimalSeparator);
                    }
                    ++afterDecimalNumerics;
                }
                else
                {
                    break; // that's enough...
                }

                // remove any leading zeros, if any
                while (group.Length > 1 && group[0] == '0')
                {
                    group = group.Substring(1);
                }

                // subtract as many odd numbers as possible => count is the next digit
                string sub = "";
                if (resultNoDecimalPoint.Length > 0)
                    sub = Multiply(resultNoDecimalPoint.ToString(), "2");                
                sub += "1"; // "string + 1"  equals  "value * 10 + 1"
                int digit = 0;
                for (; GreaterThanOrEqual(group, sub); sub = Add(sub, "2"))
                {
                    group = Sub(group, sub);
                    ++digit;
                }
                result.Append(digit.ToString());
                resultNoDecimalPoint.Append(digit.ToString());
            }

            return result.ToString();
        }

        public string Mod(string dividend, string divisor)
        {
            int maxAfterDecimalNumericsSave = MaxAfterDecimalNumerics;
            try
            {
                // Do an integer division, multiply again with divisor. Voila!
                MaxAfterDecimalNumerics = 0;
                var result = DivideHelper(dividend, divisor);
                return result.Item2;
            }
            finally
            {
                // Restore settings
                MaxAfterDecimalNumerics = maxAfterDecimalNumericsSave;
            }
        }

        public string Div(string dividend, string divisor)
        {
            var result = DivideHelper(dividend, divisor);
            return result.Item1;
        }

        public bool GreaterThanOrEqual(string left, string right)
        {
            if (left == right)
                return true;

            return GreaterThan(left, right);
        }

        public bool GreaterThan(string left, string right)
        {
            if (left == right)
                return false;

            var negativeA = left.StartsWith('-');
            var negativeB = right.StartsWith('-');

            // for example: 10 > -5
            if (!negativeA && negativeB)
                return true;

            // for example: -10 < 5
            if (negativeA && !negativeB)
                return false;

            // for example:  10 > 5
            //               5  < 10
            //               13 > 12
            //               12 < 13
            if (!negativeA && !negativeB)
            {
                if (left.Length > right.Length)
                    return true;
                if (left.Length < right.Length)
                    return false;

                // Length equal? Find the first difference...
                for (int i = 0; i < left.Length; i++)
                {
                    if (left[i] != right[i])
                        return left[i] > right[i]; // bigger positive number is "GreaterThan"
                }
            }

            // for example:  -10 < -5
            //               -5  > -10
            //               -13 < -12
            //               -12 > -13
            if (negativeA && negativeB)
            {
                if (left.Length > right.Length)
                    return false;
                if (left.Length < right.Length)
                    return true;

                // Length equal? Find the first difference...
                for (int i = 0; i < left.Length; i++)
                {
                    if (left[i] != right[i])
                        return left[i] < right[i]; // smaller negative number is "GreaterThan"
                }
            }

            return false; // satisfy compiler, cannot reach this
        }

        /// <summary>
        /// Returns the integer value of the division of a dividend by a divisor, and the remaining part.
        /// </summary>
        /// <param name="dividend">The dividend.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns>A tuple of the integer value of the division (no rounding), and the remaining part.</returns>
        private Tuple<string, string> DivideHelper(string dividend, string divisor)
        {
            if (divisor == "0")
                throw new DivideByZeroException();

            if (divisor == "1")
                return new Tuple<string, string>(dividend, "0");

            var negativeA = dividend.StartsWith('-');
            var negativeB = divisor.StartsWith('-');

            // if both dividend and divisor are negative, think positive
            // - if either negativeA or negativeB => the division is always negative
            // - if negativeA => the modulo is always negative (regardless of negativeB)
            if (negativeA && negativeB)
            {
                var res = DivideHelper(dividend.Substring(1), divisor.Substring(1));
                var division = res.Item1;
                var remaining = res.Item2 == "0" ? "0" : "-" + res.Item2;
                return new Tuple<string, string>(division, remaining);
            }
            else if (negativeA)
            {
                var res = DivideHelper(dividend.Substring(1), divisor);
                var division = res.Item1 == "0" ? "0" : "-" + res.Item1;
                var remaining = res.Item2 == "0" ? "0" : "-" + res.Item2;
                return new Tuple<string, string>(division, remaining);
            }
            else if (negativeB)
            {
                var res = DivideHelper(dividend, divisor.Substring(1));
                var division = res.Item1 == "0" ? "0" : "-" + res.Item1;
                var remaining = res.Item2;
                return new Tuple<string, string>(division, remaining);
            }


            int afterDecimalNumerics = 0;
            var result = new StringBuilder(dividend.Length);

            // C# strings cannot have more than 2^32-1 characters, so using int for offset is fine
            int offset = 0;
            string part = "";
            while (offset < dividend.Length || part != "0")
            {
                part = part != "0" ? part : "";
                if (offset < dividend.Length)
                {
                    part += dividend.Substring(offset, 1);
                    ++offset;
                }
                else if (afterDecimalNumerics < MaxAfterDecimalNumerics)
                {
                    part += "0";
                    if (afterDecimalNumerics == 0)
                    {
                        if (result.Length == 0)
                            result.Append("0");
                        result.Append(NumberDecimalSeparator);
                    }
                    ++afterDecimalNumerics;
                }
                else
                {
                    break; // that's enough...
                }

                int lastDigit = 0;
                while (GreaterThanOrEqual(part, divisor))
                {
                    part = Sub(part, divisor);
                    ++lastDigit; // how many times does divisor fit into part?
                }

                if (lastDigit != 0 || result.Length != 0)  // skip leading zeros...
                    result.Append(lastDigit.ToString());
            }

            var div = result.ToString();
            if (div.Length == 0)
                div = "0";
            return new Tuple<string, string>(div, part);
        }
    }
}
