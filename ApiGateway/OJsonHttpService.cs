/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/
using System;
using System.Threading.Tasks;
using System.Threading;

using Newtonsoft.Json;

using K2host.Web.Interface;
using K2host.Core.Classes;

namespace K2host.Web.Classes
{
    
    public class OJsonHttpService : OHttpService, IJsonHttpService
    {
          
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiConfigurationModel"></param>
        public OJsonHttpService(OApiConfig apiConfigurationModel) 
            : base(apiConfigurationModel)
        {

        }
      
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OHttpResponse<T>> HttpDeleteAsync<T>(string requestUri, CancellationToken cancellationToken = default) 
            => DeserializeResponse<T>(await base.HttpDeleteAsync(requestUri, cancellationToken));
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OHttpResponse<T>> HttpGetAsync<T>(string requestUri, CancellationToken cancellationToken = default) 
            => DeserializeResponse<T>(await base.HttpGetAsync(requestUri, cancellationToken));
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OHttpResponse<T>> HttpPostAsync<T>(string requestUri, T model, CancellationToken cancellationToken = default)
            => DeserializeResponse<T>(await base.HttpPostAsync(requestUri, JsonConvert.SerializeObject(model), false, cancellationToken));
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OHttpResponse<T>> HttpPostAsync<I, T>(string requestUri, I model, CancellationToken cancellationToken = default)
            => DeserializeResponse<T>(await base.HttpPostAsync(requestUri, JsonConvert.SerializeObject(model), false, cancellationToken));
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OHttpResponse<T>> HttpPutAsync<T>(string requestUri, T model, CancellationToken cancellationToken = default)
            => DeserializeResponse<T>(await base.HttpPutAsync(requestUri, JsonConvert.SerializeObject(model), cancellationToken));
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OHttpResponse<T>> HttpPutAsync<I, T>(string requestUri, I model, CancellationToken cancellationToken = default)
            => DeserializeResponse<T>(await base.HttpPutAsync(requestUri, JsonConvert.SerializeObject(model), cancellationToken));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<OVirtualFile> HttpDownloadAsync(string requestUri, CancellationToken cancellationToken = default)
            => await base.HttpGetFileAsync(requestUri, cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializedHttpResponse"></param>
        /// <returns></returns>
        private static OHttpResponse<T> DeserializeResponse<T>(OHttpResponse<string> serializedHttpResponse)
        {

            OHttpResponse<T> httpResponse = new()
            {
                IsSuccessful = serializedHttpResponse.IsSuccessful,
                Message = serializedHttpResponse.Message,
                StatusCode = serializedHttpResponse.StatusCode,
            };

            try
            {

                if (!string.IsNullOrEmpty(serializedHttpResponse.Model))
                {
                    var wrapper = JsonConvert.DeserializeObject<OApiResponse<T>>(serializedHttpResponse.Model);
                    httpResponse.Model = wrapper.Model;
                    httpResponse.IsSuccessful = wrapper.IsSuccessful;
                    httpResponse.Message = wrapper.Message;
                }
            }
            catch (Exception ex)
            {
                httpResponse.Message = ex.Message;
                httpResponse.IsSuccessful = false;
            }

            return httpResponse;
        }

    }

}