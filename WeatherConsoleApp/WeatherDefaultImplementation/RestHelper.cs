using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace WeatherDefaultImplementation
{
    /// <summary>
    /// Helper methods for doing HTTP requests.
    /// </summary>
    internal static class RestHelper
    {

        internal static async Task<string> DownloadJsonAsync(string url, string bearerToken, System.Text.Encoding encoding, 
            int retries, int retryEverySeconds, ILogger logger)
        {

            logger.LogDebug(string.Format("Starting GET request to REST API url: {0}.", url));

            byte[] dataStream = null;

            using (WebClient client = new WebClient())
            {

                client.UseDefaultCredentials = true;
                client.Headers.Add(string.Format("Authorization: bearer {0}", bearerToken));
                client.Headers.Add("Content-Type: application/json");
                client.Headers.Add("Accept: application/json");

                // regular source of issues due to different sec prot support by different providers
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 
                    | SecurityProtocolType.Tls;                

                for (int i = 0; i <= retries; i++)
                {
                    try
                    {
                        dataStream = await client.DownloadDataTaskAsync(url);
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i != retries)
                        {
                            logger.LogWarning(GetRealBaseException(ex), string.Format(
                                "GET request to REST API failed for url: {0}{1}{2}{3}Retrying ({4}) GET request.", 
                                url, Environment.NewLine, GetRealBaseExceptionDescription(ex), Environment.NewLine, i+1));
                            await Task.Delay(retryEverySeconds * 1000);
                        }
                        else
                        {
                            logger.LogError(GetRealBaseException(ex), string.Format(
                                "GET request to REST API failed for url: {0}{1}{2}{3}Retrying failed {4} times.",
                                url, Environment.NewLine, GetRealBaseExceptionDescription(ex), Environment.NewLine, i));
                        }
                    }                    
                }

            }

            logger.LogDebug(string.Format("GET request successful for REST API url: {0}.", url));

            logger.LogDebug(string.Format("Applying text encoding to GET response stream from url:{0}", url));

            string result;
            try
            {
                result = encoding.GetString(dataStream);
            }
            catch (Exception ex)
            {
                var newEx = new Exception(string.Format("Failed to apply encoding {0} to GET response stream from url:{1}", 
                    encoding.EncodingName, ex.Message), ex);
                logger.LogError(newEx, newEx.Message);
                throw newEx;
            }

            logger.LogDebug(string.Format("Text encoding successfuly applied to GET response stream from url:{0}", url));

            return result;
        }

        internal static async Task<string> DownloadJsonByPostAsync(string url, string payload,
            System.Text.Encoding encoding, int retries, int retryEverySeconds, ILogger logger)
        {

            logger.LogDebug(string.Format("Starting POST request to REST API url: {0}", url));

            using (var client = new HttpClient())
            {

                var content = new StringContent(payload, encoding, "application/json");
  
                string result=null;

                for (int i = 0; i <= retries; i++)
                {
                    try
                    {
                        var response = await client.PostAsync(url, content);
                        result = await response.Content.ReadAsStringAsync();
                        break;
                    }
                    catch (Exception ex)
                    {
                        if (i != retries)
                        {
                            logger.LogWarning(GetRealBaseException(ex), string.Format(
                                "GET request to REST API failed for url: {0}{1}{2}{3}Retrying ({4}) GET request.",
                                url, Environment.NewLine, GetRealBaseExceptionDescription(ex), Environment.NewLine, i + 1));
                            await Task.Delay(retryEverySeconds * 1000);
                        }
                        else
                        {
                            logger.LogError(GetRealBaseException(ex), string.Format(
                                "GET request to REST API failed for url: {0}{1}{2}{3}Retrying failed {4} times.",
                                url, Environment.NewLine, GetRealBaseExceptionDescription(ex), Environment.NewLine, i));
                        }
                    }
                }

                logger.LogDebug(string.Format("POST request successful for REST API url: {0}.", url));

                return result;

            }

        }


        private static Exception GetRealBaseException(Exception ex)
        {
            var result = ex;

            if (ex.GetType() == typeof(AggregateException))
            {
                result = ((AggregateException)ex).Flatten();
            }

            if (result.GetType() == typeof(WebException)) return result;

            while (result.InnerException != null)
            {
                result = result.InnerException;
                if (result.GetType() == typeof(WebException)) return result;
            }
            
            return result;

        }

        private static string GetRealBaseExceptionDescription(Exception ex)
        {

            var result = ex;

            if (ex.GetType() == typeof(AggregateException))
            {
                result = ((AggregateException)ex).Flatten();
            }

            if (result.GetType() == typeof(WebException)) return GetWebExceptionDescription((WebException)result);

            while (result.InnerException != null)
            {
                result = result.InnerException;
                if (result.GetType() == typeof(WebException)) return GetWebExceptionDescription((WebException)result);
            }
            
            return string.Format("Failed to fetch data from REST API:{0}", result.Message);

        }

        private static string GetWebExceptionDescription(WebException ex)
        {
            return string.Format("Failed to fetch data from REST API, status={0}, message={1}", ex.Status.ToString(), ex.Message);
        } 

    }

}
