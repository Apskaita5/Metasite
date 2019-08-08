using System;
using System.Runtime.CompilerServices;
using WeatherAbstractions;

[assembly: InternalsVisibleTo("WeatherDefaultImplementationTests")]
namespace WeatherDefaultImplementation
{
    
    /// <summary>
    ///  default implementation of <see cref="IWeatherEntry"/>
    /// </summary>
    internal sealed class DefaultWeatherData : IWeatherEntry
    {

        public DefaultWeatherData()
        {
        }

        public DefaultWeatherData(CityWeather cityWeather, DefaultWeatherSourceOptions options)
        {
            EffectiveAt = DateTime.UtcNow;
            SourceId = options.GetSourceIdOrDefault();
            City = cityWeather.city;
            Temperature = cityWeather.temperature;
            Precipitation = cityWeather.precipitation;
            Weather = cityWeather.weather;
        }

        public DateTime EffectiveAt { get; set; }
        public string City { get; set; }
        public double Temperature { get; set; }
        public TemperatureUnits TemperatureUnit { get; set; }
        public int Precipitation { get; set; }
        public string SourceId { get; set; }
        public string Weather { get; set; }
        
    }

}
