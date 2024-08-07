using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicWeather.Enums;

namespace DynamicWeather.Enums
{
    internal static class TextureType
    {
        internal static Dictionary<WeatherType, string> WeatherTypes = new Dictionary<WeatherType, string>()
        {
            { WeatherType.Blizzard, "Blizzard" },
            { WeatherType.Clear | WeatherType.Neutral | WeatherType.ExtraSunny, "ExtraSunny" },
            { WeatherType.Clearing, "Clearing" },
            { WeatherType.Clouds, "Clouds" },
            { WeatherType.Foggy, "Foggy" },
            { WeatherType.Halloween, "Halloween" },
            { WeatherType.Overcast, "Overcast" },
            { WeatherType.Rain, "Rain" },
            { WeatherType.Smog, "Smog" },
            { WeatherType.Snow, "Snow" },
            { WeatherType.Snowlight, "Snowlight" },
            { WeatherType.Thunder, "Thunder" },
            { WeatherType.Xmas, "Xmas" }
        };
        
        internal static string GetTextureName(WeatherType weatherType)
        {
            if (WeatherTypes.ContainsKey(weatherType))
            {
                return WeatherTypes[weatherType];
            }
            return "ExtraSunny";
        }
    }
}
