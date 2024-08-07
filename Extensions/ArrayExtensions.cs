using System;

namespace DynamicWeather.Extensions;

public static class ArrayExtensions
{
    public static T GetNext<T>(this T[] array, int currentIndex)
    {
        if (currentIndex < 0 || currentIndex >= array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(currentIndex), "Current index is out of range.");
        }
        return array[(currentIndex + 1) % array.Length];
    }
}