using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherAbstractions;
using System.Linq;

namespace WeatherDefaultImplementation
{
    /// <summary>
    /// Base implementation of <see cref="IWeatherSource"/> excluding HTTP requests functionality.
    /// Used to allow unit testing that cannot involve real HTTP requests.
    /// </summary>
    public abstract class WeatherSourceBase : IWeatherSource
    {

        protected ILogger _Logger;
        protected DefaultWeatherSourceOptions _Options;
        protected string _BearerToken;
        private string[] _ServicedCities = new string[] { };
        private bool _IsAuthenticated = false;


        public WeatherSourceBase(ILogger<DefaultWeatherSource> Logger, IOptions<DefaultWeatherSourceOptions> options)
        {
            _Logger = Logger ?? throw new ArgumentNullException(nameof(Logger));
            _Options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        public WeatherSourceBase(ILogger<DefaultWeatherSource> Logger, DefaultWeatherSourceOptions options)
        {
            _Logger = Logger ?? throw new ArgumentNullException(nameof(Logger));
            _Options = options ?? throw new ArgumentNullException(nameof(options));
        }


        /// <summary>
        /// to show real class name in logs and exceptions
        /// </summary>
        protected abstract string ConcreteClassName { get; }


        public string Uri => string.Format("uri://{0}@{1}", ConcreteClassName, _Options?.Url ?? string.Empty);

        public bool IsConfigured => _Options.IsValid();

        public bool IsAuthenticated => _IsAuthenticated;

        public string[] ServicedCities => _ServicedCities;


        public string GetConfigurationErrors() => _Options.GetErrors();

        public async Task AuthenticateAsync()
        {

            if (!IsConfigured)
            {
                var ex = new InvalidOperationException(string.Format("Service {0} is not configured: {1}",
                    ConcreteClassName, GetConfigurationErrors()));
                _Logger.LogError(ex, ex.Message);
                throw ex;
            }

            _Logger.LogDebug(string.Format("Starting {0} authentication.", ConcreteClassName));

            _BearerToken = await FetchBearerTokenAsync();
            _ServicedCities = await FetchServicedCitiesAsync();
            _IsAuthenticated = true;

            _Logger.LogDebug(string.Format("{0} authentication successful.", ConcreteClassName));

        }

        public async Task<IWeatherEntry[]> FetchAsync(string[] cities)
        {

            var validCities = cities?.Where(c => !string.IsNullOrWhiteSpace(c));

            if (validCities == null || validCities.Count() < 1)
            {
                _Logger.LogError(string.Format("Null argument exception in {0}.FetchAsync for parameter cities.", ConcreteClassName));
                throw new ArgumentNullException(nameof(cities));
            }

            if (!IsAuthenticated)
            {
                var ex = new InvalidOperationException(string.Format("Service {0} is not authenticated: {1}",
                    ConcreteClassName, GetConfigurationErrors()));
                _Logger.LogError(ex, ex.Message);
                throw ex;
            }

            var servicedCities = _ServicedCities.Where(c => cities.Any(s => s.CityEquals(c)));
            if (servicedCities.Count() < 1)
            {
                var ex = new ArgumentException(string.Format("None of the cities requested ({0}) are currently serviced.",
                    string.Join(", ", validCities.ToArray())));
                _Logger.LogError(ex, ex.Message);
                throw ex;
            }

            var tasks = new List<Task<IWeatherEntry>>();
            foreach (var city in servicedCities) tasks.Add(FetchWeatherDataAsync(city));

            return await Task.WhenAll(tasks);

        }


        protected abstract Task<IWeatherEntry> FetchWeatherDataAsync(string city);
        
        protected abstract Task<string> FetchBearerTokenAsync();
        
        protected abstract Task<string[]> FetchServicedCitiesAsync();    

    }

}
