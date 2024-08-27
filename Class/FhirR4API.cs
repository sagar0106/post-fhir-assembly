using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PostDnFhirR4Api.Class
{
    public class FhirR4API
    {
        private static readonly HttpClient _apiClient = new HttpClient();
        private static readonly SemaphoreSlim _cachedTokensAsyncLock = new SemaphoreSlim(1, 1);
        private static List<CacheToken> _accessTokens = new List<CacheToken>();

        private string _LOGIN_ENDPOINT;

        private Auth Authorization { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="eventTarget"></param>
        public FhirR4API(Auth auth)
        {
            Authorization = auth;
        }

        /// <summary>
        /// Logs in to FHIR.
        /// </summary>
        /// <returns>A CacheToken object containing the FHIR access token.</returns>
        public async Task<CacheToken> Login()
        {
            _LOGIN_ENDPOINT = new Uri(Authorization.AuthorizationUrl).ToString();

            CacheToken token = _accessTokens.FirstOrDefault(x => x.ClientID == Authorization.ClientID && x.ClientSecret == Authorization.ClientSecret);

            if (token != null)
            {
                return token;
            }
            else
            {
                await _cachedTokensAsyncLock.WaitAsync();

                try
                {
                    token = _accessTokens.FirstOrDefault(x => x.ClientID == Authorization.ClientID && x.ClientSecret == Authorization.ClientSecret);

                    if (token != null)
                    {
                        return token;
                    }
                    else
                    {
                        token = await GetNewAccessToken().ConfigureAwait(false);
                        _accessTokens.Add(token);
                        return token;
                    }
                }
                finally
                {
                    _cachedTokensAsyncLock.Release();
                }
            }
        }

        /// <summary>
        /// Removes authentication access token to allow a new one to be retrieved.
        /// </summary>
        public async Task ExpireToken()
        {
            await _cachedTokensAsyncLock.WaitAsync();

            try
            {
                _accessTokens.RemoveAll(x => x.ClientID == Authorization.ClientID && x.ClientSecret == Authorization.ClientSecret);
            }
            finally
            {
                _cachedTokensAsyncLock.Release();
            }
        }

        /// <summary>
        /// POST Record
        /// </summary>
        /// <param name="postMessage"></param>
        /// <param name="recordType"></param>
        /// <returns></returns>
        public async Task<ExpandoObject> CreateRecord(string postMessage, string recordType, string baseUrl, string siteServiceKey)
        {
            string uri = $"{baseUrl}/{recordType}";
            string responseString = String.Empty;
            dynamic result = new ExpandoObject();

            using (HttpContent contentUpdate = new StringContent(postMessage, Encoding.UTF8, "application/json"))
            {
                using (var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = contentUpdate })
                {
                    CacheToken token = await Login().ConfigureAwait(false);
                    request.Headers.Add("Authorization", "Bearer " + token.Token);
                    request.Headers.Add("SiteServiceKey", siteServiceKey);
                    using (HttpResponseMessage response = await SendRequest(request).ConfigureAwait(false))
                    {
                        responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        Console.WriteLine($"HTTP Response Body: {responseString}");
                        result.responseString = responseString;
                        result.httpStatus = (int)response.StatusCode + "-" + response.StatusCode;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// GET Record
        /// </summary>
        /// <param name="url"></param>
        /// <param name="siteServiceKey"></param>
        /// <returns></returns>
        public async Task<ExpandoObject> GetRecord(string url, string siteServiceKey)
        {
            string responseString = String.Empty;
            dynamic result = new ExpandoObject();

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                CacheToken token = await Login().ConfigureAwait(false);
                request.Headers.Add("Authorization", "Bearer " + token.Token);
                request.Headers.Add("SiteServiceKey", siteServiceKey);
                using (HttpResponseMessage response = await SendRequest(request).ConfigureAwait(false))
                {
                    responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    Console.WriteLine($"HTTP Response Body: {responseString}");
                    result.responseString = responseString;
                    result.httpStatus = (int)response.StatusCode + "-" + response.StatusCode;
                }
            }

            return result;
        }

        /// <summary>
        /// Gets a new access token from ClientAPI.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <returns>A new access token.</returns>
        /// <exception cref="APIException">Thrown when API returns a non-success HTTP status.</exception>
        private async Task<CacheToken> GetNewAccessToken()
        {
            HttpResponseMessage response = null;

            try
            {
                if (_apiClient == null)
                {
                    throw new Exception("HttpClient is null. Please check assembly logs."); // We should be reusing HttpClient wherever possible.
                }

                Console.WriteLine("Started process for fetching FHIR access token.");

                using (var request = new HttpRequestMessage(HttpMethod.Post, _LOGIN_ENDPOINT))
                {
                    var formData = new Dictionary<string, string>()
                    {
                        {"grant_type", "client_credentials"},
                        {"client_id", Authorization.ClientID},
                        {"client_secret", Authorization.ClientSecret},
                    };

                    using (var requestParams = new FormUrlEncodedContent(formData))
                    {
                        request.Content = requestParams;
                        response = await SendRequest(request).ConfigureAwait(false);

                        JObject tokenObj = JObject.Parse(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                        string access_token = (string)tokenObj["access_token"];

                        if (String.IsNullOrWhiteSpace(access_token))
                        {
                            throw new CustomAPIException($"Field 'access_token' is required.", response);
                        }
                        else
                        {
                            response.Dispose();

                            return new CacheToken()
                            {
                                ClientID = Authorization.ClientID,
                                ClientSecret = Authorization.ClientSecret,
                                Token = access_token
                            };
                        }
                    }
                }
            }
            catch (CustomAPIException)
            {
                // Don't dispose HttpResponseMessage here for APIExceptions as it is needed to evaluate for transient-errors. Disposal is done after evaluating response. 
                throw;
            }
            catch (Exception)
            {
                // Make sure to dispose of HttpResponseMessage for all other exceptions.
                response?.Dispose();
                throw;
            }
        }

        internal async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request)
        {
            Console.WriteLine($"HTTP {request.Method}: {request.RequestUri}");

            var postingStopWatch = new Stopwatch();
            postingStopWatch.Start();
            HttpResponseMessage response = await _apiClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
            postingStopWatch.Stop();

            Console.WriteLine($"Request elapsed time in milliseconds: {postingStopWatch.ElapsedMilliseconds}");

            string responseString = $"HTTP Status: {(int)response.StatusCode}-{response.ReasonPhrase}";

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(responseString);
                return response;
            }
            else
            {
                responseString += $", HTTP Response Body: {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}";
                throw new CustomAPIException(responseString, response);
            }
        }
    }
}
