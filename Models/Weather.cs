using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
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
        public WeatherTypesEnum WeatherType { get; set; }
        
        [XmlAttribute]
        public string WeatherName { get; set; }
       
        [XmlIgnore]
        public int Temperature { get; set; }
        
        [XmlAttribute]
        public int MinTemperature { get; set; }
        
        [XmlAttribute]
        public int MaxTemperature { get; set; }
        
        [XmlIgnore]
        internal Texture DayTexture { get; set; }
        
        [XmlIgnore]
        internal Texture NightTexture { get; set; }
        
        [XmlIgnore]
        internal DateTime WeatherTime { get; set; }

        internal Weather(WeatherTypesEnum weatherType, string weatherName, int temperature, int minTemperature, int maxTemperature)
        {
            WeatherType = weatherType;
            WeatherName = weatherName.ToUpper();
            Temperature = temperature;
            MinTemperature = minTemperature;
            MaxTemperature = maxTemperature;
            DayTexture = null;
            NightTexture = null;
        }

        internal int GetTemperature(Weather weather)
        {
            if (EntryPoint.isRealLifeWeatherSyncRunning) return Temperature;
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

            return Forecast.random.Next(minTemperature, maxTemperature);
        }
        
        internal void Draw(Rage.Graphics g)
        {
            SizeF size = Game.Resolution;
            String f =
                $"{Temperature.ToString()}° {(Weathers.usingMuricaUnits ? "F" : "C")}\n{GameTimeImproved.GetTimeString()}";
            TextureHelper.DrawText(g, new Text(f, 37, Color.White), size.Width - 200, size.Height / 5);
            TextureHelper.DrawTexture(g, GetTexture(), size.Width - 200, size.Height / 10, 96, 96);
        }
        

        public Weather() { }

        public Weather Clone()
        {
            Weather returnVal = new Weather(this.WeatherType, this.WeatherName, this.Temperature,
                this.MinTemperature, this.MaxTemperature);
            returnVal.DayTexture = this.DayTexture;
            returnVal.NightTexture = this.NightTexture;
            return returnVal;
        }

        internal Texture GetTexture()
        {
            if (GameTimeImproved.IsNightTime(WeatherTime)) return NightTexture;
            return DayTexture;
        }
    }

    [XmlRoot("Weathers")]

    public class Weathers
    {
        internal static Dictionary<WeatherTypesEnum, Weather> WeatherData = new Dictionary<WeatherTypesEnum, Weather>();
        internal static bool usingMuricaUnits = true;
        
        [XmlElement("Weather")]
        public Weather[] AllWeathers;

        [XmlAttribute("UseFreedomUnits")] public bool MuricaUnits = true; 
        
        internal Weathers() { }

        internal static void DeserializeAndValidateXML()
        {
            XMLParser<Weathers> xmlParser = new(@"Plugins/DynamicWeather/Weathers.xml");
            Weathers data = xmlParser.DeserializeXML();
            usingMuricaUnits = data.MuricaUnits;
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
