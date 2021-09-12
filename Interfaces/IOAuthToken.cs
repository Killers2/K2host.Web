/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/

using System;
using System.Collections.Generic;

namespace K2host.Web.Interface
{

    /// <summary>
    /// This is used to help create the OAuth2.0 token class you define.
    /// </summary>
    public interface IOAuthToken : IDisposable
    {
        
        /// <summary>
        /// The responce token type
        /// </summary>
        string TokenType { get; set; }

        /// <summary>
        /// The access token responce if sucessful type
        /// </summary>
        string AccessToken { get; set; }

        /// <summary>
        /// The refresh token responce if sucessful type
        /// </summary>
        string RefreshToken { get; set; }

        /// <summary>
        /// The access token expiry date responce if sucessful type
        /// </summary>
        DateTime AccessTokenExpires { get; set; }

        /// <summary>
        /// The refresh token expiry date responce if sucessful type
        /// </summary>
        DateTime RefreshTokenExpires { get; set; }

        /// <summary>
        /// Anyother properties based on the 3rd party service
        /// </summary>
        IDictionary<string, string> AllProperties { get; set; }

    }

}
