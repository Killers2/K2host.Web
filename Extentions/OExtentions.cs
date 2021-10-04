/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/
using System;
using System.Data;
using System.Linq;
using System.IO;
using System.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using K2host.Core;
using K2host.Core.Classes;
using K2host.Web.Interface;
using K2host.Core.Enums;

using gl = K2host.Core.OHelpers;
using sl = K2host.Sockets.OHelpers;

namespace K2host.Web.Extentions
{
    public static class OExtentions
    {
        
        public static I DnsFromLocal<I>(this I e) where I : ITraceMapperItem
        {

            IPAddress routeIpAddress;

            static string returnHostName(int index, IPAddress hostAddress)
            {
                string hostName;
                string selfDomain = Dns.GetHostEntry("localhost").HostName;

                if (selfDomain.Contains("."))
                    selfDomain = Dns.GetHostEntry("localhost").HostName.Remove(0, Dns.GetHostEntry("localhost").HostName.IndexOf(".") + 1);

                try  {
                    hostName = Dns.GetHostEntry(hostAddress).HostName;
                    if (index > 1 && hostName.Contains(selfDomain))
                            hostName = hostName.Replace("." + selfDomain, string.Empty);
                } catch  {
                    hostName = hostAddress.ToString();
                }

                return hostName;

            };

            if (sl.IsLocalIp(e.IP) && (e.Index == 0 || e.Index == 1)) {
                
                WebClient w = new();
                w.Headers.Add("user-agent", "Core.API/5.0");
                StreamReader r = new(w.OpenRead(new Uri("https://api64.ipify.org/?format=json")));
                routeIpAddress = IPAddress.Parse(JObject.Parse(r.ReadToEnd()).Properties().FirstOrDefault().Value.ToString());
                r.Dispose();
                w.Dispose();

                if (e.Index == 0) {
                    e.IP        = routeIpAddress.ToString();
                    e.HostName  = returnHostName(e.Index, routeIpAddress);
                }

                if (e.Index == 1) {
                    IPHostEntry ipe = Dns.GetHostEntry(returnHostName(e.Index, routeIpAddress));

                    e.IP        = ipe.AddressList[0].ToString();
                    e.HostName  = returnHostName(e.Index, ipe.AddressList[0]);

                    if (ipe.AddressList[0].ToString() == routeIpAddress.ToString())
                        e.PingTime = "None Responsive Router.";
                }

            }
            else 
            { 
                routeIpAddress  = IPAddress.Parse(e.IP);
                e.HostName      = returnHostName(e.Index, routeIpAddress);
            }

            return e;

        }

        public static I GeoIpFromIpStack<I>(this I e, string APIAccessKey) where I : ITraceMapperItem
        {

            WebClient w = new();
            w.Headers.Add("user-agent", "Core.API/5.0");
            StreamReader r = new(w.OpenRead(new Uri("http://api.ipstack.com/" + e.IP + "?access_key=" + APIAccessKey)));
            
            JObject GeoIpData = JObject.Parse(r.ReadToEnd());
            
            r.Dispose();
            w.Dispose();

            e.Longitude     = GeoIpData.Properties().Where(p => p.Name == "longitude").FirstOrDefault().Value.ToString();
            e.Latitude      = GeoIpData.Properties().Where(p => p.Name == "latitude").FirstOrDefault().Value.ToString();
            e.Nmea          = gl.DecimalPositionToDegreesPosition(Convert.ToDouble(e.Longitude), OLongLat.Longitude, ONmeaFormat.WithSigns) + " / " + gl.DecimalPositionToDegreesPosition(Convert.ToDouble(e.Latitude), OLongLat.Latitude, ONmeaFormat.WithSigns);
            e.StateRegion   = GeoIpData.Properties().Where(p => p.Name == "region_name").FirstOrDefault().Value.ToString();
            e.City          = GeoIpData.Properties().Where(p => p.Name == "city").FirstOrDefault().Value.ToString();
            e.Country       = GeoIpData.Properties().Where(p => p.Name == "country_name").FirstOrDefault().Value.ToString();
            e.CountryCode   = GeoIpData.Properties().Where(p => p.Name == "country_code").FirstOrDefault().Value.ToString();
            e.PostalCode    = GeoIpData.Properties().Where(p => p.Name == "zip").FirstOrDefault().Value.ToString();

            return e;

        }

    }
}
