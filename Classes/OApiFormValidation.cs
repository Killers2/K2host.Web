/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/
using System;

using gl = K2host.Core.OHelpers;

namespace K2host.Web.Classes
{
    /// <summary>
    /// This class helps build a form validation post requests.
    /// </summary>
    public class OApiFormValidation : IDisposable   
    {

        /// <summary>
        /// The key for the loaded form
        /// </summary>
        public string FormKey { get; }

        /// <summary>
        /// The date of when this key was generated.
        /// </summary>
        public DateTime Created { get; }

        /// <summary>
        /// This indicates the key usage.
        /// </summary>
        public bool HasBeenUsed { get; set; }

        /// <summary>
        /// Creates the instance of the form validation object
        /// </summary>
        public OApiFormValidation()
        {
            FormKey         = gl.UniqueIdent().ToUpper();
            Created         = DateTime.Now;
            HasBeenUsed     = false;
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
