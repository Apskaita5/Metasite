namespace WeatherDefaultImplementation
{
    /// <summary>
    /// REST API proxy
    /// </summary>
    internal class CityWeather
    {
        public string city { get; set; }
        public double temperature { get; set; }
        public int precipitation { get; set; }
        public string weather { get; set; }
    }
}
