/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/

using System;

namespace K2host.Web.Interface
{

    /// <summary>
    /// This is used to help create the trace mapper item class you define.
    /// </summary>
    public interface ITraceMapperItem : IDisposable
    {

        /// <summary>
        /// 
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string IP { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string HostName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string PingTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string PingStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string DistanceE { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        string DistanceF { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        string Nmea { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        string Country { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string CountryCode { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        string StateRegion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string City { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string PostalCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Latitude { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string Longitude { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        string MetroCode { get; set; }
       
        /// <summary>
        /// 
        /// </summary>
        string AreaCode { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        string IspInfo { get; set; }

    }

}
