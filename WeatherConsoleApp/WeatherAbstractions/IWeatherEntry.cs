using System;

namespace WeatherAbstractions
{
    /// <summary>
    /// an abstraction for weather data
    /// </summary>
    public interface IWeatherEntry
    {

        /// <summary>
        /// a timestamp of when the entry data is effective
        /// </summary>
        DateTime EffectiveAt { get; set; }

        /// <summary>
        /// an id of the source system that provided the entry data
        /// </summary>
        string SourceId { get; set; }

        /// <summary>
        /// a city as provided in the task REST definition
        /// </summary>
        string City { get; set; }

        /// <summary>
        /// a temperature as provided in the task REST definition
        /// </summary>
        double Temperature { get; set; }

        /// <summary>
        /// units of measurement for the temperature
        /// </summary>
        TemperatureUnits TemperatureUnit { get; set; }

        /// <summary>
        /// a precipitation as provided in the task REST definition
        /// </summary>
        /// <remarks>units of measurement could also be handy, but the requirements do not specify
        /// which ones are relevant for the application; have no idea how precipitation is meassured;
        /// should be defined as enumeration having in mind multiple source systems 
        /// that could use different symbols for the same units</remarks>
        int Precipitation { get; set; }

        /// <summary>
        /// weather as provided in the task REST definition
        /// </summary>
        /// <remarks>not of much use unless could be somehow tokenized having in mind multiple source systems</remarks>
        string Weather { get; set; } 

    }
}
