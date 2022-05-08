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
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using K2host.Data;
using K2host.Data.Classes;
using K2host.Data.Enums;
using K2host.Core;
using K2host.Core.Enums;
using K2host.Web.Interface;
using K2host.Web.Classes;

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


        /// <summary>
        /// This is used to try and build the parts from the query for a paged / non paged list from the IDataObject
        /// </summary>
        /// <typeparam name="I"></typeparam>
        /// <param name="query"></param>
        /// <param name="append"></param>
        /// <param name="drawNumber"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageOrder"></param>
        /// <param name="paging"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public static ODataFieldSet[] BuildFilters<I>(this OApiQuery query, ODataFieldSet[] append, out int drawNumber, out int pageNumber, out ODataOrder[] pageOrder, out ODataTakeSkip paging, out ODataCondition[] search)
        {

            drawNumber  = query.Draw;
            pageNumber  = 0;
            pageOrder   = null;
            paging      = null;
            search      = null;

            //If these are in the json query then we can create a paging option.
            //dataTables.Net jquery plugin (Page = draw, Take = length, Skip = start) or page = ((skip + take) / take)
            if (query.Take > 0) 
            {
                paging = new ODataTakeSkip() { Take = query.Take, Skip = query.Skip };
                pageNumber = (query.Skip + query.Take) / query.Take;
            }

            ODataFieldSet[] fields = typeof(I).GetFieldSets(ODataFieldSetType.SELECT);

            //If selected fields option is in the query we remove the unwanted ones. this one take presedence
            if (query.Fields.Length > 0)
                fields = fields.Filter(f => query.Fields.Select(l => l.Name.ToLower()).Contains(f.Column.Name.ToLower()));

            //Based on jquery dataTables.net
            if (query.Fields.Length <= 0 && query.Columns.Length > 0)
                fields = fields.Filter(f => query.Columns.Select(l => l.Name.ToLower()).Contains(f.Column.Name.ToLower()));

            //If in code we are appending then add at this point.
            if (append != null && append.Length > 0)
                fields = fields.Concat(append).ToArray();

            if (query.Order.Length > 0)
            {

                ODataOrder[] pageOrderBuild = Array.Empty<ODataOrder>();

                query.Order.ForEach(o => {

                    int colIndex = o.Column;
                    string colName = string.Empty;
                   
                    try { colName = query.Columns[colIndex].Name; } catch { }

                    if (!string.IsNullOrEmpty(colName) && fields.Where(f => f.Column.Name == colName || f.Alias == colName).Any())
                        try
                        {
                            pageOrderBuild = pageOrderBuild.Append(new ODataOrder()
                            {
                                Column  = fields.Where(f => f.Column.Name == colName || f.Alias == colName).FirstOrDefault().Column,
                                Order   = gl.StringToEnum<ODataOrderType>(o.Dir.ToUpper())
                            }).ToArray();
                        }
                        catch { }

                });

                if (pageOrderBuild.Length > 0)
                    pageOrder = pageOrderBuild;

            }
            else
            {
                var prop = typeof(I).GetProperty("ViewOrder");
                if (prop != null) {
                    pageOrder = new ODataOrder[] {
                        new ODataOrder() {
                            Column  = typeof(I).GetProperty("ViewOrder"),
                            Order   = ODataOrderType.ASC
                        }
                    };
                }

            }

            if (query.Search != null && !string.IsNullOrEmpty(query.Search.Value))
            {
                List<ODataCondition> ToConditions = new();
                fields.Where(f => f.Column.PropertyType == typeof(string)).ToArray().ForEach(f => {
                    ToConditions.Add(new ODataCondition()
                    {
                        Column          = f.Column,
                        Operator        = ODataOperator.LIKE,
                        LikeOperator    = ODataLikeOperator.CONTAINS,
                        Values          = new object[] { query.Search.Value },
                        FollowBy        = ODataFollower.OR
                    });
                });

                ToConditions.First().Container  = ODataConditionContainer.OPEN;
                ToConditions.Last().FollowBy    = ODataFollower.NONE;
                ToConditions.Last().Container   = ODataConditionContainer.CLOSE;
                search = ToConditions.ToArray();
            }

            return fields;

        }


    }
}
