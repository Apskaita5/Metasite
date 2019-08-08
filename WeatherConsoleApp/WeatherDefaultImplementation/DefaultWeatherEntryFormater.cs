using System;
using WeatherAbstractions;

namespace WeatherDefaultImplementation
{
    /// <summary>
    /// default implementation of <see cref="IWeatherEntryFormater"/>
    /// </summary>
    public sealed class DefaultWeatherEntryFormater : IWeatherEntryFormater
    {

        public string Format(IWeatherEntry weatherEntry)
        {
            if (weatherEntry == null) throw new ArgumentNullException(nameof(weatherEntry));
            return string.Format("Weather in {0} at {1:g}: temperature - {2:F1}{3}, precipitation - {4}, other info - {5}",
                weatherEntry.City, weatherEntry.EffectiveAt.ToLocalTime(), weatherEntry.Temperature, 
                weatherEntry.TemperatureUnit==TemperatureUnits.Celsius ? "C":"F", weatherEntry.Precipitation, 
                weatherEntry.Weather);
        }

    }
}
