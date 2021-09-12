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
   
    public class OApiJsonQueryResponsePaging
    {

        /// <summary>
        /// The draw counter that this object is a response to - from the draw parameter sent as part of the data request. Note that it is strongly recommended for security reasons that you cast this parameter to an integer, rather than simply echoing back to the client what it sent in the draw parameter, in order to prevent Cross Site Scripting (XSS) attacks.
        /// </summary>
        public long Draw { get; set; }

        /// <summary>
        /// Total records, before filtering (i.e. the total number of records in the database).
        /// </summary>
        public long RecordsTotal { get; set; }

        /// <summary>
        /// Total records, after filtering (i.e. the total number of records after filtering has been applied - not just the number of records being returned for this page of data).
        /// </summary>
        public long RecordsFiltered { get; set; }

        /// <summary>
        /// The data to be displayed in the table. This is an array of data source objects, one for each row, which will be used by DataTables. Note that this parameter's name can be changed using the ajax option's dataSrc property.
        /// </summary>
        public JContainer Data { get; set; }

        /// <summary>
        /// Creates the instance of the OApiJsonQueryResponsePaging
        /// </summary>
        public OApiJsonQueryResponsePaging()
        {
            Draw                = 0;
            RecordsTotal        = 0;
            RecordsFiltered     = 0;
            Data                = null;
        }

    }

}
