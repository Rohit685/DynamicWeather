using DynamicWeather.Enums;

namespace DynamicWeather.Models
{
    public class Weather
    {
        public WeatherType WeatherType { get; set; }
        public string WeatherName { get; set; }
        public int Temperature { get; set; }
        public TextureType WeatherTextureName { get; set; }
    }
}
