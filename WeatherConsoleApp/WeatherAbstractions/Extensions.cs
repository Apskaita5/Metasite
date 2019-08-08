using System;

namespace WeatherAbstractions
{
    public static class Extensions
    {

        /// <summary>
        /// extension to uniformly treat city names in case and padding insensitive way
        /// </summary>
        /// <param name="city"></param>
        /// <param name="cityToCompare"></param>
        public static bool CityEquals(this string city, string cityToCompare)
        {
            if (string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(cityToCompare)) return false;
            return city.Trim().Equals(cityToCompare.Trim(), StringComparison.OrdinalIgnoreCase);
        }

    }
}
