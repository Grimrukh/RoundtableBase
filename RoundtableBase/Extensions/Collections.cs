namespace RoundtableBase.Extensions;

public static class Collections
{
    /// <summary>
    /// Forgiving version of `List` slicer that automatically clamps the indices to the list bounds.
    ///
    /// If `startIndex` is greater than `endIndex`, an empty list is returned.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="startIndex"></param>
    /// <param name="endIndex"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> GetRangeSafe<T>(this List<T> list, int startIndex, int endIndex)
    {
        endIndex = Math.Min(endIndex, list.Count);
        return startIndex > endIndex 
            ? [] 
            : list.GetRange(startIndex, endIndex - startIndex);
    }
}