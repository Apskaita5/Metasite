using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherAbstractions;
using Newtonsoft.Json;

namespace WeatherDefaultImplementation
{

    /// <summary>
    /// Actual implementation of <see cref="IWeatherSource"/> using the REST API provided by the task.
    /// </summary>
    public class DefaultWeatherSource : WeatherSourceBase
    {

        protected override string ConcreteClassName => nameof(DefaultWeatherSource);


        public DefaultWeatherSource(ILogger<DefaultWeatherSource> Logger, IOptions<DefaultWeatherSourceOptions> options) :
            base(Logger, options) { }
        

        protected override async Task<IWeatherEntry> FetchWeatherDataAsync(string city)
        {

            _Logger.LogDebug(string.Format("Fetching weather data for city {0}.", city));

            var responseJson = await RestHelper.DownloadJsonAsync(_Options.GetApiMethodUrl("/api/Weather/")
                + System.Web.HttpUtility.UrlEncode(city), _BearerToken, _Options.GetEncodingOrDefault(), 
                _Options.GetRetriesOrDefault(), _Options.GetRetryEverySecondsOrDefault(), _Logger);

            try
            {
                var result = new DefaultWeatherData(JsonConvert.DeserializeObject<CityWeather>(responseJson), _Options);
                _Logger.LogDebug(string.Format("Fetched weather data for city {0}.", city));
                return result;
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Failed to deserialize REST API city weather response: {0}", responseJson);
                throw;
            }

        } 

        protected override async Task<string> FetchBearerTokenAsync()
        {

            _Logger.LogDebug("Fetching Bearer token.");

            var request = new AuthorizationRequest
            {
                username = _Options.User,
                password = _Options.Password
            };

            var responseJson = await RestHelper.DownloadJsonByPostAsync(_Options.GetApiMethodUrl("/api/authorize"),
                JsonConvert.SerializeObject(request), _Options.GetEncodingOrDefault(), _Options.GetRetriesOrDefault(),
                _Options.GetRetryEverySecondsOrDefault(), _Logger);

            try
            {
                var result = JsonConvert.DeserializeObject<AuthorizationResponse>(responseJson).bearer;
                _Logger.LogDebug("Bearer token fetched.");
                return result;
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Failed to deserialize web authorization response for Bearer token: {0}", responseJson);
                throw;
            }

        }

        protected override async Task<string[]> FetchServicedCitiesAsync()
        {

            _Logger.LogDebug("Fetching serviced cities.");

            var responseJson = await RestHelper.DownloadJsonAsync(_Options.GetApiMethodUrl("/api/Cities"), _BearerToken,
                _Options.GetEncodingOrDefault(), _Options.GetRetriesOrDefault(), _Options.GetRetryEverySecondsOrDefault(), _Logger);

            try
            {
                var result = JsonConvert.DeserializeObject<List<string>>(responseJson);
                _Logger.LogDebug("Serviced cities fetched.");
                return result.ToArray();
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Failed to deserialize web authorization response for cities: {0}", responseJson);
                throw;
            }

        }

    }

}
