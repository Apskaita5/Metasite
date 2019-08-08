using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherDefaultImplementation;
using Microsoft.Extensions.Logging.Abstractions;

namespace WeatherDefaultImplementationTests
{
    [TestClass]
    public class DoomyWeatherSourceTest
    {

        [TestMethod]
        public void NormalWorkflowTest()
        {

            var options = DefaultWeatherSourceOptionsTest.GetDoomyOptions();
            var logger = new NullLogger<DefaultWeatherSource>();
            var service = new DoomyWeatherSource(logger, options);

            Assert.IsFalse(service.IsAuthenticated, "New base weather source service shall not be authenticated.");
            Assert.IsTrue(service.IsConfigured, "Base weather source service shall be configured when valid options are provided.");

            service.AuthenticateAsync().Wait();

            Assert.IsTrue(service.IsAuthenticated, "Base weather source service shall be authenticated after AuthenticateAsync is invoked.");

            var result = service.FetchAsync(new string[] { "Vilnius", "Baku", "Minsk", "Klaipeda" }).Result;

            Assert.AreEqual(result.Length, 3, "Serviced cities and request mismatch.");

            try
            {
                service.FetchAsync(new string[] { "Klaipeda" }).Wait();
                Assert.Fail("Unserviced list didn't throw.");
            }
            catch (Exception)
            { }           

        }

        [TestMethod]
        public void InvalidWorkflowTest()
        {

            var options = DefaultWeatherSourceOptionsTest.GetDoomyOptions();
            options.Password = "";
            var logger = new NullLogger<DefaultWeatherSource>();
            var service = new DoomyWeatherSource(logger, options);

            Assert.IsFalse(service.IsConfigured, "Base weather source service shall not configured when invalid options are provided.");

            try
            {
                service.AuthenticateAsync().Wait();
                Assert.Fail("Authentication with invalid options didn't throw.");
            }
            catch (Exception)
            { }   
            

        }

    }
}
