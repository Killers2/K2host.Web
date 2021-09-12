/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/

using System;
using K2host.Web.Enums;

namespace K2host.Web.Interface
{

    /// <summary>
    /// This is used to help create the OAuth2.0 client class you define.
    /// </summary>
    public interface IOAuthClient : IDisposable
    {

        /// <summary>
        /// The API client id given to you at creating an OAuth2 application.
        /// </summary>
        string ClientId { get; set; }

        /// <summary>
        /// The API client secret given to you at creating an OAuth2 application.
        /// </summary>
        string ClientSecret { get; set; }

        /// <summary>
        /// The Redirect URI you will use of your application
        /// </summary>
        Uri CallbackUrl { get; set; }

        /// <summary>
        /// The Url for getting the auth grant type value.
        /// </summary>
        Uri AuthUrl { get; set; }

        /// <summary>
        /// The Url to access the token and request new ones etc..
        /// </summary>
        Uri AccessTokenUrl { get; set; }

        /// <summary>
        /// The Url to revoke the token.
        /// </summary>
        Uri RevokeTokenUrl { get; set; }

        /// <summary>
        /// The auth grant type set
        /// </summary>
        OAuthGrantType GrantType { get; set; }

        /// <summary>
        /// Used to send the auth in the body or as a basic header.
        /// </summary>
        OAuthClientAuthenication ClientAuthentication { get; set; }

    }

}
