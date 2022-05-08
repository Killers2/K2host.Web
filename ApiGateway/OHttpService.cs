/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/
using System;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Collections.Generic;

using K2host.Web.Interface;
using K2host.Core.Classes;

namespace K2host.Web.Classes
{
    
    public class OHttpService : IHttpService
    {
        
        /// <summary>
        /// 
        /// </summary>
        private readonly HttpClient _httpClient;
        
        /// <summary>
        /// 
        /// </summary>
        private readonly OApiConfig _apiConfig;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiConfigurationModel"></param>
        public OHttpService(OApiConfig apiConfigurationModel)
        {

            _httpClient = new HttpClient { 
                BaseAddress = new Uri(apiConfigurationModel.ApiUri) 
            };

            _apiConfig = apiConfigurationModel;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OHttpResponse<string>> HttpGetAsync(string requestUri, CancellationToken cancellationToken = default) 
            => await HttpSendAsync(requestUri, HttpMethod.Get, string.Empty, false, cancellationToken);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OHttpResponse<string>> HttpDeleteAsync(string requestUri, CancellationToken cancellationToken = default) 
            => await HttpSendAsync(requestUri, HttpMethod.Delete, string.Empty, false, cancellationToken);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="requestBody"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OHttpResponse<string>> HttpPutAsync(string requestUri, string requestBody, CancellationToken cancellationToken = default)
            => await HttpSendAsync(requestUri, HttpMethod.Put, requestBody, false, cancellationToken);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="requestBody"></param>
        /// <param name="isAnonymous"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OHttpResponse<string>> HttpPostAsync(string requestUri, string requestBody, bool isAnonymous = false, CancellationToken cancellationToken = default)
            => await HttpSendAsync(requestUri, HttpMethod.Post, requestBody, isAnonymous, cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="requestMethod"></param>
        /// <param name="requestBody"></param>
        /// <param name="isAnonymous"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual async Task<OHttpResponse<string>> HttpSendAsync(string requestUri, HttpMethod requestMethod, string requestBody = "", bool isAnonymous = false, CancellationToken cancellationToken = default)
        {

            if (string.IsNullOrEmpty(requestUri))
                throw new ArgumentNullException(nameof(requestUri), "The request uri can not be empty");

            HttpRequestMessage httpReq = new();

            if (!isAnonymous)
                httpReq.Headers.Add(_apiConfig.ApiConfigKeyName, _apiConfig.ApiConfigKey);

            httpReq.Headers.Add("Accept", _apiConfig.ApiContentType);
            httpReq.Method = requestMethod;
            httpReq.RequestUri = new Uri($"{_httpClient.BaseAddress}{requestUri}");

            if (!string.IsNullOrEmpty(requestBody))
                httpReq.Content = new StringContent(requestBody, Encoding.UTF8, _apiConfig.ApiContentType);

            OHttpResponse<string> output = new();

            try
            {

                var httpResp        = await _httpClient.SendAsync(httpReq, cancellationToken);
                output.StatusCode   = httpResp.StatusCode;
                output.IsSuccessful = httpResp.IsSuccessStatusCode;
                output.Model        = await httpResp.Content.ReadAsStringAsync(cancellationToken);

            }
            catch (ArgumentNullException ex)
            {
                output = new OHttpResponse<string>(ex);
            }
            catch (WebException ex)
            {
                output.IsSuccessful = false;
                output.StatusCode = ((HttpWebResponse)ex.Response).StatusCode;
                output.Message = ex.Message;
            }

            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected virtual async Task<OVirtualFile> HttpGetFileAsync(string requestUri, CancellationToken cancellationToken = default)
        {

            if (string.IsNullOrEmpty(requestUri))
                throw new ArgumentNullException(nameof(requestUri), "The request uri can not be empty");

            HttpRequestMessage httpReq = new();

            httpReq.Headers.Add(_apiConfig.ApiConfigKeyName, _apiConfig.ApiConfigKey);
            httpReq.Headers.Add("Accept", _apiConfig.ApiContentType);
            httpReq.Method = HttpMethod.Get;
            httpReq.RequestUri = new Uri($"{_httpClient.BaseAddress}{requestUri}");

            OVirtualFile output = new();

            try
            {

                var httpResp = await _httpClient.SendAsync(httpReq, cancellationToken);

                output.Data = await httpResp.Content.ReadAsByteArrayAsync(cancellationToken);

                var contentDisposition = string.Join(",", httpResp.Headers.GetValues("Content-Disposition"));
                contentDisposition = contentDisposition.Remove(0, contentDisposition.IndexOf("filename=") + "filename=".Length);
                contentDisposition = contentDisposition.Remove(contentDisposition.IndexOf(";"));

                output.FileName = contentDisposition;
                output.MimeType = string.Join(",", httpResp.Headers.GetValues("Content-Type"));

            }
            catch (Exception)
            {
;
            }

            return output;
        }

        #region Deconstuctor

        private bool IsDisposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
                if (disposing)
                {

                    _apiConfig.Dispose();

                }
            IsDisposed = true;
        }

        #endregion

    }

}
