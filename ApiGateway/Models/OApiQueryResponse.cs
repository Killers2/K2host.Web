/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/
using System;
using K2host.Web.Interface;

namespace K2host.Web.Classes
{

    public class OApiQueryResponse<I> : IApiQueryResponse<I>
    {

        /// <summary>
        /// Used to store the string error message if required.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Used to store the string error message if required.
        /// </summary>
        public DateTime DateStamp { get; set; } = DateTime.Now;

        /// <summary>
        /// The draw counter that this object is a response to - from the draw parameter sent as part of the data request. Note that it is strongly recommended for security reasons that you cast this parameter to an integer, rather than simply echoing back to the client what it sent in the draw parameter, in order to prevent Cross Site Scripting (XSS) attacks.
        /// </summary>
        public int Draw { get; set; } = 0;

        /// <summary>
        /// Total records, before filtering (i.e. the total number of records in the database).
        /// </summary>
        public int RecordsTotal { get; set; } = 0;

        /// <summary>
        /// Total records, after filtering (i.e. the total number of records after filtering has been applied - not just the number of records being returned for this page of data).
        /// </summary>
        public int RecordsFiltered { get; set; } = 0;

        /// <summary>
        /// The data to be displayed in the table. This is an array of data source objects, one for each row, which will be used by DataTables. Note that this parameter's name can be changed using the ajax option's dataSrc property.
        /// </summary>
        public I[] Results { get; set; }

        /// <summary>
        /// Creates the instance of the OApiJsonQueryResponsePaging
        /// </summary>
        public OApiQueryResponse()
        {

        }
       
        #region Deconstuctor

        private bool IsDisposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
                if (disposing)
                {


                }
            IsDisposed = true;
        }

        #endregion

    }

}
