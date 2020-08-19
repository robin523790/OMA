using System;
using System.Text;
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

            // remove any leading zeros (possible if one addend has leading zeros, or if they are negative)
            while (result.Length > 1 && result[0] == '0')
            {
                result.Remove(0, 1);
            }

            // if both addends are negative, the result is always negative too
            // (the case of only one addend being negative is handled above as a subtraction)
            if (negativeA && negativeB)
            {
                result.Insert(0, '-');
            }

            return result.ToString();
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
    }
}
