using System;
using Rage;

namespace DynamicWeather.Extensions;

public static class ArrayExtensions
{
    public static T GetNext<T>(this T[] array, int currentIndex)
    {
        currentIndex++;
        if (currentIndex < 0 || currentIndex >= array.Length)
        {
            Game.LogTrivial("Index out of bounds. Returning first element.");
            return array[0];
        }
        return array[currentIndex];
    }
    
    public static T Get<T>(this T[] array, int currentIndex)
    {
        if (currentIndex < 0 || currentIndex >= array.Length)
        {
            Game.LogTrivial("Index out of bounds. Returning first element.");
            return array[0];
        }
        return array[currentIndex];
    }
}