using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeatherAbstractions;
using WeatherDefaultImplementation;

namespace WeatherDefaultImplementationTests
{
    [TestClass]
    public class DefaultWeatherSourceOptionsTest
    {

        internal static DefaultWeatherSourceOptions GetDoomyOptions()
        {
            return new DefaultWeatherSourceOptions
            {
                Url = "https://docs.microsoft.com/",
                User = "user",
                Password = "password",
                TemperatureUnit = TemperatureUnits.Celsius,
                EncodingName = "Unicode",
                Retries = 3,
                RetryEverySeconds = 10,
                SourceId = "SomeId"
            };
        }

        [TestMethod]
        public void IsValidTest()
        {

            var invalidOptions = GetDoomyOptions();
            invalidOptions.EncodingName = null;
            Assert.IsTrue(invalidOptions.IsValid(), "EncodingName shall not be required.");
            invalidOptions.Retries = -1;
            Assert.IsTrue(invalidOptions.IsValid(), "Retries shall not be required.");
            invalidOptions.RetryEverySeconds = -1;
            Assert.IsTrue(invalidOptions.IsValid(), "RetryEverySeconds shall not be required.");
            invalidOptions.SourceId = null;
            Assert.IsTrue(invalidOptions.IsValid(), "SourceId shall not be required.");

            invalidOptions.Password = null;
            Assert.IsFalse(invalidOptions.IsValid(), "Password shall be required.");
            invalidOptions.Password = "     ";
            Assert.IsFalse(invalidOptions.IsValid(), "All blanks password shall be invalid.");

            invalidOptions.Password = "password";

            invalidOptions.User = null;
            Assert.IsFalse(invalidOptions.IsValid(), "User shall be required.");
            invalidOptions.User = "     ";
            Assert.IsFalse(invalidOptions.IsValid(), "All blanks User shall be invalid.");

            invalidOptions.User = "user";

            invalidOptions.Url = null;
            Assert.IsFalse(invalidOptions.IsValid(), "Url shall be required.");
            invalidOptions.Url = "     ";
            Assert.IsFalse(invalidOptions.IsValid(), "All blanks Url shall be invalid.");
            invalidOptions.Url = "not url";
            Assert.IsFalse(invalidOptions.IsValid(), "Malformed Url shall be invalid.");

        }

        [TestMethod]
        public void GetErrorsTest()
        {

            var invalidOptions = GetDoomyOptions();
            invalidOptions.EncodingName = null;
            Assert.IsTrue(string.IsNullOrWhiteSpace(invalidOptions.GetErrors()), "EncodingName shall not be required.");
            invalidOptions.Retries = -1;
            Assert.IsTrue(string.IsNullOrWhiteSpace(invalidOptions.GetErrors()), "Retries shall not be required.");
            invalidOptions.RetryEverySeconds = -1;
            Assert.IsTrue(string.IsNullOrWhiteSpace(invalidOptions.GetErrors()), "RetryEverySeconds shall not be required.");
            invalidOptions.SourceId = null;
            Assert.IsTrue(string.IsNullOrWhiteSpace(invalidOptions.GetErrors()), "SourceId shall not be required.");

            invalidOptions.Password = null;
            Assert.IsFalse(string.IsNullOrWhiteSpace(invalidOptions.GetErrors()), "Password shall be required.");
            invalidOptions.Password = "     ";
            Assert.IsFalse(string.IsNullOrWhiteSpace(invalidOptions.GetErrors()), "All blanks password shall be invalid.");

            invalidOptions.Password = "password";

            invalidOptions.User = null;
            Assert.IsFalse(string.IsNullOrWhiteSpace(invalidOptions.GetErrors()), "User shall be required.");
            invalidOptions.User = "     ";
            Assert.IsFalse(string.IsNullOrWhiteSpace(invalidOptions.GetErrors()), "All blanks User shall be invalid.");

            invalidOptions.User = "user";

            invalidOptions.Url = null;
            Assert.IsFalse(string.IsNullOrWhiteSpace(invalidOptions.GetErrors()), "Url shall be required.");
            invalidOptions.Url = "     ";
            Assert.IsFalse(string.IsNullOrWhiteSpace(invalidOptions.GetErrors()), "All blanks Url shall be invalid.");
            invalidOptions.Url = "not url";
            Assert.IsFalse(string.IsNullOrWhiteSpace(invalidOptions.GetErrors()), "Malformed Url shall be invalid.");

        }

