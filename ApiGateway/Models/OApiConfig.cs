/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2021-06-15                        | 
'| Use: General                                         |
' \====================================================/
*/

using System;

namespace K2host.Web.Classes
{

    public class OApiConfig : IDisposable
    {

        /// <summary>
        /// Api Url
        /// </summary>
        public string ApiUri { get; set; }

        /// <summary>
        /// The configuration key.
        /// </summary>
        public string ApiConfigKey { get; set; }

        /// <summary>
        /// The configuration key header name.
        /// </summary>
        public string ApiConfigKeyName { get; set; } = "APICONFIGKEY";

        /// <summary>
        /// The configuration header for the content type.
        /// </summary>
        public string ApiContentType { get; set; } = "application/json";

        /// <summary>
        /// 
        /// </summary>
        public OApiConfig()
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
