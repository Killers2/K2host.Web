/*
' /====================================================\
'| Developed Tony N. Hyde (www.k2host.co.uk)            |
'| Projected Started: 2019-06-01                        | 
'| Use: General                                         |
' \====================================================/
*/
using System;
using System.Security.Claims;

using K2host.Core;
using K2host.Erp.Interfaces.Roles;

using gl = K2host.Core.OHelpers;

namespace K2host.Web
{
    
    public static class OHelpers
    {

        /// <summary>
        /// Used to convert JWT Claim to a User role based on <see cref="IErpUserRole"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <returns></returns>
        public static T ToRole<T>(this Claim e) where T : IErpUserRole 
        {

            T role = Activator.CreateInstance<T>();
            
            role.RoleName       = e.Type;
            role.RoleValue      = e.Value.Substring(0, e.Value.IndexOf(","));
            role.RoleValueType  = e.ValueType;
            role.RoleValueData  = e.Value.StrToByteArray();
            role.RoleClaim      = e;
            role.RolePolicies   = gl.SplitLong(e.Value.Remove(0, e.Value.IndexOf(",") + 1), ',');

            return role;

        }


    }

}
