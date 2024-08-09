using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using DynamicWeather.Enums;
using DynamicWeather.Helpers;
using DynamicWeather.IO;
using Rage;

namespace DynamicWeather.Models
{
    public class Weather
    {
        [XmlIgnore]
        public WeatherTypesEnum WeatherTypesEnum { get; set; }
        
        [XmlAttribute]
        public string WeatherName { get; set; }
       
        [XmlIgnore]
        public int Temperature { get; set; }
        
        [XmlAttribute]
        public int MinTemperature { get; set; }
        
        [XmlAttribute]
        public int MaxTemperature { get; set; }
        
        [XmlIgnore]
        internal Texture Texture { get; set; }

        internal Weather(WeatherTypesEnum weatherTypesEnum, string weatherName, int temperature, int minTemperature, int maxTemperature)
        {
            WeatherTypesEnum = weatherTypesEnum;
            WeatherName = weatherName.ToUpper();
            Temperature = temperature;
            MinTemperature = minTemperature;
            MaxTemperature = maxTemperature;
            Texture = null;
        }

        internal int GetTemperature(Weather weather)
        {
            Random random = new Random();
            var timeOfDay = World.DateTime.TimeOfDay;
            int hour = timeOfDay.Hours;

            int minTemperature;
            int maxTemperature;

            switch (hour)
            {
                case int h when (h >= 0 && h < 6):
                    minTemperature = MinTemperature;
                    maxTemperature = MinTemperature + 5;
                    break;

                case int h when (h >= 6 && h < 12):
                    minTemperature = MinTemperature + 5;
                    maxTemperature = (MinTemperature + MaxTemperature) / 2;
                    break;

                case int h when (h >= 12 && h < 16):
                    minTemperature = (MinTemperature + MaxTemperature) / 2;
                    maxTemperature = MaxTemperature;
                    break;

                case int h when (h >= 16 && h < 20):
                    minTemperature = (MinTemperature + MaxTemperature) / 2;
                    maxTemperature = MaxTemperature - 5;
                    break;

                case int h when (h >= 20 && h < 24):
                    minTemperature = MinTemperature + 5;
                    maxTemperature = (MinTemperature + MaxTemperature) / 2;
                    break;

                default:
                    minTemperature = MinTemperature;
                    maxTemperature = MaxTemperature;
                    break;
            }

            return random.Next(minTemperature, maxTemperature);
        }

        public Weather() { }
    }

    [XmlRoot("Weathers")]

    public class Weathers
    {
        internal static Dictionary<WeatherTypesEnum, Weather> WeatherData = new Dictionary<WeatherTypesEnum, Weather>();
        
        [XmlElement("Weather")]
        public Weather[] AllWeathers;
        
        internal Weathers() { }

        internal static void DeserializeAndValidateXML()
        {
            XMLParser<Weathers> xmlParser = new(@"Plugins/DynamicWeather/Weathers.xml");
            Weathers data = xmlParser.DeserializeXML();
            Game.LogTrivial($"Number of weathers: {data.AllWeathers.Length}");
            Random random = new Random(DateTime.Today.Millisecond);
            foreach (Weather weather in data.AllWeathers)
            {
                if (!Enum.TryParse(weather.WeatherName, true, out WeatherTypesEnum type))
                {
                    throw new InvalidDataException($"Invalid weather name found in xml: {weather.WeatherName}");
                }
                if (WeatherData.ContainsKey(type))
                {
                    throw new InvalidDataException($"Duplicate weather types found in xml: {weather.WeatherName}");
                }
                Weather w = new Weather(type, weather.WeatherName.ToUpper(), 
                    random.Next(weather.MinTemperature, weather.MaxTemperature + 1), weather.MinTemperature, 
                    weather.MaxTemperature);
                WeatherData.Add(type, w);
            }
            if (WeatherData.Count != Enum.GetValues(typeof(WeatherTypesEnum)).Length)
            {
                throw new InvalidDataException("Not all weathers present in the xml");
            }
            Game.LogTrivial("Weathers deserialized successfully.");
        }

    }
}
