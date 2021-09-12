/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

using K2host.Core;
using K2host.Web.Delegates;
using K2host.Web.Enums;
using K2host.Web.Interface;

using gl = K2host.Core.OHelpers;

namespace K2host.Web.Classes
{

    /// <summary>
    /// A Generic OAuth2.0 client that helps authenticate with 3rd partie services
    /// </summary>
    public class OAuth20 : IOAuthClient
    {
        
        /// <summary>
        /// The API client id given to you at creating an OAuth2 application.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// The API client secret given to you at creating an OAuth2 application.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The Redirect URI you will use of your application
        /// </summary>
        public Uri CallbackUrl { get; set; }

        /// <summary>
        /// The Url for getting the auth grant type value.
        /// </summary>
        public Uri AuthUrl { get; set; }

        /// <summary>
        /// The Url to access the token and request new ones etc..
        /// </summary>
        public Uri AccessTokenUrl { get; set; }
       
        /// <summary>
        /// The Url to revoke the token.
        /// </summary>
        public Uri RevokeTokenUrl { get; set; }

        /// <summary>
        /// The auth grant type set
        /// NOT USED AT THE MO
        /// </summary>
        public OAuthGrantType GrantType { get; set; }

        /// <summary>
        /// Used to send the auth in the body or as a basic header.
        /// NOT USED AT THE MO
        /// </summary>
        public OAuthClientAuthenication ClientAuthentication { get; set; }

        /// <summary>
        /// Used to create the instance of this class object
        /// </summary>
        public OAuth20() { }

        /// <summary>
        /// Used to generate the url for authenicating on the service.
        /// </summary>
        /// <param name="state">The unique id for validting a response.</param>
        /// <param name="scopes">Scopes are applcation based</param>
        /// <param name="addParms">A callback used to add additional url parameters.</param>
        /// <returns>The Generated Url.</returns>
        public string GetAuthorizationURL(string state, string scopes, OnUrlParms addParms)
        {
            StringBuilder output = new();

            output.Append(AuthUrl.OriginalString);
            output.Append("?response_type=code");
            output.Append("&client_id=" + WebUtility.UrlEncode(ClientId));
            output.Append("&redirect_uri=" + WebUtility.UrlEncode(CallbackUrl.OriginalString));
            output.Append("&scope=" + Uri.EscapeUriString(scopes));
            output.Append("&state=" + WebUtility.UrlEncode(state));

            if(addParms != null)
                addParms.Invoke().ForEach(kvp => {
                    output.Append("&" + kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value));
                });

            return output.ToString();
        }

        /// <summary>
        /// Used to return an instance based on the <see cref="IOAuthToken"/> interface.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="code"></param>
        /// <param name="addParms">A callback used to add additional url parameters.</param>
        /// <returns></returns>
        public T RequestAccessToken<T>(string code, OnUrlParms addParms) where T : IOAuthToken
        {

            StringBuilder body = new();

            body.Append("grant_type=authorization_code");
            body.Append("&code=" + WebUtility.UrlEncode(code));
            body.Append("&redirect_uri=" + WebUtility.UrlEncode(CallbackUrl.OriginalString));

            if (ClientAuthentication == OAuthClientAuthenication.SendInBody)
            {
                body.Append("&client_id=" + WebUtility.UrlEncode(ClientId));
                body.Append("&client_secret=" + WebUtility.UrlEncode(ClientSecret));
            }

            if (addParms != null)
                addParms.Invoke().ForEach(kvp => {
                    body.Append("&" + kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value));
                });

            HttpWebRequest wr = WebRequest.CreateHttp(AccessTokenUrl.OriginalString);

            wr.ContentType  = "application/x-www-form-urlencoded";
            wr.Method       = "POST";
            wr.UserAgent    = "K2host.Web/OAuth2.0";
            wr.Accept       = "*/*";
           
            if (ClientAuthentication == OAuthClientAuthenication.SendAsAuthHeader)
                wr.Headers.Add("Authorization", "Basic " + gl.EncryptB64(ClientId + ":" + ClientSecret));

            byte[] input = Encoding.UTF8.GetBytes(body.ToString());

            Stream sw = wr.GetRequestStream();
            sw.Write(input, 0, input.Length);

            HttpWebResponse wp = (HttpWebResponse)wr?.GetResponse();
            Stream sr = wp?.GetResponseStream();