        [TestMethod]
        public void GetEncodingOrDefaultTest()
        {

            var invalidOptions = GetDoomyOptions();

            invalidOptions.EncodingName = "Unicode";
            Assert.AreEqual(System.Text.Encoding.Unicode, invalidOptions.GetEncodingOrDefault(), "Unicode shall be valid encoding name.");
            invalidOptions.EncodingName = "ASCII";
            Assert.AreEqual(System.Text.Encoding.ASCII, invalidOptions.GetEncodingOrDefault(), "ASCII shall be valid encoding name.");
            invalidOptions.EncodingName = "BigEndianUnicode";
            Assert.AreEqual(System.Text.Encoding.BigEndianUnicode, invalidOptions.GetEncodingOrDefault(), "BigEndianUnicode shall be valid encoding name.");
            invalidOptions.EncodingName = "UTF32";
            Assert.AreEqual(System.Text.Encoding.UTF32, invalidOptions.GetEncodingOrDefault(), "UTF32 shall be valid encoding name.");
            invalidOptions.EncodingName = "UTF7";
            Assert.AreEqual(System.Text.Encoding.UTF7, invalidOptions.GetEncodingOrDefault(), "UTF7 shall be valid encoding name.");
            invalidOptions.EncodingName = "Unicode32";
            Assert.AreNotEqual(System.Text.Encoding.Unicode, invalidOptions.GetEncodingOrDefault(), "Unicode32 shall not be a valid encoding name.");
            Assert.AreEqual(System.Text.Encoding.UTF8, invalidOptions.GetEncodingOrDefault(), "UTF8 shall be default encoding name.");
            invalidOptions.EncodingName = "UnIcOdE";
            Assert.AreEqual(System.Text.Encoding.Unicode, invalidOptions.GetEncodingOrDefault(), "Encoding name shall not be case sensitive.");
            
        }

        [TestMethod]
        public void GetRetriesOrDefaultTest()
        {

            var invalidOptions = GetDoomyOptions();
            
            invalidOptions.Retries = 1;
            Assert.AreEqual(invalidOptions.Retries, invalidOptions.GetRetriesOrDefault(), "All positive retry values shall be valid.");
            invalidOptions.Retries = 0;
            Assert.AreEqual(3, invalidOptions.GetRetriesOrDefault(), "Default retry value shall be 3.");
            invalidOptions.Retries = null;
            Assert.AreEqual(3, invalidOptions.GetRetriesOrDefault(), "Default retry value shall be 3.");
            invalidOptions.Retries = 1001;
            Assert.AreEqual(1000, invalidOptions.GetRetriesOrDefault(), "Max retry value shall be 1000.");

        }

        [TestMethod]
        public void GetRetryEverySecondsOrDefaultTest()
        {
            var invalidOptions = GetDoomyOptions();

            invalidOptions.RetryEverySeconds = 1;
            Assert.AreEqual(invalidOptions.RetryEverySeconds, invalidOptions.GetRetryEverySecondsOrDefault(), "All positive RetryEverySeconds values shall be valid.");
            invalidOptions.RetryEverySeconds = 0;
            Assert.AreEqual(10, invalidOptions.GetRetryEverySecondsOrDefault(), "Default RetryEverySeconds value shall be 10.");
            invalidOptions.RetryEverySeconds = null;
            Assert.AreEqual(10, invalidOptions.GetRetryEverySecondsOrDefault(), "Default RetryEverySeconds value shall be 10.");
            invalidOptions.RetryEverySeconds = 43201;
            Assert.AreEqual(43200, invalidOptions.GetRetryEverySecondsOrDefault(), "Max RetryEverySeconds value shall be 43200.");

        }

        [TestMethod]
        public void GetSourceIdOrDefaultTest()
        {
            var invalidOptions = GetDoomyOptions();

            invalidOptions.SourceId = "Source123";
            Assert.AreEqual(invalidOptions.GetSourceIdOrDefault(), "Source123", "All non null SourceId values shall be valid.");
            invalidOptions.SourceId = null;
            Assert.IsNotNull(invalidOptions.GetSourceIdOrDefault(), "Default SourceId value shall be an empty string.");
        }

        [TestMethod]
        public void GetApiMethodUrlTest()
        {
            var invalidOptions = GetDoomyOptions();
            StringAssert.EndsWith(invalidOptions.GetApiMethodUrl("/api/test"), "/api/test", "Joined URL shall has the same ending as API.");
            Assert.IsFalse(invalidOptions.GetApiMethodUrl("/api/test").EndsWith("//api/test"), "Extra slashes shall be removed on URL join.");
        }

    }
}
