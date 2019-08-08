using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherAbstractions;

namespace WeatherDefaultImplementation
{
    /// <summary>
    /// an in-memory implementation of <see cref="IWeatherStore"/>
    /// </summary>
    public sealed class DefaultWeatherStore : IWeatherStore
    {

        // as per requirement letter there will be no concurrency issues but just to be on safe side
        private ConcurrentBag<IWeatherEntry> _List = new ConcurrentBag<IWeatherEntry>();
        private ILogger _Logger;


        public DefaultWeatherStore(ILogger<DefaultWeatherStore> logger)
        {
            _Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public string Uri => "uri://WeatherDefaultImplementation@InMemoryList";

        public bool IsConfigured => true;

        public bool IsInitialized => true;


        public string GetConfigurationErrors() => string.Empty;

        
        public Task InitializeAsync() => Task.FromResult(0);
        
        public Task SaveAsync(IWeatherEntry[] weatherEntries)
        {
            if (weatherEntries == null)
            {
                _Logger.LogError("Null argument exception in DefaultWeatherStore.SaveAsync for parameter weatherEntries.");
               throw new ArgumentNullException(nameof(weatherEntries));
            }             
            foreach (var entry in weatherEntries) _List.Add(entry);
            _Logger.LogDebug(string.Format("DefaultWeatherStore has saved {0} new weather entries.", weatherEntries.Length));
            return Task.FromResult(0);
        }

        public Task<IWeatherEntry[]> FetchAsync(DateTime from, DateTime to) =>
            Task.FromResult(_List.Where(e => e.EffectiveAt >= from && e.EffectiveAt <= to).ToArray());

        public Task<IWeatherEntry[]> FetchAsync(DateTime from, DateTime to, string[] cities)
        {
            if (cities == null || cities.Length < 1)
            {
                _Logger.LogError("Null argument exception in DefaultWeatherStore.FetchAsync for parameter cities.");
                throw new ArgumentNullException(nameof(cities));
            }
            return Task.FromResult(_List.Where(
                e => e.EffectiveAt >= from && e.EffectiveAt <= to
                && cities.Any(c => c.CityEquals(e.City))
                ).ToArray());
        }                
                
    }

}
