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
   
    public class OApiJsonQueryException
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
        /// Creates the instance of the OApiJsonException
        /// </summary>
        public OApiJsonQueryException()
        {
            Message = string.Empty;
        }

        /// <summary>
        /// Returns the string error message from this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Message;
        }

    }

}
