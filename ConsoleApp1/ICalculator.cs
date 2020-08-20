namespace OmaConsole
{
    public interface ICalculator
    {
        /// <summary>
        /// Sums up the numeric representation of the two string parameters.
        /// </summary>
        /// <param name="summandA">Summand A.</param>
        /// <param name="summandB">Summand B.</param>
        /// <returns>The string representation of the sum.</returns>
        string Add(string summandA, string summandB);

        /// <summary>
        /// Subtracts the subtrahend from the minuend.
        /// </summary>
        /// <param name="minuend">The minuend.</param>
        /// <param name="subtrahend">The subtrahend.</param>
        /// <returns>The difference.</returns>
        string Sub(string minuend, string subtrahend);

        /// <summary>
        /// Multiplies the numeric representation of the two string parameters.
        /// </summary>
        /// <param name="factorA">Faktor A.</param>
        /// <param name="factorB">Faktor B.</param>
        /// <returns>The string representation of the product.</returns>
        string Multiply(string factorA, string factorB);

        /// <summary>
        /// Raises the numeric representation of a string value to the power of a specified value.
        /// </summary>
        /// <param name="baseValue">The number to raise to the exponent power.</param>
        /// <param name="exponent">The exponent to raise value by.</param>
        /// <returns>The result of raising value to the exponent power.</returns>
        string Pow(string baseValue, string exponent);

        /// <summary>
        /// Raises the numeric representation of a string value to the power of 2.
        /// </summary>
        /// <param name="baseValue">The number to raise to the exponent power.</param>
        /// <returns>The result of raising value to the exponent power.</returns>
        string Sqr(string baseValue);

        /// <summary>
        /// Finds the remainder after division of a dividend by a divisor.
        /// </summary>
        /// <param name="dividend">The dividend.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns>The remainder of the division.</returns>
        string Mod(string dividend, string divisor);

        /// <summary>
        /// Returns the integer value of the division of a dividend by a divisor.
        /// </summary>
        /// <param name="dividend">The dividend.</param>
        /// <param name="divisor">The divisor.</param>
        /// <returns>The integer value of the division. No rounding of the result takes place.</returns>
        string Div(string dividend, string divisor);

        /// <summary>
        /// Returns a value that indicates whether the numeric representation of the left string is greater than or equal to the numeric representation of the right string.
        /// </summary>
        /// <param name="left">The left string.</param>
        /// <param name="right">The right string.</param>
        /// <returns>true if left is greater than right; otherwise, false.</returns>
        bool GreaterThanOrEqual(string left, string right);

        /// <summary>
        /// Returns a value that indicates whether the numeric representation of the left string is greater than the numeric representation of the right string.
        /// </summary>
        /// <param name="left">The left string.</param>
        /// <param name="right">The right string.</param>
        /// <returns>true if left is greater than right; otherwise, false.</returns>
        bool GreaterThan(string left, string right);
    }
}
