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

    public class OApiResponse<T> : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsSuccessful { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public T Model { get; set; }

        /// <summary>
        ///
        /// </summary>
        public OApiResponse()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        public OApiResponse(T value)
        {
            IsSuccessful = true;
            Model = value;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="error"></param>
        public OApiResponse(string error)
        {
            IsSuccessful = false;
            Message = error;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="ex"></param>
        public OApiResponse(Exception ex)
        {
            IsSuccessful = false;
            Message = $"Exception: {ex.GetType().Name}, {ex.Message}";
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
