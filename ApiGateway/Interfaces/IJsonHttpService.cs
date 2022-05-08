/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/

using System;
using System.Threading;
using System.Threading.Tasks;
using K2host.Core.Classes;
using K2host.Web.Classes;

namespace K2host.Web.Interface
{

    public interface IJsonHttpService : IDisposable
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OHttpResponse<T>> HttpGetAsync<T>(string requestUri, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OHttpResponse<T>> HttpDeleteAsync<T>(string requestUri, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OHttpResponse<T>> HttpPostAsync<T>(string requestUri, T model, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OHttpResponse<T>> HttpPostAsync<I, T>(string requestUri, I model, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OHttpResponse<T>> HttpPutAsync<T>(string requestUri, T model, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OHttpResponse<T>> HttpPutAsync<I, T>(string requestUri, I model, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OVirtualFile> HttpDownloadAsync(string requestUri, CancellationToken cancellationToken = default);

    }

}
