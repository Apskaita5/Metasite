using System;
using WeatherAbstractions;
using WeatherDefaultImplementation;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WeatherDefaultImplementationTests
{
    [TestClass]
    public class DefaultWeatherStoreTest
    {

        private static DefaultWeatherData GetDoomyData()
        {
            var rnd = new Random();
            var cities = new string[] { "Vilnius", "London", "New York" };
            return new DefaultWeatherData
            {
                City = cities[rnd.Next(0, 2)],
                EffectiveAt = DateTime.UtcNow,
                TemperatureUnit = TemperatureUnits.Celsius,
                SourceId = "SomeId123",
                Precipitation = rnd.Next(0, 100),
                Temperature = Math.Round(rnd.Next(0, 50) * rnd.NextDouble()),
                Weather = string.Empty
            };
        }

        [TestMethod]
        public void NormalWorkflowTest()
        {

            var logger = new NullLogger<DefaultWeatherStore>();
            var service = new DefaultWeatherStore(logger);

            var entries = new DefaultWeatherData[] { GetDoomyData() , GetDoomyData() , GetDoomyData() };

            IWeatherEntry[] result = null;

            try
            {
                result = service.FetchAsync(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow).Result;
            }
            catch (Exception)
            {
                Assert.Fail("Unexpected exception.");
                return;
            }            

            Assert.AreEqual(result.Length, 0, "There shouldn't be any saved entries yet.");

            try
            {
                 service.SaveAsync(entries).Wait();
                result = service.FetchAsync(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow).Result;
            }
            catch (Exception)
            {
                Assert.Fail("Unexpected exception.");
                return;
            }
 
            Assert.AreEqual(result.Length, 3, "There shouldn be 3 saved entries now.");

        }

    }
}
