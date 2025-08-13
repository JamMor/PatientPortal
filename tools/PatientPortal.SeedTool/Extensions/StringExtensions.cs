public static class StringExtensions
{
    /// <summary>
    /// Truncates the string to a specified maximum length if it exceeds it,
    /// otherwise returns the original string.
    /// </summary>
    /// <param name="value">The original string.</param>
    /// <param name="maxLength">The maximum allowed length.</param>
    /// <returns>The clamped or original string.</returns>
    public static string Clamp(this string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        // Use Math.Min to ensure the length used in Substring is not greater than
        // the actual length of the string, preventing an exception.
        int length = Math.Min(value.Length, maxLength);

        // If the original length is less than or equal to maxLength, Substring will
        // return the original string's content.
        return value.Substring(0, length);
    }
}
