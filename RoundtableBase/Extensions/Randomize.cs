using System.Numerics;

namespace RoundtableBase.Extensions;

/// <summary>
/// Extensions that support quick random operations.
/// </summary>
public static class Randomize
{
    static Random Rand = new();

    /// <summary>
    /// Initialize `Rand` to an optionally known `seed` (defaults to `null`).
    /// </summary>
    /// <param name="seed"></param>
    public static void SetRand(int? seed = null)
    {
        Rand = seed == null ? new Random() : new Random(seed.Value);
    }

    /// <summary>
    /// Randomly order all elements in a list.
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Rand.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    /// <summary>
    /// Return a randomly shuffled clone of a list.
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> CloneShuffled<T>(this IEnumerable<T> list)
    {
        var clone = new List<T>(list);
        clone.Shuffle();
        return clone;
    }

    /// <summary>
    /// Get a random permutation of integers from 0 to `count` (exclusive).
    /// </summary>
    /// <param name="count"></param>
    /// <returns></returns>
    public static List<int> GetRandomPermutation(int count)
    {
        var permutation = new List<int>(count);
        for (int i = 0; i < count; i++)
            permutation.Add(i);
        permutation.Shuffle();
        return permutation;
    }

    /// <summary>
    /// Wrapper for `Rand.NextSingle()`.
    /// </summary>
    /// <returns></returns>
    public static float NextSingle()
    {
        return Rand.NextSingle();
    }

    /// <summary>
    /// Return `true` with probability `chance`.
    /// </summary>
    /// <param name="chance"></param>
    /// <returns></returns>
    public static bool RollChance(float chance)
    {
        return chance switch
        {
            < 0f => false,
            >= 1f => true,
            _ => Rand.NextSingle() < chance,
        };
    }

    public static T RollChances<T>(List<(float chance, T item)> chances)
    {
        float totalChance = 0f;
        foreach ((float chance, T _) in chances)
            totalChance += chance;

        if (totalChance <= 0f)
        {
            // Too dangerous to throw here.
            Logging.Warning("Total chance is zero. Returning random item with uniform distribution.");
            return chances[Rand.Next(0, chances.Count)].item;
        }

        float roll = Rand.NextSingle() * totalChance;
        foreach ((float chance, T item) in chances)
        {
            if (roll < chance)
                return item;
            roll -= chance;
        }

        return chances[^1].item;
    }

    public static T RollChances<T>(List<T> items, List<float> chances)
    {
        if (items.Count != chances.Count)
            throw new ArgumentException("Items and chances must have same length.");
        List<(float, T)> combined = items.Select((t, i) => (chances[i], t)).ToList();
        return RollChances(combined);
    }

    /// <summary>
    /// Keep rolling each item's chances and return the first one that succeeds. Requires a final lone item to use
    /// if all previous rolls fail.
    /// 
    /// This will progressively split the remaining probability, so:
    ///     RollChancesInSequence([(0.5, "A"), (0.5, "B")], "C")
    /// will have an overall 50% chance of "A", 25% chance of "B", and (remaining) 25% chance of "C".
    /// </summary>
    /// <param name="chances"></param>
    /// <param name="finalItem"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T RollChancesInSequence<T>(List<(float chance, T item)> chances, T finalItem)
    {
        if (chances.Count == 0)
            return finalItem;

        foreach ((float chance, T item) in chances)
        {
            if (Rand.NextSingle() < chance)
                return item;
        }

        return finalItem;
    }

    public static int RandInt(int maxValue)
    {
        return Rand.Next(maxValue);
    }

    public static int RandInt(int minValue, int maxValue)
    {
        return Rand.Next(minValue, maxValue);
    }

    public static T GetRandomWeighted<T>(this List<T> list, List<float> weights, float totalWeight)
    {
        if (list.Count == 0)
            throw new ArgumentException("List must not be empty.");
        if (list.Count != weights.Count)
            throw new ArgumentException("List and weights must have same length.");
        if (totalWeight <= 0f)
            throw new ArgumentException("Total weight must be positive.");
        float roll = Rand.NextSingle() * totalWeight;
        for (int i = 0; i < list.Count; i++)
        {
            if (roll < weights[i])
                return list[i];
            roll -= weights[i];
        }

        return list[^1]; // won't happen
    }

    public static T GetRandomWeighted<T>(this List<T> list, List<int> weights, int totalWeight)
    {
        if (list.Count == 0)
            throw new ArgumentException("List must not be empty.");
        if (list.Count != weights.Count)
            throw new ArgumentException("List and weights must have same length.");
        if (totalWeight <= 0f)
            throw new ArgumentException("Total weight must be positive.");
        int roll = Rand.Next(totalWeight);
        for (int i = 0; i < list.Count; i++)
        {
            if (roll < weights[i])
                return list[i];
            roll -= weights[i];
        }

        return list[^1]; // won't happen
    }

    /// <summary>
    /// Get a random `float` between `min` (inclusive) and `max` (exclusive).
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float RandRange(float min, float max)
    {
        return min + (max - min) * Rand.NextSingle();
    }

    /// <summary>
    /// Get a random rotation angle, in degrees, between [-180, 180).
    /// </summary>
    /// <returns></returns>
    public static float GetRandomRotation()
    {
        return 360f * Rand.NextSingle() - 180f;
    }

    /// <summary>
    /// Get a Vector3 with a random Y rotation angle, in degrees, between [-180, 180). X and Z are zero.
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetRandomRotYVector()
    {
        return new Vector3(0f, 360f * Rand.NextSingle() - 180f, 0f);
    }

    static readonly List<float> Cardinals = [0f, 90f, 180f, -90f];

    public static float GetRandomCardinalVector()
    {
        return Cardinals[Rand.Next(4)];
    }

    public static T GetRandomItem<T>(this List<T> listio)
    {
        int index = Rand.Next(0, listio.Count);
        return listio[index];
    }

    public static List<T> GetRandomItems<T>(this List<T> listio, int count, bool withReplacement = true)
    {
        List<T> items = [];
        if (withReplacement)
        {
            for (int i = 0; i < count; i++)
            {
                int index = Rand.Next(0, listio.Count);
                items.Add(listio[index]);
            }

            return items;
        }

        listio = listio.ToList(); // copy
        for (int i = 0; i < count; i++)
        {
            int index = Rand.Next(0, listio.Count);
            T item = listio[index];
            listio.RemoveAt(index);
            items.Add(item);
        }

        return items;
    }

    public static T PopRandomItem<T>(this List<T> listio)
    {
        int index = Rand.Next(0, listio.Count);
        T item = listio[index];
        listio.RemoveAt(index);
        return item;
    }
}