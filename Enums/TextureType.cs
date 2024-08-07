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
            { WeatherType.Blizzard, "BLIZZARD" },
            { WeatherType.Clear | WeatherType.Neutral | WeatherType.ExtraSunny, "EXTRASUNNY" },
            { WeatherType.Clearing, "CLEARING" },
            { WeatherType.Clouds, "CLOUDS" },
            { WeatherType.Foggy, "FOGGY" },
            { WeatherType.Halloween, "HALLOWEEN" },
            { WeatherType.Overcast, "OVERCAST" },
            { WeatherType.Rain, "RAIN" },
            { WeatherType.Smog, "SMOG" },
            { WeatherType.Snow, "SNOW" },
            { WeatherType.Snowlight, "SNOWLIGHT" },
            { WeatherType.Thunder, "THUNDER" },
            { WeatherType.Xmas, "XMAS" }
        };
        
        internal static string GetTextureName(WeatherType weatherType)
        {
            if (WeatherTypes.ContainsKey(weatherType))
            {
                return WeatherTypes[weatherType];
            }
            return "EXTRASUNNY";
        }
    }
}
