/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlTypes;

using Newtonsoft.Json.Linq;

using K2host.Core;
using K2host.Web.Interface;

namespace K2host.Web.Classes
{
    /// <summary>
    /// A Generic OAuth2.0 client that helps authenticate with 3rd partie services
    /// </summary>
    public class OAuth20Token : IOAuthToken
    {

        /// <summary>
        /// The responce token type
        /// </summary>
        public string TokenType { get; set; }

        /// <summary>
        /// The access token responce if sucessful type
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// The refresh token responce if sucessful type
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// The access token expiry date responce if sucessful type
        /// </summary>
        public DateTime AccessTokenExpires { get; set; }
       
        /// <summary>
        /// The refresh token expiry date responce if sucessful type
        /// </summary>
        public DateTime RefreshTokenExpires { get; set; }

        /// <summary>
        /// Anyother properties based on the 3rd party service
        /// </summary>
        public IDictionary<string, string> AllProperties { get; set; }

        /// <summary>
        /// Used to create the instance of this class object
        /// </summary>
        public OAuth20Token() 
        {
            AllProperties = new Dictionary<string, string>();
        }

        /// <summary>
        /// Used to create the instance of this class object
        /// </summary>
        /// <param name="e">The response from the request to parse.</param>
        public OAuth20Token(string e)
            : this()
        {
            JObject response = JObject.Parse(e);

            response.Properties().ForEach(p => {
                AllProperties.Add(p.Name.ToLower(), p.Value.ToString());
            });

            try {
                AccessToken         = response.Properties().Where(p => p.Name == "access_token").FirstOrDefault().Value.ToString();
                RefreshToken        = response.Properties().Where(p => p.Name == "refresh_token").FirstOrDefault().Value.ToString();
                TokenType           = response.Properties().Where(p => p.Name == "token_type").FirstOrDefault().Value.ToString();
                AccessTokenExpires  = DateTime.Now.AddSeconds(Convert.ToInt32(response.Properties().Where(p => p.Name == "expires_in").FirstOrDefault().Value.ToString()));
            } catch { }

            try {
                JProperty b = response.Properties().Where(p => p.Name.Contains("refresh") && p.Name.Contains("expire")).FirstOrDefault();
                if (b != null) { RefreshTokenExpires = DateTime.Now.AddSeconds(Convert.ToInt32(b.Value.ToString())); }
            } catch {
                RefreshTokenExpires = (DateTime)SqlDateTime.MinValue;
            }

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
