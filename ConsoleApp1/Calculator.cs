using System;
using System.Text;
using System.Threading.Tasks;

namespace OmaConsole
{
    public class Calculator : ICalculator
    {
        public string Add(string summandA, string summandB)
        {
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
            object localLockObject = new object();
            string result = "0";

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

        public string Mod(string dividend, string divisor)
        {
            var result = DivideHelper(dividend, divisor);
            return result.Item2;
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

            // for example:  10 > 5
            //               5  < 10
            //               13 > 12
            //               12 < 13
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

            if (!GreaterThanOrEqual(dividend, divisor))
                return new Tuple<string, string>("0", dividend);

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
                else
                {
                    break; // return only integer part
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

            return new Tuple<string, string>(result.ToString(), part);
        }
    }
}
