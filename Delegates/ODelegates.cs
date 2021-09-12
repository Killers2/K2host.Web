/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/

using System.Collections.Generic;

using K2host.Web.Classes;
using K2host.Web.Interface;

namespace K2host.Web.Delegates
{

    public delegate Dictionary<string, string> OnUrlParms();

    public delegate void OnTraceMapperComplete<I>(OApiTraceMapperQuery<I> sender) where I : ITraceMapperItem;
   
    public delegate void OnTraceMapperError<I>(OApiTraceMapperQuery<I> sender, string error) where I : ITraceMapperItem;

    public delegate void OnTraceMapperRouteFound<I>(OApiTraceMapperQuery<I> sender, I e, bool complete) where I : ITraceMapperItem;

    public delegate void OnTraceMapperRouteProcess<I>(OApiTraceMapperQuery<I> sender, I e) where I : ITraceMapperItem;

}
