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
        internal static Dictionary<WeatherTypesEnum, string> EnumToTexture = new Dictionary<WeatherTypesEnum, string>()
        {
            { WeatherTypesEnum.Blizzard, "BLIZZARD" },
            { WeatherTypesEnum.Clear | WeatherTypesEnum.Neutral | WeatherTypesEnum.ExtraSunny, "EXTRASUNNY" },
            { WeatherTypesEnum.Clearing, "CLEARING" },
            { WeatherTypesEnum.Clouds, "CLOUDS" },
            { WeatherTypesEnum.Foggy, "FOGGY" },
            { WeatherTypesEnum.Halloween, "HALLOWEEN" },
            { WeatherTypesEnum.Overcast, "OVERCAST" },
            { WeatherTypesEnum.Rain, "RAIN" },
            { WeatherTypesEnum.Smog, "SMOG" },
            { WeatherTypesEnum.Snow, "SNOW" },
            { WeatherTypesEnum.Snowlight, "SNOWLIGHT" },
            { WeatherTypesEnum.Thunder, "THUNDER" },
            { WeatherTypesEnum.Xmas, "XMAS" }
        };
        
        internal static string GetTextureName(WeatherTypesEnum weatherTypesEnum)
        {
            if (EnumToTexture.ContainsKey(weatherTypesEnum))
            {
                return EnumToTexture[weatherTypesEnum];
            }
            return "EXTRASUNNY";
        }
    }
}
