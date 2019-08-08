using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherAbstractions;

namespace WeatherConsoleApp
{
    /// <summary>
    /// Not that it makes much sense, just to demonstrate AOP by text book example - Logger wrapper.
    /// </summary>
    internal sealed class WeatherSourceAopProxy : IWeatherSourceAopProxy
    {

        private IWeatherSource _WeatherSource;
        private ILogger _Logger;


        public WeatherSourceAopProxy(IWeatherSource weatherSource, ILogger<WeatherSourceAopProxy> logger)
        {
            _WeatherSource = weatherSource ?? throw new ArgumentNullException(nameof(weatherSource));
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
        }


        public string Uri => _WeatherSource.Uri;

        public bool IsConfigured => _WeatherSource.IsConfigured;

        public bool IsAuthenticated => _WeatherSource.IsAuthenticated;

        public string[] ServicedCities => _WeatherSource.ServicedCities;


        public string GetConfigurationErrors() => _WeatherSource.GetConfigurationErrors();
        
        public async Task AuthenticateAsync()
        {

            _Logger.LogInformation("Starting authentification at weather source service {0}.", _WeatherSource.Uri);

            try
            {
                await _WeatherSource.AuthenticateAsync();
                _Logger.LogInformation("Successfully authenticated at weather source service {0}.", _WeatherSource.Uri);
            }
            catch (AggregateException ex)
            {
                _Logger.LogError(ex, "Failed to authenticate at weather source service {0}: {1}", _WeatherSource.Uri,
                    ex.Flatten().InnerExceptions.Aggregate("", (current, next) => current = current + 
                    (string.IsNullOrEmpty(current) ? "" :"; ") + next.Message));
                throw;
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Failed to authenticate at weather source service {0}: {1}", _WeatherSource.Uri, ex.Message);
                throw;
            }

        }        

        public async Task<IWeatherEntry[]> FetchAsync(string[] cities)
        {

            _Logger.LogInformation("Starting fetch operation at weather source service {0}.", _WeatherSource.Uri);

            try
            {
                var result = await _WeatherSource.FetchAsync(cities);
                _Logger.LogInformation("Successfully finished fetch operation at weather source service {0}.", _WeatherSource.Uri);
                return result;
            }
            catch (AggregateException ex)
            {
                _Logger.LogError(ex, "Failed to fetch at weather source service {0}: {1}", _WeatherSource.Uri,
                    ex.Flatten().InnerExceptions.Aggregate("", (current, next) => current = current +
                    (string.IsNullOrEmpty(current) ? "" : "; ") + next.Message));
                throw;
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Failed to fetch at weather source service {0}: {1}", _WeatherSource.Uri, ex.Message);
                throw;
            }

        }
                
    }

}
