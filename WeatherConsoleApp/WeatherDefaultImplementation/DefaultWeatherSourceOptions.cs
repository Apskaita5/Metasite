using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using WeatherAbstractions;

[assembly: InternalsVisibleTo("WeatherDefaultImplementationTests")]
namespace WeatherDefaultImplementation
{
    /// <summary>
    /// configuration options/settings for <see cref="DefaultWeatherSource"/> service
    /// </summary>
    public sealed class DefaultWeatherSourceOptions
    {

        /// <summary>
        /// Url of the REST API. Required.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// User name to use when authenticating the REST API. Required.
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// User password to use when authenticating the REST API. Required.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Temperature units that the source system uses. Required.
        /// </summary>
        /// <remarks>The implemented source system doesn't provide temperature units.</remarks>
        public TemperatureUnits TemperatureUnit { get; set; }

        /// <summary>
        /// A name of the encoding to use in web request. Not required. Default - UTF8.
        /// Possible values: Unicode, ASCII, BigEndianUnicode, UTF32, UTF8, UTF7.
        /// </summary>
        public string EncodingName { get; set; }

        /// <summary>
        /// Web request retries on web request failure. Not required. Default - 3 retries.
        /// </summary>
        public int? Retries { get; set; }

        /// <summary>
        /// Time span between web request retries on web request failure in seconds. Not required. Default - 10 s. 
        /// </summary>
        public int? RetryEverySeconds { get; set; }

        /// <summary>
        /// An id of the source system. Not required. Default - empty string.
        /// </summary>
        public string SourceId { get; set; }

         
        internal bool IsValid() => !(string.IsNullOrWhiteSpace(Url) || string.IsNullOrWhiteSpace(User)
            || string.IsNullOrWhiteSpace(Password) || !Uri.IsWellFormedUriString(Url, UriKind.Absolute));

        internal string GetErrors()
        {

            var result = new List<string>();

            if (string.IsNullOrWhiteSpace(Url)) result.Add("Url is not specified.");
            // yes I know it's not entirely fool proof check, what slips through the authentication will handle 
            if (!Uri.IsWellFormedUriString(Url, UriKind.Absolute)) result.Add("Url is not valid.");
            if (string.IsNullOrWhiteSpace(User)) result.Add("User is not specified.");
            if (string.IsNullOrWhiteSpace(Password)) result.Add("Password is not specified.");

            if (result.Count < 1) return string.Empty;

            return string.Join(Environment.NewLine, result.ToArray());

        }


        internal System.Text.Encoding GetEncodingOrDefault()
        {
            if (string.IsNullOrWhiteSpace(EncodingName)) return System.Text.Encoding.UTF8;
            if (EncodingName.Trim().Equals("Unicode", StringComparison.OrdinalIgnoreCase)) return System.Text.Encoding.Unicode;
            if (EncodingName.Trim().Equals("ASCII", StringComparison.OrdinalIgnoreCase)) return System.Text.Encoding.ASCII;
            if (EncodingName.Trim().Equals("BigEndianUnicode", StringComparison.OrdinalIgnoreCase)) return System.Text.Encoding.BigEndianUnicode;
            if (EncodingName.Trim().Equals("UTF32", StringComparison.OrdinalIgnoreCase)) return System.Text.Encoding.UTF32;
            if (EncodingName.Trim().Equals("UTF7", StringComparison.OrdinalIgnoreCase)) return System.Text.Encoding.UTF7;
            return System.Text.Encoding.UTF8;
        }

        internal int GetRetriesOrDefault()
        {
            if (!Retries.HasValue || Retries.Value < 1) return 3;
            if (Retries.Value > 1000) return 1000; // sanity check
            return Retries.Value;
        }

        internal int GetRetryEverySecondsOrDefault()
        {
            if (!RetryEverySeconds.HasValue || RetryEverySeconds.Value < 1) return 10;
            if (RetryEverySeconds.Value > 43200) return 43200; //sanity check 12hr
            return RetryEverySeconds.Value;
        }

        internal string GetSourceIdOrDefault() => SourceId?.Trim() ?? string.Empty;


        internal string GetApiMethodUrl(string apiMethod)
        {
            var safeUrl = Url.Trim();
            if (safeUrl.Substring(safeUrl.Length - 1) == "/") safeUrl = safeUrl.Substring(0, safeUrl.Length - 1);
            return safeUrl + apiMethod;
        }        

    }
}
