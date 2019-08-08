using WeatherAbstractions;

namespace WeatherConsoleApp
{
    /// <summary>
    /// In order to support standard ASP .NET Core DI different interface should be used for AOP proxy.
    /// </summary>
    internal interface IWeatherSourceAopProxy : IWeatherSource
    {
    }
}
