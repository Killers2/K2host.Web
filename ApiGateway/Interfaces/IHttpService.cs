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

    public interface IHttpService : IDisposable
    {
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OHttpResponse<string>> HttpGetAsync(string requestUri, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OHttpResponse<string>> HttpDeleteAsync(string requestUri, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="requestBody"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OHttpResponse<string>> HttpPutAsync(string requestUri, string requestBody, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="requestBody"></param>
        /// <param name="isAnonymous"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<OHttpResponse<string>> HttpPostAsync(string requestUri, string requestBody, bool isAnonymous = false, CancellationToken cancellationToken = default);
        
    }

}
