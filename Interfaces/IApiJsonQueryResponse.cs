/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/

using System;
using Newtonsoft.Json.Linq;

namespace K2host.Web.Interface
{

    /// <summary>
    /// This is used to help create the ApiJsonQueryResponse client class you define.
    /// </summary>
    public interface IApiJsonQueryResponse : IDisposable
    {


        /// <summary>
        /// Used to contain the data in the repsponse.
        /// </summary>
        JContainer Data { get; set; }

    }

}
