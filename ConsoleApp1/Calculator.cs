using System;
using System.Text;
using System.Threading;

namespace OmaConsole
{
    public class Calculator : ICalculator
    {
        public string Add(string summandA, string summandB)
        {
            var len = Math.Max(summandA.Length, summandB.Length);
            var result = new StringBuilder();

            int carry = 0; // Carry over from one digit to the next...
            for (int i = 0; i < len; i++)
            {
                // Get the last digits of summandA and summandB
                // - by now the code ensures that both are positive
                char charA = i < summandA.Length ? summandA[summandA.Length - 1 - i] : '0';
                char charB = i < summandB.Length ? summandB[summandB.Length - 1 - i] : '0';
                int valA = (int)Char.GetNumericValue(charA);
                int valB = (int)Char.GetNumericValue(charB);
                int res = valA + valB;
                res += carry;     // Add old carry-over value
                carry = res / 10; // Carry over the 10s to the next digit
                res %= 10;        // Ignore the 10s here
                result.Insert(0, res.ToString());
            }

            // insert last carry-over value, if any
            if (carry > 0)
            {
                result.Insert(0, carry.ToString());
            }

            return result.ToString();
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
            var result = new StringBuilder();

            int carry = 0; // Carry over from one digit to the next...
            for (int i = 0; i < len; i++)
            {
                // Get the last digits of minuend and subtrahend
                // - by now the code ensures that both are positive
                char charA = i < minuend.Length ? minuend[minuend.Length - 1 - i] : '0';
                char charB = i < subtrahend.Length ? subtrahend[subtrahend.Length - 1 - i] : '0';
                int valA = (int)Char.GetNumericValue(charA);
                int valB = (int)Char.GetNumericValue(charB);
                int res = valA - valB;
                res -= carry;     // Add old carry-over value
                if (res < 0)
                {
                    res += 10;
                    carry = 1;    // Take 10 from the next digit
                }
                else
                {
                    carry = 0;
                }
                result.Insert(0, res.ToString());
            }

            // remove any leading zeros, if any
            while (result.Length > 1 && result[0] == '0')
            {
                result.Remove(0, 1);
            }

            return result.ToString();
        }

        public string Multiply(string factorA, string factorB)
        {
            string result = "0";

            for (int a = factorA.Length - 1; a >= 0; a--)
            {
                string rowResult = "0";

                for (int b = factorB.Length - 1; b >= 0; b--)
                {
                    // Get the last digits of factorA and factorB
                    // - by now the code ensures that both are positive
                    int valA = (int)Char.GetNumericValue(factorA[a]);
                    int valB = (int)Char.GetNumericValue(factorB[b]);
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
                
                result = Add(rowResult, result);
            }

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
            // Do an integer division, multiply again with divisor. Voila!
            string divided = Div(dividend, divisor);
            string multipl = Multiply(divided, divisor);
            return Sub(dividend, multipl);
        }

        public string Div(string dividend, string divisor)
        {
            if (divisor == "0")
                throw new DivideByZeroException();

            if (divisor == "1")
                return dividend;

            if (!GreaterThanOrEqual(dividend, divisor))
                return "0";

            var result = new StringBuilder();

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
                for (; GreaterThanOrEqual(part, divisor); part = Sub(part, divisor))
                {
                    ++lastDigit; // how many times does divisor fit into part?
                }

                if (lastDigit != 0 || result.Length != 0)  // skip leading zeros...
                    result.Append(lastDigit.ToString());
            }

            return result.ToString();
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
    }
}