            MemoryStream response = new();

            sr.CopyTo(response);

            string output = Encoding.UTF8.GetString(response.ToArray());

            response.Flush();
            response.Close();
            response.Dispose();

            return (T)Activator.CreateInstance(typeof(T), output);
        }

        /// <summary>
        /// Used to renew an access token from an old one that has expired.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="addParms">A callback used to add additional url parameters.</param>
        /// <returns></returns>
        public T RefreshAccessToken<T>(IOAuthToken e, OnUrlParms addParms) where T : IOAuthToken
        {

            StringBuilder body = new();

            body.Append("grant_type=refresh_token");
            body.Append("&refresh_token=" + WebUtility.UrlEncode(e.RefreshToken));

            if (ClientAuthentication == OAuthClientAuthenication.SendInBody)
            {
                body.Append("&client_id=" + WebUtility.UrlEncode(ClientId));
                body.Append("&client_secret=" + WebUtility.UrlEncode(ClientSecret));
            }

            if (addParms != null)
                addParms.Invoke().ForEach(kvp => {
                    body.Append("&" + kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value));
                });

            HttpWebRequest wr = WebRequest.CreateHttp(AccessTokenUrl.OriginalString);

            wr.ContentType  = "application/x-www-form-urlencoded";
            wr.Method       = "POST";
            wr.UserAgent    = "K2host.Web/OAuth2.0";
            wr.Accept       = "*/*";
            
            if (ClientAuthentication == OAuthClientAuthenication.SendAsAuthHeader)
                wr.Headers.Add("Authorization", "Basic " + gl.EncryptB64(ClientId + ":" + ClientSecret));

            byte[] input = Encoding.ASCII.GetBytes(body.ToString());

            Stream sw = wr.GetRequestStream();
            sw.Write(input, 0, input.Length);

            HttpWebResponse wp = (HttpWebResponse)wr?.GetResponse();
            Stream sr = wp?.GetResponseStream();

            MemoryStream response = new();

            sr.CopyTo(response);

            string output = Encoding.UTF8.GetString(response.ToArray());

            response.Flush();
            response.Close();
            response.Dispose();

            return (T)Activator.CreateInstance(typeof(T), output);

        }

        /// <summary>
        /// Used to revoke an access token from a current access token item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="addParms">A callback used to add additional url parameters.</param>
        /// <returns></returns>
        public T RevokeAccessToken<T>(IOAuthToken e, OnUrlParms addParms, bool isJson = false) where T : IOAuthToken
        {

            StringBuilder body = new();

            if (isJson)
            {
                body.Append('{');
                body.Append("\"token\"=\"" + e.AccessToken + "\"");

                if (addParms != null)
                    addParms.Invoke().ForEach(kvp => {
                        body.Append(", \"" + kvp.Key + "\"=\"" + WebUtility.UrlEncode(kvp.Value) + "\"");
                    });

                body.Append('}');

            }
            else
            {
                body.Append("token=" + e.AccessToken);

                if (addParms != null)
                    addParms.Invoke().ForEach(kvp => {
                        body.Append("&" + kvp.Key + "=" + WebUtility.UrlEncode(kvp.Value));
                    });
            }

            HttpWebRequest wr = WebRequest.CreateHttp(RevokeTokenUrl.OriginalString);

            wr.ContentType  = "application/x-www-form-urlencoded";
            wr.Method       = "POST";
            wr.UserAgent    = "K2host.Web/OAuth2.0";
            wr.Accept       = "*/*";
            
            if (ClientAuthentication == OAuthClientAuthenication.SendAsAuthHeader)
                wr.Headers.Add("Authorization", "Basic " + gl.EncryptB64(ClientId + ":" + ClientSecret));

            byte[] input = Encoding.ASCII.GetBytes(body.ToString());

            Stream sw = wr.GetRequestStream();
            sw.Write(input, 0, input.Length);

            HttpWebResponse wp = (HttpWebResponse)wr?.GetResponse();
            Stream sr = wp?.GetResponseStream();

            MemoryStream response = new();

            sr.CopyTo(response);

            string output = Encoding.UTF8.GetString(response.ToArray());

            response.Flush();
            response.Close();
            response.Dispose();

            return (T)Activator.CreateInstance(typeof(T), output);

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
