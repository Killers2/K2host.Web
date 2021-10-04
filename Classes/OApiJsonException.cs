/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/

using System;

namespace K2host.Web.Classes
{
    /// <summary>
    /// This class helps creates an exception based on the ODataObjects. 
    /// </summary>
    public class OApiJsonException : Exception
    {

        public int StatusCode { get; }

        public OApiJsonException()
        {

        }

        public OApiJsonException(int statuscode, string message)
            : base(message)
        {
            StatusCode = statuscode;
        }

        public OApiJsonException(int statuscode, string message, Exception inner)
            : base(message, inner)
        {
            StatusCode = statuscode;
        }

    }
}
