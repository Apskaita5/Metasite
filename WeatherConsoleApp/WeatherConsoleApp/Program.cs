using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WeatherAbstractions;
using WeatherDefaultImplementation;

namespace WeatherConsoleApp
{
    class Program
    {

        private static ServiceProvider serviceProvider;
        private static Timer _Timer;        


        static void Main(string[] args)
        {

            if (!ArgsConfirmToSpecification(args, out IEnumerable<string> citiesRequested))
            {
                Console.WriteLine("To start the application use command: metaapp.exe weather --city city1, city2,...,cityn Example: metaapp.exe weather --city Vilnius, Riga");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return;
            }     

            Console.WriteLine("Starting application...");

            try
            {
                ConfigureServices();
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Failed to start application:{0}", ex.Message));
                Console.Beep();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                Environment.ExitCode = -1;
                return;
            }
            

            // to adapt the code below for WebApp use something like this:
            // https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
            // https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-2/

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            Console.WriteLine("Initializing application services...");

            InitializeServices(logger);

            ValidateServicedCities(citiesRequested, logger);

            Console.WriteLine("Application is running. Press any key to exit...");
            Console.WriteLine();

            _Timer = new Timer(async (e) =>
            {
                try
                {
                    var source = serviceProvider.GetService<IWeatherSourceAopProxy>();
                    var entries = await source.FetchAsync(citiesRequested.ToArray());
                    var store = serviceProvider.GetService<IWeatherStore>();
                    await store.SaveAsync(entries);
                    var formater = serviceProvider.GetService<IWeatherEntryFormater>();
                    Array.ForEach(entries, (entry) => Console.WriteLine(formater.Format(entry)));
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    _Timer.Change(Timeout.Infinite, Timeout.Infinite);
                    _Timer.Dispose(); // no point in waiting in this case
                    LogCriticalExceptionAndExit(ex, "Critical exception occured while running batch: {0}", logger);                    
                }                
            }, null, 0, 30000);
            
            Console.ReadKey();

            _Timer.Change(Timeout.Infinite, Timeout.Infinite);
            _Timer.Dispose(); // no point in waiting in this case

            Environment.ExitCode = 0;

        }


        private static bool ArgsConfirmToSpecification(string[] args, out IEnumerable<string> cities)
        {

            // For a real application a command line parser would be better, but it's not, therefore just check conformance to spec
            // On the other hand, city names are very tricky, can contain various symbols not to speak about blank spaces

            var citiesString = args?.Where((s, i) => i > 1).Aggregate("", (current, item) => current + item) ?? string.Empty;

            if (args == null || args.Length < 3 || !args[0].Trim().Equals("weather", StringComparison.OrdinalIgnoreCase)
                || !args[1].Trim().Equals("--city", StringComparison.OrdinalIgnoreCase) || citiesString.Contains("--"))
            {
                cities = null;
                return false;
            }
            
            cities = citiesString.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim());

            return true;

        }

        private static void ConfigureServices()
        {

            // As the requirements implicates:
            // 1. dependency inversion, dependency injection and IoC;
            // 2. overall requirements to IoC are low, Autofac et altera would be overkill;
            // 3. there are plans to move to web app but strictly within .NET Core, i.e. ASP.NET Core.
            // Reusing ASP.NET Core IoC components is a great choice.


            var dic = new Dictionary<string, string>
                {
                    {"DefaultWeatherSourceOptions:Url", "https://metasite-weather-api.herokuapp.com/"},
                    {"DefaultWeatherSourceOptions:User", Environment.GetEnvironmentVariable("METASITE_REST_USER")},
                    {"DefaultWeatherSourceOptions:Password", Environment.GetEnvironmentVariable("METASITE_REST_PASSWORD")},
                    {"DefaultWeatherSourceOptions:EncodingName", "UTF8"},
                    {"DefaultWeatherSourceOptions:Retries", "1"},
                    {"DefaultWeatherSourceOptions:RetryEverySeconds", "10"},
                    {"DefaultWeatherSourceOptions:SourceId", "Metasite" },
                    {"DefaultWeatherSourceOptions:TemperatureUnit", "celsius" } 
                }; 
            IConfiguration config = new ConfigurationBuilder()
                .AddInMemoryCollection(dic)
                .Build();

            serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddOptions()
                .Configure<DefaultWeatherSourceOptions>(config.GetSection("DefaultWeatherSourceOptions"))
                .AddSingleton<IWeatherSource, DefaultWeatherSource>()
                .AddSingleton<IWeatherStore, DefaultWeatherStore>()
                .AddSingleton<IWeatherSourceAopProxy, WeatherSourceAopProxy>()
                .AddSingleton<IWeatherEntryFormater, DefaultWeatherEntryFormater>()
                .BuildServiceProvider();

            // Microsoft sense of humor is specific :)
            // The following code sample uses a constructor that has been obsoleted in version 2.2. 
            // Proper replacements for obsolete logging APIs will be available in version 3.0.
            // In the meantime, it is safe to ignore and suppress the warnings.
            //
            // Actually shouldn't be used, just the most simple thing for demonstration
            // Don't suppress warning in order not to forget the issue when upgrading
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddDebug(LogLevel.Trace);

        }

