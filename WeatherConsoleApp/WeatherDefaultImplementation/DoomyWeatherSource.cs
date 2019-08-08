using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherAbstractions;

[assembly: InternalsVisibleTo("WeatherDefaultImplementationTests")]
namespace WeatherDefaultImplementation
{
    /// <summary>
    /// only for unit tests to test base <see cref="IWeatherSource"/> implementation without
    /// actually making HTTP requests
    /// </summary>
    internal sealed class DoomyWeatherSource : WeatherSourceBase
    {

        protected override string ConcreteClassName => nameof(DoomyWeatherSource);


        public DoomyWeatherSource(ILogger<DefaultWeatherSource> Logger, DefaultWeatherSourceOptions options) :
            base(Logger, options) { }


        protected override Task<IWeatherEntry> FetchWeatherDataAsync(string city)
        {

            _Logger.LogDebug(string.Format("Fetching test weather data for city {0}.", city));
            _Logger.LogDebug(string.Format("Fetched test weather data for city {0}.", city));

            var rnd = new Random();

            return Task.FromResult<IWeatherEntry>(new DefaultWeatherData
                {
                    City = city,
                    EffectiveAt = DateTime.UtcNow,
                    TemperatureUnit = _Options.TemperatureUnit,
                    SourceId = _Options.SourceId,
                    Precipitation = rnd.Next(0, 100),
                    Temperature = Math.Round(rnd.Next(0, 50) * rnd.NextDouble()),
                    Weather = string.Empty
                });

        }

        protected override Task<string> FetchBearerTokenAsync()
        {
            
            _Logger.LogDebug("Fetching test Bearer token.");
            _Logger.LogDebug("Bearer test token fetched.");
            return Task.FromResult(Guid.NewGuid().ToString());

        }

        protected override Task<string[]> FetchServicedCitiesAsync()
        {
            _Logger.LogDebug("Fetching test serviced cities.");
            _Logger.LogDebug("Serviced cities fetched.");

            return Task.FromResult<string[]>(new string[] { "Vilnius", "Baku", "Minsk", "London", "New York" });

        }

    }
}
