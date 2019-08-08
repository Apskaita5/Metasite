using System.Threading.Tasks;

namespace WeatherAbstractions
{
    /// <summary>
    /// An abstraction of a service that can fetch <see cref="IWeatherEntry">weather entries</see> 
    /// from an arbitrary source (REST API, RabitMQ, file etc.).
    /// Implementations shall provide contructor (or constructors) that is compartible 
    /// with ASP .NET Core DI container.
    /// </summary>                                              
    public interface IWeatherSource
    {

        /// <summary>
        /// Gets an Uri description of service. Let the conventional format be:
        /// uri://[Service Type]@[Resource Address]:[Resource Port, if exists]?option=value&option=value
        /// E.g. uri://DefaultWeatherSource@https://metasite-weather-api.herokuapp.com/api
        /// Credentials SHALL NOT be included in service uri, as the uri is displayed in logs.
        /// </summary>
        string Uri { get; }

        /// <summary>
        /// Gets whether the service has been provided with all the options (settings) that are required to use
        /// the service (including authentication).
        /// </summary>
        bool IsConfigured { get; }

        /// <summary>
        /// Gets whether the service has succesfully authenticated in the source system. 
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets a names of the cities that the service (the source system) can provide weather data for.
        /// Should only be trusted after authentification as the service might not know serviced cities
        /// before it connects to a (remote) source system.
        /// </summary>
        string[] ServicedCities { get; }


        /// <summary>
        /// Gets a description of configuration errors. Returns an empty string if there are no errors.
        /// </summary>
        /// <returns>a description of configuration errors if there are any; an empty string otherwise</returns>
        string GetConfigurationErrors();

        /// <summary>
        /// Initializes the service and authorizes against source system.
        /// </summary>
        /// <remarks>Use of postfix Async in interfaces is debatable, but has no impact on the functionality itself.</remarks>
        Task AuthenticateAsync();

        /// <summary>
        /// Fetches a list of <see cref="IWeatherEntry">weather data</see> from the source system.
        /// </summary>
        /// <param name="cities">names of the cities to fetch the weather data for</param>
        /// <remarks>Use of postfix Async in interfaces is debatable, but has no impact on the functionality itself.</remarks>
        Task<IWeatherEntry[]> FetchAsync(string[] cities);

    }
}
