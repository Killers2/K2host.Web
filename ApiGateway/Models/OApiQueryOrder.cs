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
   
    public class OApiQueryOrder
    {

        /// <summary>
        /// The index of the column.
        /// </summary>
        public int Column { get; set; }
       
        /// <summary>
        /// The order direction.
        /// </summary>
        public string Dir { get; set; }

        /// <summary>
        /// Creates the instance of the OApiJsonQueryResonse
        /// </summary>
        public OApiQueryOrder()
        {

        }

    }

}
