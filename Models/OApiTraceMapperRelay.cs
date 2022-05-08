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

using Newtonsoft.Json.Linq;

using K2host.Core;
using K2host.Web.Interface;

namespace K2host.Web.Classes
{
    /// <summary>
    /// A Generic OAuth2.0 client that helps authenticate with 3rd partie services
    /// </summary>
    public class OApiTraceMapperRelay : ITraceMapperItem
    {

        /// <summary>
        /// 
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PingTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PingStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DistanceE { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DistanceF { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Nmea { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string StateRegion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MetroCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IspInfo { get; set; }

        /// <summary>
        /// Used to create the instance of this class object
        /// </summary>
        public OApiTraceMapperRelay() 
        {
            Index           = 0;
            IP              = string.Empty;
            HostName        = string.Empty;
            PingTime        = string.Empty;
            PingStatus      = string.Empty;
            DistanceE       = string.Empty;
            DistanceF       = string.Empty;
            Nmea            = string.Empty;
            Country         = string.Empty;
            CountryCode     = string.Empty;
            StateRegion     = string.Empty;
            City            = string.Empty;
            PostalCode      = string.Empty;
            Latitude        = string.Empty;
            Longitude       = string.Empty;
            MetroCode       = string.Empty;
            AreaCode        = string.Empty;
            IspInfo         = string.Empty;
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
