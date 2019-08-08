namespace WeatherAbstractions
{
    /// <summary>
    /// an abstraction for weather entry formater that defines how an entry is diplayed in the console
    /// </summary>
    public interface IWeatherEntryFormater
    {

        /// <summary>
        /// Gets a description of a weather entry
        /// </summary>
        /// <param name="weatherEntry">a weather entry to describe</param>
        string Format(IWeatherEntry weatherEntry);

    }
}
