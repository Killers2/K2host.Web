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
   
    public class OApiQuery
    {

        /// <summary>
        /// The username of the user trying to login.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// The password of the user trying to login.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// This is the page
        /// </summary>
        public int Draw { get; set; } = 0;

        /// <summary>
        /// This is the take from the the take / skip properties
        /// </summary>
        public int Take { get; set; } = 0;

        /// <summary>
        /// This is the skip from the the take / skip properties
        /// </summary>
        public int Skip { get; set; } = 0;

        /// <summary>
        /// Listed Fields that contain the field name and value for filtering
        /// </summary>
        public OApiQueryField[] Fields { get; set; } = Array.Empty<OApiQueryField>();

        /// <summary>
        /// Listed Fields that contain the field name and value for filtering (Based on jquery dataTables.net)
        /// </summary>
        public OApiQueryColumn[] Columns { get; set; } = Array.Empty<OApiQueryColumn>();

        /// <summary>
        /// Listed orders by column (Based on jquery dataTables.net)
        /// </summary>
        public OApiQueryOrder[] Order { get; set; } = Array.Empty<OApiQueryOrder>();

        /// <summary>
        /// This is the search of the return results (Based on jquery dataTables.net)
        /// </summary>
        public OApiQuerySearch Search { get; set; } = null;

        /// <summary>
        /// Creates the instance of the OApiJsonQueryResonse
        /// </summary>
        public OApiQuery()
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
