using System;
using System.Collections.Generic;
using Rage;

namespace DynamicWeather.Extensions;

public static class ListExtensions
{
    public static T GetNext<T>(this List<T> list, int currentIndex)
    {
        currentIndex++;
        if (currentIndex < 0 || currentIndex >= list.Count)
        {
            Game.LogTrivial("Index out of bounds. Returning first element.");
            return list[0];
        }
        return list[currentIndex];
    }
    
    public static T Get<T>(this List<T> list, int currentIndex)
    {
        if (currentIndex < 0 || currentIndex >= list.Count)
        {
            Game.LogTrivial("Index out of bounds. Returning first element.");
            return list[0];
        }
        return list[currentIndex];
    }
}