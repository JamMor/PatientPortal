namespace PatientPortal.SeedTool.Utilities;

internal class Rand
{
    /// <summary>
    /// Returns a random integer between the specified minimum and maximum values, inclusive.
    /// </summary>
    /// <param name="min">The minimum value (inclusive).</param>
    /// <param name="max">The maximum value (inclusive).</param>
    /// <returns>A random integer between min and max.</returns>
    public static int Between(int min, int max)
    {
        return Random.Shared.Next(min, max + 1);
    }

    /// <summary>
    /// Returns a random integer between 1 and the specified maximum value, inclusive.
    /// </summary>
    /// <param name="max">The maximum value (inclusive).</param>
    /// <returns>A random integer between 1 and max.</returns>
    public static int BetweenOneAnd(int max)
    {
        return Between(1, max);
    }

    /// <summary>
    /// Returns a random subset of the specified enumerable with the given count (or
    /// maximum available if count exceeds the number of elements).
    /// Warning: This method is O(n log n) and should not be used for excessively large collections.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="list">The enumerable to select from.</param>
    /// <param name="count">The number of elements to select.</param>
    /// <returns>A list containing up to <paramref name="count"/> random elements from the original enumerable.</returns>
    public static List<T> GetRandomSubset<T>(IEnumerable<T> list, int count)
    {
        return list.OrderBy(_ => Random.Shared.Next()).Take(count).ToList();
    }

    /// <summary>
    /// Returns a random element from the specified read-only list.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="list">The read-only list to select from.</param>
    /// <returns>A random element from the list.</returns>
    /// <exception cref="ArgumentException">Thrown if the list is null or empty.</exception>
    public static T GetRandomElement<T>(IReadOnlyList<T> list)
    {
        if (list == null || list.Count == 0)
        {
            throw new ArgumentException("List cannot be null or empty.", nameof(list));
        }

        return list[Random.Shared.Next(list.Count)];
    }

    /// <summary>
    /// Returns a random double between 0.0 and 1.0.
    /// </summary>
    /// <returns>A random double between 0.0 and 1.0.</returns>
    public static double RandomDouble()
    {
        return Random.Shared.NextDouble();
    }
}
