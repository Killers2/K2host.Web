/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net;

using K2host.Core;
using K2host.Web.Interface;
using K2host.Web.Delegates;
using K2host.Web.Extentions;

namespace K2host.Web.Classes
{
    /// <summary>
    /// This class helps build a traceRT request and a list of end points.
    /// </summary>
    /// <typeparam name="I"></typeparam>
    public class OApiTraceMapperQuery<I> : IDisposable where I : ITraceMapperItem     
    {

        /// <summary>
        /// The max hops the instance will loop though unless the end is successful
        /// </summary>
        public int MaxHops { get; set; }
       
        /// <summary>
        /// The timeout in ms for the latency of a ping.
        /// </summary>
        public int Timeout { get; set; }
        
        /// <summary>
        /// This will allow the list to include timed out hops.
        /// </summary>
        public bool IncludeTimeoutRoutes { get; set; }

        /// <summary>
        /// The host name of the server you want to query
        /// </summary>
        public string HostName { get; private set; }
       
        /// <summary>
        /// The list of routes built from the service.
        /// </summary>
        public I[] Routes { get; private set; }
        
        /// <summary>
        /// This event is triggered on completeing the trace.
        /// This is for a threaded operation.
        /// </summary>
        public OnTraceMapperComplete<I> OnComplete { get; set; }

        /// <summary>
        /// This will trigger on the intial desination ping test if times out.
        /// This is for a threaded operation.
        /// </summary>
        public OnTraceMapperError<I> OnError { get; set; }

        /// <summary>
        /// This will trigger ever time a valid and successful route is found.
        /// This is for a threaded operation.
        /// </summary>
        public OnTraceMapperRouteFound<I> OnRouteFound { get; set; }

        /// <summary>
        /// A callback for Processing the hostname data
        /// </summary>
        public OnTraceMapperRouteProcess<I> OnProcessDns { get; set; }

        /// <summary>
        /// A callback for processing the geoip data
        /// </summary>
        public OnTraceMapperRouteProcess<I> OnProcessGeoIp { get; set; }

        /// <summary>
        /// This is set to true when the operation is complete
        /// </summary>
        bool Complete { get; set; }
        
        /// <summary>
        /// This is the random buffer sent in the request of the ping.
        /// </summary>
        byte[] Buffer { get; set; }

        /// <summary>
        /// The desination ip address of the request.
        /// </summary>
        IPAddress Destination { get; set; }

        /// <summary>
        /// Creates the instance of the query object
        /// </summary>
        public OApiTraceMapperQuery(string hostName)
        {
            Routes                  = Array.Empty<I>();
            MaxHops                 = 30;
            Timeout                 = 1000;
            IncludeTimeoutRoutes    = true;
            Complete                = false;
            HostName                = hostName; 
            Buffer                  = Enumerable.Repeat((byte)101, 32).ToArray();

        }

        /// <summary>
        /// Used to start the operation.
        /// </summary>
        public void Start() 
        {

            Complete = false;
            
            Destination = Dns.GetHostEntry(HostName).AddressList[0];

            Ping        destinationTest     = new();
            PingReply   destinationReply    = destinationTest.Send(Destination, Timeout);

            if (destinationReply.Status == IPStatus.TimedOut)
                OnError?.Invoke(this, "The destination has timed out or is unreachable.");

            destinationTest.Dispose();

            Ping        trace           = new();
            PingOptions traceOptions    = new(1, true);

            while (traceOptions.Ttl <= MaxHops) 
            {

                if(Complete)
                    break;

                PingReply traceReply = trace.Send(Destination, Timeout, Buffer, traceOptions);

                if (traceReply.Status == IPStatus.Success || traceReply.Status == IPStatus.TtlExpired)
                {

                    Ping        latency         = new();
                    PingReply   latencyReply    = latency.Send(traceReply.Address, Timeout);

                    I result = AddNewItem(this, traceReply.Address, latencyReply.RoundtripTime, traceReply.Status);

                    OnRouteFound?.Invoke(this, result, Complete);

                    latency.Dispose();

                }
                else {
                    if (IncludeTimeoutRoutes)
                        AddNewItem(this, IPAddress.Any, 0, IPStatus.Unknown);
                }

                if ((traceReply.Status == IPStatus.Success || traceReply.Status == IPStatus.TtlExpired) && traceReply.Address.Equals(Destination))
                {
                    OnComplete?.Invoke(this);
                    Complete = true;
                    break;
                }

                traceOptions.Ttl++;

            }
          
            if (!Complete) 
            {
                Complete = true;
                I result = AddNewItem(this, Destination, destinationReply.RoundtripTime, destinationReply.Status);
                OnRouteFound?.Invoke(this, result, Complete);
                OnComplete?.Invoke(this);
            }

            trace.Dispose();

        }

        /// <summary>
        /// Used to stop a started start the operation.
        /// </summary>
        public void Stop() 
        {
            Complete = true;
        }
        
        /// <summary>
        /// Used to add an item to the routes list.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="roundTrip"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        I AddNewItem(OApiTraceMapperQuery<I> sender, IPAddress address, long roundTrip, IPStatus status) {

            I output            = Activator.CreateInstance<I>();
            output.Index        = Routes.Length;
            output.IP           = address.ToString();
            output.PingTime     = roundTrip.ToString();
            output.PingStatus   = status.ToString();

            OnProcessDns?.Invoke(sender, output);

            //Done seperatly incase of any code between thats needed.
            OnProcessGeoIp?.Invoke(sender, output);

            Routes = Routes.Append(output).ToArray();

            return output;
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
                    Routes.Dispose(out _);
                }
            IsDisposed = true;
        }

        #endregion

    }

}