        private static void InitializeServices(ILogger logger)
        {

            logger.LogDebug("Initializing application...");

            try
            {
                var source = serviceProvider.GetService<IWeatherSourceAopProxy>();                
                if (!source.IsConfigured) throw new InvalidOperationException(string.Format(
                    "Weather source service is not properly configured: {0}", source.GetConfigurationErrors()));

                var store = serviceProvider.GetService<IWeatherStore>();
                if (!store.IsConfigured) throw new InvalidOperationException(string.Format(
                    "Weather store service is not properly configured: {0}", store.GetConfigurationErrors()));

                var initTasks = new List<Task>()
                {
                    source.AuthenticateAsync(),
                    store.InitializeAsync()
                };

                Task.WhenAll(initTasks).Wait();

            }
            catch (Exception ex)
            {
                LogCriticalExceptionAndExit(ex, "Failed to initialize services: {0}", logger);
            }

            logger.LogDebug("Application initialization succeded.");

        }

        private static void ValidateServicedCities(IEnumerable<string> cities, ILogger logger)
        {

            logger.LogDebug("Validating serviced cities...");

            var source = serviceProvider.GetService<IWeatherSourceAopProxy>();
            var notServicedCities = cities.Where(
                c => !source.ServicedCities.Any(s => s.CityEquals(c))
                ).ToArray();

            logger.LogDebug("Serviced cities validation result: {0} out of {1} cities are serviced.", 
                (cities.Count() - notServicedCities.Count()), cities.Count());
            if (notServicedCities.Count() > 0)
                logger.LogDebug("Not serviced cities are: {0}.", string.Join(", ", notServicedCities.ToArray()));

            if (notServicedCities.Count() == cities.Count())
            {
                Console.WriteLine("None of the cities specified are currently serviced.");
                Console.WriteLine(string.Format("Currently serviced cities are: {0}",
                    string.Join(", ", source.ServicedCities)));
                Console.WriteLine("Press any key to exit application...");
                Console.ReadKey();
            }
            else if (notServicedCities.Count() > 0)
            {
                Console.WriteLine(string.Format("WARNING! The following cities are not currently serviced: {0}",
                     string.Join(", ", notServicedCities.ToArray())));
                Console.WriteLine("If you wish to proceed with the rest of the cities press Y.");
                var keyPressed = Console.ReadKey();
                if (keyPressed.KeyChar == 'y' || keyPressed.KeyChar == 'Y')
                {
                    logger.LogDebug("User decided to proceed with partial set of the cities.");
                    return;
                }
                    
            }
            else
            {
                return;
            }

            logger.LogDebug("Application exited at the serviced city validation.");

            Environment.Exit(2);

        }

        private static void LogCriticalExceptionAndExit(Exception ex, string message, ILogger logger)
        {
            logger.LogCritical(ex, message, ex.Message);
            Console.WriteLine(string.Format(message, ex.Message));
            Console.Beep();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            Environment.Exit(-1);
        }                

    }
}
