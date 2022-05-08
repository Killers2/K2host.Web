/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/
using System;

using Newtonsoft.Json.Linq;

namespace K2host.Web.Classes
{
   
    public class OApiQuerySearch
    {

        /// <summary>
        ///
        /// </summary>
        public bool Regex { get; set; }
        
        /// <summary>
        ///
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Creates the instance of the OApiQuerySearch
        /// </summary>
        public OApiQuerySearch()
        {

        }

    }

}
