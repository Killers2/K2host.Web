/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/
using Newtonsoft.Json.Linq;
using System;

namespace K2host.Web.Classes
{
   

    public class OApiJsonQueryResponse
    {

        /// <summary>
        /// Used to store the string error message if required.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Used to store the string error message if required.
        /// </summary>
        public DateTime DateStamp { get; set; }

        /// <summary>
        /// Used to store the responce Json to serialize.
        /// </summary>
        public JContainer Data { get; set; }

        /// <summary>
        /// Creates the instance of the OApiJsonQueryResonse
        /// </summary>
        public OApiJsonQueryResponse()
        {
            Message = string.Empty;
        }

    }

}
