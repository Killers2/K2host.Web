/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/
using System;
using Newtonsoft.Json.Linq;
using K2host.Web.Interface;

namespace K2host.Web.Classes
{
   

    public class OApiJsonQueryResponse : IApiJsonQueryResponse
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
