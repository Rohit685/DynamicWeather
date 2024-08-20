using System;
using Rage;
using Rage.Native;

namespace DynamicWeather.Helpers;

internal static class GameTimeImproved
{
    internal static int day;
    internal static int year;
    internal static int month;
    internal static int hour;
    internal static int minute;
    internal static int second;
    internal static bool dayReset = false;
    internal static bool TimeInit = false;
    
    internal static void Process()
    {
        NativeFunction.Natives.SET_CLOCK_DATE(1,1,2024);
        year = NativeFunction.Natives.GET_CLOCK_YEAR<int>();
        month = NativeFunction.Natives.GET_CLOCK_MONTH<int>();
        day = NativeFunction.Natives.GET_CLOCK_DAY_OF_MONTH<int>();
        hour = NativeFunction.Natives.GET_CLOCK_HOURS<int>();
        minute = NativeFunction.Natives.GET_CLOCK_MINUTES<int>();
        second = NativeFunction.Natives.GET_CLOCK_SECONDS<int>();
        TimeInit = true;
        while (true)
        {
            GameFiber.Yield();
            hour = NativeFunction.Natives.GET_CLOCK_HOURS<int>();
            minute = NativeFunction.Natives.GET_CLOCK_MINUTES<int>();
            second = NativeFunction.Natives.GET_CLOCK_SECONDS<int>();
            if (hour == 0 && !dayReset)
            {
                if (day == DateTime.DaysInMonth(year, month))
                {
                    day = 1;
                    if (month == 12)
                    {
                        month = 1;
                        year++;
                    }
                    else
                    {
                        month++;
                    }
                }
                else
                {
                    day++;
                }
                dayReset = true;
            }

            if (hour == 1)
            {
                dayReset = false;
            }
        }
    }
    
    internal static DateTime GetTime()
    {
            return new DateTime(year, month,day,hour, minute, second);
    }

    internal static bool IsNightTime()
    {
        return hour > 21 || hour < 6;
    }
    
    internal static bool IsNightTime(DateTime time)
    {
        return time.Hour > 21 || time.Hour < 6;
    }
    
        
    internal static string GetTimeString()
    {
        if (Settings.EnableMilitaryClock)
        {
            return GetTime().ToString("HH:mm");
        }
        return GetTime().ToString("hh:mm tt");
    }
    
    internal static string GetTimeString(DateTime time)
    {
        if (Settings.EnableMilitaryClock)
        {
            return time.ToString("HH:mm");
        }
        return time.ToString("hh:mm tt");
    }
}