using System;
using System.Threading.Tasks;

namespace WeatherAbstractions
{
    /// <summary>
    /// an abstraction of a service that persists <see cref="IWeatherEntry">weather entries</see>
    /// in an arbitrary storage (SQL server, NOSSQL server, ES system, local file, memory etc.)
    /// Implementations shall provide contructor (or constructors) that is compartible 
    /// with ASP .NET Core DI container.
    /// </summary>
    public interface IWeatherStore
    {

        /// <summary>
        /// Gets an Uri description of service. Let the conventional format be:
        /// uri://[Service Type]@[Resource Address]:[Resource Port, if exists]?option=value&option=value
        /// E.g. uri://MySqlWeatherStore@localhost:3306
        /// Credentials SHALL NOT be included in service uri, as the uri is displayed in logs.
        /// </summary>
        string Uri { get; }

        /// <summary>
        /// Gets whether the service has been provided with all the options (settings) that are required to use
        /// the service (including authentication).
        /// </summary>
        bool IsConfigured { get; }

        /// <summary>
        /// Gets whether the service has succesfully initialized (and authenticated) in the persistence system. 
        /// </summary>
        bool IsInitialized { get; }


        /// <summary>
        /// Gets a description of configuration errors. Returns an empty string if there are no errors.
        /// </summary>
        /// <returns>a description of configuration errors if there are any; an empty string otherwise</returns>
        string GetConfigurationErrors();

        /// <summary>
        /// Initializes the service (might also authorize against persistence system, e.g. SQL server).
        /// </summary>
        /// <remarks>Use of postfix Async in interfaces is debatable, but has no impact on the functionality itself.</remarks>
        Task InitializeAsync();

        /// <summary>
        /// Saves the <see cref="IWeatherEntry">weather entries</see> to the persistent storage. 
        /// </summary>
        /// <param name="weatherEntries">weather entries to save</param>
        /// <remarks>Use of postfix Async in interfaces is debatable, but has no impact on the functionality itself.</remarks>
        Task SaveAsync(IWeatherEntry[] weatherEntries);

        /// <summary>
        /// Fetches a list of <see cref="IWeatherEntry">weather data</see> from the persistent storage.
        /// </summary>
        /// <param name="from">start of the period</param>
        /// <param name="to">en of the period</param>
        /// <remarks>Use of postfix Async in interfaces is debatable, but has no impact on the functionality itself.</remarks>
        Task<IWeatherEntry[]> FetchAsync(DateTime from, DateTime to);

        /// <summary>
        /// Fetches a list of <see cref="IWeatherEntry">weather data</see> from the persistent storage.
        /// </summary>
        /// <param name="from">start of the period</param>
        /// <param name="to">en of the period</param>
        /// <param name="cities">names of the cities to fetch the data for</param>
        /// <remarks>Use of postfix Async in interfaces is debatable, but has no impact on the functionality itself.</remarks>
        Task<IWeatherEntry[]> FetchAsync(DateTime from, DateTime to, string[] cities);

    }
}
