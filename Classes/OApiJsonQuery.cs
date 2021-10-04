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
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Data;
using System.Data.SqlTypes;
using System.Security.Claims;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using K2host.Core;
using K2host.Core.Classes;
using K2host.Console.Classes;
using K2host.Data;
using K2host.Data.Enums;
using K2host.Data.Classes;
using K2host.Erp.Interfaces.Users;
using K2host.Erp.Interfaces.Roles;

using gl = K2host.Core.OHelpers;

namespace K2host.Web.Classes
{

    public class OApiJsonQuery : IDisposable
    {

        /// <summary>
        /// Used to define a json path in sql using the ODataSelectQuery Generator.
        /// </summary>
        [JsonIgnore]
        public static readonly string ODataJsonPathDefault = "data";

        /// <summary>
        /// Used to store the incomming command
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Used to store the data array from the object
        /// </summary>
        public Dictionary<string, string> Data { get; set; }

        /// <summary>
        /// Used to store anonymous data as string from the object request.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Used to store the object json string data that can be attached to the object.
        /// </summary>
        public Dictionary<string, JContainer> JsonObjects { get; set; }

        /// <summary>
        /// Used to store the request command from the json part.
        /// </summary>
        public List<string> CommandStack { get; set; }

        /// <summary>
        /// Used to store the ODataObject as a user on validation making the request.
        /// </summary>
        [JsonIgnore]
        public IErpUser User { get; set; }

        /// <summary>
        /// Used to store the ODataObject as a user role on the request.
        /// </summary>
        [JsonIgnore]
        public IErpUserRole[] UserRoles { get; set; }

        /// <summary>
        /// Used to store the Key used to validate uses on authentication, the system will be dependent on this key.
        /// </summary>
        [JsonIgnore]
        public string GlobalApplicationKey { get; private set; }
        
        /// <summary>
        /// The api command start stack or prefix
        /// </summary>
        public string ApiCommandPrefix { get; set; }

        /// <summary>
        /// Used to get the commands from the header of any api request using this format.
        /// </summary>
        [JsonIgnore]
        public OCommandParser CommandParser { get; private set; }

        /// <summary>
        /// This will hold all Users in the system referenced from memory.
        /// Uid, IErpUser
        /// </summary>
        [JsonIgnore]
        public Dictionary<long, IErpUser> AllUsers { get; set; }

        /// <summary>
        /// Creates the instance of the query object
        /// </summary>
        public OApiJsonQuery(string apiCommandPrefix, string globalApplicationKey)
        {
            ApiCommandPrefix        = apiCommandPrefix;
            GlobalApplicationKey    = globalApplicationKey;
            Data                    = new Dictionary<string, string>();
            JsonObjects             = new Dictionary<string, JContainer>();
            AllUsers                = new Dictionary<long, IErpUser>();
            CommandParser           = new OCommandParser();
        }

        /// <summary>
        /// Used as a helper for loggin on a <see cref="IErpUser"/> on the auth command.
        /// Where T is the type used and interfaced with IErpUser
        /// </summary>
        /// <param name="erpUsers">Optional the users the system has stored</param>
        /// <param name="connection"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public T UserLogon<T>(out string error) where T : IErpUser
        {

            error = string.Empty;

            if (!Data.ContainsKey("username"))
            {
                error = "The username credentials supplied can not be found in the request.";
                return default;
            }

            try
            {

                string Username = Data["username"];
                string Password = Data["password"];

                string AesHash  = gl.EncryptAes(
                    Password, 
                    GlobalApplicationKey, 
                    Encoding.Unicode.GetBytes(Username + ":" + Password)
                );


                IErpUser    u           = null;
                Exception   UserError   = null;

                try
                {
                    u = AllUsers.Values
                        .Where(u => u.AesHash == AesHash)
                        .FirstOrDefault();
                }
                catch (Exception ex) { 
                    UserError = ex; 
                }

                if (u == null)
                {
                    error = "The user credentials supplied can not be found on this system. " + UserError?.Message;
                    return default;
                }

                if (u.Username != Username)
                {
                    error = "The username does not match the user details of the credentials used (mismatch).";
                    u.Dispose();
                    return default;
                }

                User = (T)u;

                return (T)u;

            }
            catch (Exception ex)
            {
                error = ex.Message;
                return default;
            }

        }

        /// <summary>
        /// Used as a helper to validate a <see cref="IErpUser"/> when a request it made.
        /// Where T is the type used and interfaced with IErpUser
        /// </summary>
        /// <param name="erpUsers">Optional the users the system has stored</param>
        /// <param name="connection"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public T ValidateUserToken<T>(out string error, bool withUser = false) where T : IErpUser
        {

            error = string.Empty;

            if (!Data.ContainsKey("authtoken") || !Data.ContainsKey("authuser"))
            {
                error = "The authentication token / user was not found in the request please try again.";
                return default;
            }

            try
            {

                string AuthenticationToken      = Data["authtoken"];
                string AuthenticationUsername   = Data["authuser"];

                //Decrypt the AuthenticationToken
                AuthenticationToken = gl.DecryptAes(
                    AuthenticationToken,
                    GlobalApplicationKey,
                    Encoding.Unicode.GetBytes(
                        GlobalApplicationKey
                    )
                );

                IErpUser u = null;

                if (withUser) 
                {

                    Exception UserError = null;

                    try
                    {
                        u = AllUsers.Values
                            .Where(u => u.Username == AuthenticationUsername)
                            .FirstOrDefault();
                    }
                    catch (Exception ex)  {
                        UserError = ex;
                    }
 
                    if (u == null)
                    {
                        error = "The authentication token supplied is not valid: user doesn't exist. " + UserError.Message;
                        return default;
                    }

                    if (u.Jtoken != AuthenticationToken)
                    {
                        error = "The authentication token supplied is not valid: please login.";
                        u.Dispose();
                        return default;
                    }

                    if (u.JtokenExpiry < DateTime.Now)
                    {
                        error = "The authentication token supplied is not valid: it is out of date.";
                        u.Jtoken = string.Empty;
                        u.JtokenExpiry = DateTime.MinValue;
                        u.Save();
                        u.Dispose();
                        return default;
                    }

                    if (u.Locked)
                    {
                        if (u.LockedUntil > DateTime.Now)
                        {
                            error = "The authentication token supplied is not valid: it is out of date.";
                            u.Jtoken = string.Empty;
                            u.JtokenExpiry = DateTime.MinValue;
                            u.Save();
                            u.Dispose();
                            return default;
                        }

                        u.LockedUntil = (DateTime)SqlDateTime.MinValue;
                        u.Save(true);
                    }

                }

                List<Claim> Claims          = gl.JWTValidateTokenGetClaims(AuthenticationToken, gl.EncryptB64(GlobalApplicationKey));
                Claim ValidateExpiration    = Claims.Find(e => { return e.Type == ClaimTypes.Expiration; });
                Claim ValidateClaim         = Claims.Find(e => { return e.Type == ClaimTypes.Name; });

                if (gl.JWTJSONToDateTime(ValidateExpiration.Value) < DateTime.Now)
                {
                    if (u != null)
                    {
                        u.Jtoken        = string.Empty;
                        u.JtokenExpiry  = DateTime.MinValue;
                        u.Save().Dispose();
                    }
                    error = "The authentication token supplied is not valid: it is out of date.";
                    return default;
                }

                if (ValidateClaim.Value != AuthenticationUsername)
                {
                    u?.Dispose();
                    error = "The authentication token supplied is not valid: the token does not match the user details of the credentials used (mismatch).";
                    return default;
                }

                Claims.Clear();

                error = string.Empty;

                return (T)u;

            }
            catch (Exception ex)
            {
                error = ex.Message;
                return default;
            }

        }

        /// <summary>
        /// Used to collect the user roles assigned at logon with the user token.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="error"></param>
        /// <returns></returns>
        public void GetUsersRoles<T>(out string error) where T : IErpUserRole
        {
            
            error = string.Empty;

            if (!Data.ContainsKey("authtoken") || !Data.ContainsKey("authuser"))
            {
                error = "The authentication token / user was not found in the request please try again.";
                return;
            }

            try
            {

                string AuthenticationToken      = Data["authtoken"];
                string AuthenticationUsername   = Data["authuser"];

                //Decrypt the AuthenticationToken
                AuthenticationToken = gl.DecryptAes(
                    AuthenticationToken,
                    GlobalApplicationKey,
                    Encoding.Unicode.GetBytes(
                        GlobalApplicationKey
                    )
                );

                List<T>     Roles = new();
                List<Claim> Claims = gl.JWTValidateTokenGetClaims(AuthenticationToken, gl.EncryptB64(GlobalApplicationKey));

                //Token roles we ned to remove, we only want internal roles
                string[] IgnoreRoles = new string[] { 
                    "exp", 
                    "iat", 
                    "nbf", 
                    "http://schemas.microsoft.com/ws/2008/06/identity/claims/expiration", 
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress", 
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/hash", 
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"
                };

                Claims.ForEach(c => { 
                    if (!IgnoreRoles.Contains(c.Type))
                        Roles.Add(c.ToRole<T>()); 
                });
                Claims.Clear();

                error = string.Empty;

                User        = AllUsers.Values.Where(u => u.Jtoken == AuthenticationToken).FirstOrDefault();
                UserRoles   = Roles.OfType<IErpUserRole>().ToArray();

            }
            catch (Exception ex)
            {
                error = ex.Message;
                return;
            }

        }

        /// <summary>
        /// Check Security Policies against the token on the request.
        /// </summary>
        /// <param name="Policies"></param>
        /// <returns></returns>
        public bool IsAllowed(params long[] Policies)
        {

            if (UserRoles == null || UserRoles.Length <= 0)
                return false;

            long[] UsersPolicies = UserRoles
                .SelectMany(p => p.RolePolicies)
                .ToArray()
                .Sort();

            if (Policies.Length > 1)
                Policies = Policies.Sort();

            foreach (long Policy in Policies)
                if (Array.BinarySearch(UsersPolicies, Policy) >= 0)
                    return true;

            return false;
        }

        /// <summary>
        /// This is used to try and build the parts from the query for a paged / non paged list from the IDataObject
        /// </summary>
        /// <param name="iDataObject"></param>
        /// <param name="drawNumber"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageOrder"></param>
        /// <param name="paging"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public ODataFieldSet[] TryGetFilter(Type iDataObject, out int drawNumber, out int pageNumber, out ODataOrder[] pageOrder, out ODataTakeSkip paging, out ODataCondition[] search)
        {

            ODataFieldSet[] output = TryGetFilter(iDataObject, null, out int OdrawNumber, out int OpageNumber, out ODataOrder[] OpageOrder, out ODataTakeSkip Opaging, out ODataCondition[] Osearch);

            drawNumber  = OdrawNumber;
            pageNumber  = OpageNumber;
            pageOrder   = OpageOrder;
            paging      = Opaging;
            search      = Osearch;

            return output;

        }

        /// <summary>
        /// This is used to try and build the parts from the query for a paged / non paged list from the IDataObject
        /// </summary>
        /// <param name="iDataObject"></param>
        /// <param name="drawNumber"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageOrder"></param>
        /// <param name="paging"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public ODataFieldSet[] TryGetFilter(Type iDataObject, ODataFieldSet[] append, out int drawNumber, out int pageNumber, out ODataOrder[] pageOrder, out ODataTakeSkip paging, out ODataCondition[] search) 
        {

            drawNumber  = 0;
            pageNumber  = 0;
            pageOrder   = null;
            paging      = null;
            search      = null;

            //If these are in the json query then we can create a paging option.
            //dataTables.Net jquery plugin (Page = draw, Take = length, Skip = start) or page = ((skip + take) / take)
            if (Data.ContainsKey("draw"))
                drawNumber = Convert.ToInt32(Data["draw"]);

            if (Data.ContainsKey("take") && Data.ContainsKey("skip"))
                if (Convert.ToInt32(Data["take"]) > 0)
                {
                    paging = new ODataTakeSkip() {
                        Take = Convert.ToInt32(Data["take"]),
                        Skip = Convert.ToInt32(Data["skip"])
                    };

                    if (Data.ContainsKey("page"))
                        pageNumber = Convert.ToInt32(Data["page"]);
                    else
                        pageNumber = ((Convert.ToInt32(Data["skip"]) + Convert.ToInt32(Data["take"])) / Convert.ToInt32(Data["take"]));
                }


            ODataFieldSet[] fields = iDataObject.GetFieldSets(ODataFieldSetType.SELECT);
            
            //If selected fields option is in the query we remove the unwanted ones. thgis one take presedence
            if (Data.ContainsKey("fields"))
            {
                if (JArray.Parse(Data["fields"]).Children().Any()) { 
                    fields = fields.Where(f => JArray.Parse(Data["fields"]).Select(o => ((JObject)o).Properties().FirstOrDefault().Value.ToString().ToLower()).ToArray().Contains(f.Column.Name.ToLower())).ToArray();
                }
            }

            //Based on jquery dataTables.net
            if (!Data.ContainsKey("fields") && JsonObjects.ContainsKey("Columns"))
                fields = fields.Where(f => ((JArray)JsonObjects["Columns"]).Select(o => ((JObject)o).Properties().FirstOrDefault().Value.ToString().ToLower()).ToArray().Contains(f.Column.Name.ToLower())).ToArray();
           
            //If in code we are appending then add at this point.
            if (append != null && append.Length > 0)
                fields = fields.Concat(append).ToArray();

            if (JsonObjects.ContainsKey("Order"))
            {

                ODataOrder[] pageOrderBuild = Array.Empty<ODataOrder>();

                ((JArray)JsonObjects["Order"]).ToArray().ForEach(o => {

                    string  colName     = string.Empty;
                    int     colIndex    = Convert.ToInt32(((JObject)o).Properties().First().Value.ToString());

                    try { colName = ((JObject)((JArray)JsonObjects["Columns"]).Children().ToArray()[colIndex]).Properties().FirstOrDefault().Value.ToString(); } catch { }

                    if (!string.IsNullOrEmpty(colName) && fields.Where(f => f.Column.Name == colName || f.Alias == colName).Any())
                        try
                        {
                            pageOrderBuild = pageOrderBuild.Append(new ODataOrder() {
                                Column  = fields.Where(f => f.Column.Name == colName || f.Alias == colName).FirstOrDefault().Column,
                                Order   = gl.StringToEnum<ODataOrderType>(((JObject)o).Properties().Last().Value.ToString().ToUpper())
                            }).ToArray();
                        }
                        catch { }

                });

                if(pageOrderBuild.Length > 0)
                    pageOrder = pageOrderBuild;

            }
            else
            {
                pageOrder = new ODataOrder[] {
                    new ODataOrder() {
                        Column = iDataObject.GetProperty("ViewOrder"),
                        Order = ODataOrderType.ASC
                    }
                };
            }

            if (JsonObjects.ContainsKey("Search"))
            {
                string ToSearch = ((JObject)JsonObjects["Search"]).Properties().Where(p => p.Name == "value").FirstOrDefault().Value.ToString();
                if (!string.IsNullOrEmpty(ToSearch)) {
                    List<ODataCondition> ToConditions = new();
                    fields.Where(f => f.Column.PropertyType == typeof(string)).ToArray().ForEach(f => {
                        ToConditions.Add(new ODataCondition() {
                            Column          = f.Column,
                            Operator        = ODataOperator.LIKE,
                            LikeOperator    = ODataLikeOperator.CONTAINS,
                            Values          = new object[] { ToSearch },
                            FollowBy        = ODataFollower.OR
                        });
                    });
                    ToConditions.First().Container  = ODataConditionContainer.OPEN;
                    ToConditions.Last().FollowBy    = ODataFollower.NONE;
                    ToConditions.Last().Container   = ODataConditionContainer.CLOSE;
                    search = ToConditions.ToArray();
                }

            }

            return fields;

        }

        /// <summary>
        /// This is used to build a request from the object data in the http request.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="apiCommandPrefix"></param>
        /// <param name="globalApplicationKey"></param>
        /// <param name="httpHeaders">Optional can be null.</param>
        /// <param name="commandStack"></param>
        /// <returns></returns>
        public static OApiJsonQuery Build<T, I>(string jsonString, string apiCommandPrefix, string globalApplicationKey, Dictionary<string, string> httpHeaders, Dictionary<long, IErpUser> erpUsers, string connectionString) where T : IErpUserRolePolicy where I : IErpUserRole
        {
            OApiJsonQuery jsonQuery = null;

            try
            {

                //Make the string oneliner.
                jsonString = jsonString.Replace("\t", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty);

                List<string> JsonTypes = new()
                {
                    ",\"Config\": {",
                    ",\"Config\":{",
                    ",\"Config\":\"{",
                    ",\"User\": {",
                    ",\"User\":{",
                    ",\"User\":\"{",
                    ",\"Options\": {",
                    ",\"Options\":{",
                    ",\"Options\":\"{",
                    ",\"Extra\": {",
                    ",\"Extra\":{",
                    ",\"Extra\":\"{",
                    ",\"Data\": {",
                    ",\"Data\":{",
                    ",\"Data\":\"{",
                };

                IEnumerable<string> segmentLookup = JsonTypes.Where(w => jsonString.Contains(w));

                //If there is a Stream item in the query we need to split it and return the query object with the stream in json string.
                if (segmentLookup.Any())
                {
                    jsonQuery = JsonConvert.DeserializeObject<OApiJsonQuery>(
                        ParseSegment(
                            jsonString,
                            segmentLookup.First(),
                            out string jsonObject
                        )
                    );

                    if (jsonQuery.Data.ContainsKey("contentType")) // make sure this is a json request as urlencoded from CORS sec
                        jsonQuery.Data["contentType"] = "Json";

                    jsonQuery.JsonObjects.Add(segmentLookup.First(),  JObject.Parse(jsonObject));

                }
                else
                    jsonQuery = JsonConvert.DeserializeObject<OApiJsonQuery>(jsonString);

                if(httpHeaders != null)
                    httpHeaders.ForEach(k => {
                        jsonQuery.Data.Add(k.Key, k.Value);
                    });

            }
            catch
            {
                try
                {

                    jsonString = HttpUtility.UrlDecode(jsonString);

                    jsonQuery = Build<T, I>(
                        JsonConvert.SerializeObject(
                            OQueryStringHelper.QueryStringToDict(jsonString)
                        ),
                        apiCommandPrefix,
                        globalApplicationKey, 
                        httpHeaders, 
                        erpUsers,
                        connectionString
                    );

                }
                catch
                {

                    try
                    {

                        throw new Exception("No more build parsers for the query supplied.");

                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }
            }

            jsonQuery.AllUsers = erpUsers;

            jsonQuery.GlobalApplicationKey = globalApplicationKey;

            jsonQuery.CommandStack = jsonQuery.CommandParser
                .Parse(jsonQuery.Command)
                .GetStack(apiCommandPrefix);

            if (jsonQuery.CommandStack == null || jsonQuery.CommandStack[0] != apiCommandPrefix)
                throw new Exception("Invalid command for this service api.");
         
            jsonQuery.GetUsersRoles<I>(out _);
            
            return jsonQuery;

        }

        /// <summary>
        /// This is used to build a request from the object data in the http request.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="apiCommandPrefix"></param>
        /// <param name="globalApplicationKey"></param>
        /// <param name="httpHeaders">Optional can be null.</param>
        /// <param name="commandStack"></param>
        /// <returns></returns>
        public static OApiJsonQuery Build<T, I>(JObject json, string apiCommandPrefix, string globalApplicationKey, Dictionary<string, string> httpHeaders, Dictionary<long, IErpUser> erpUsers, string connectionString) where T : IErpUserRolePolicy where I : IErpUserRole
        {
            OApiJsonQuery jsonQuery = null;

            try
            {

                jsonQuery = new OApiJsonQuery(apiCommandPrefix, globalApplicationKey)
                {
                    Command = json.Properties().Where(x => x.Name == "Command").First().Value.ToString(),
                };

                jsonQuery.Data = ((JObject)json.Properties().Where(x => x.Name == "Data").First().Value)
                    .Properties()
                    .ToDictionary(p => p.Name, p => p.Value.ToString());

                if (httpHeaders != null)
                    jsonQuery.Data = jsonQuery.Data
                        .Concat(httpHeaders.Where(x => !jsonQuery.Data.Keys.Contains(x.Key)))
                        .ToDictionary(k => k.Key, k => k.Value);
                    
                List<string> AttachedJsonTypes = new()
                {
                    "Command",
                    "Data"
                };

                IEnumerable<JProperty> ExtendedObjs = json.Properties().Where(x => !AttachedJsonTypes.Contains(x.Name));

                if (ExtendedObjs.Any())
                    jsonQuery.JsonObjects = ExtendedObjs.ToDictionary(p => p.Name, p => (JContainer)p.Value);

                jsonQuery.AllUsers = erpUsers;

                jsonQuery.CommandStack = jsonQuery.CommandParser
                    .Parse(jsonQuery.Command)
                    .GetStack(apiCommandPrefix);

                jsonQuery.Command = jsonQuery.Command.Replace(apiCommandPrefix + ".", string.Empty);

                jsonQuery.GetUsersRoles<I>(out _);

            }
            catch
            {
                return Build<T, I>(JsonConvert.SerializeObject(json), apiCommandPrefix, globalApplicationKey, httpHeaders, erpUsers, connectionString);
            }

            if (jsonQuery.CommandStack == null || jsonQuery.CommandStack[0] != apiCommandPrefix)
                throw new Exception("Invalid command for this service api.");

            return jsonQuery;

        }

        /// <summary>
        /// This is used to build a request from the object data in the http request.
        /// </summary>
        /// <param name="apiCommandPrefix"></param>
        /// <param name="globalApplicationKey"></param>
        /// <param name="httpHeaders">Optional can be null.</param>
        /// <returns></returns>
        public static OApiJsonQuery Build<T, I>(string apiCommandPrefix, string globalApplicationKey, Dictionary<string, string> httpHeaders, Dictionary<long, IErpUser> erpUsers) where T : IErpUserRolePolicy where I : IErpUserRole
        {
            OApiJsonQuery jsonQuery = null;

            try
            {

                jsonQuery = new OApiJsonQuery(apiCommandPrefix, globalApplicationKey) { Command = apiCommandPrefix + ".null" };

                if (httpHeaders != null)
                    jsonQuery.Data = jsonQuery.Data
                        .Concat(httpHeaders.Where(x => !jsonQuery.Data.Keys.Contains(x.Key)))
                        .ToDictionary(k => k.Key, k => k.Value);

                jsonQuery.AllUsers = erpUsers;

                jsonQuery.CommandStack = jsonQuery.CommandParser
                    .Parse(jsonQuery.Command)
                    .GetStack(apiCommandPrefix);

                jsonQuery.Command = jsonQuery.Command.Replace(apiCommandPrefix + ".", string.Empty);

                jsonQuery.GetUsersRoles<I>(out _);

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (jsonQuery.CommandStack == null || jsonQuery.CommandStack[0] != apiCommandPrefix)
                throw new Exception("Invalid command for this service api.");

            return jsonQuery;

        }

        /// <summary>
        /// Used to parse the string data and split it for the deserialize object process.
        /// </summary>
        /// <param name="jsonString"></param>
        /// <param name="segmentString"></param>
        /// <param name="JsonObject"></param>
        /// <returns></returns>
        public static string ParseSegment(string jsonString, string segmentString, out string JsonObject)
        {

            string jsonStreamString = jsonString.Remove(0, jsonString.IndexOf(segmentString));

            jsonStreamString    = jsonStreamString.Remove(jsonStreamString.Length - 1);
            jsonString          = jsonString.Replace(jsonStreamString, string.Empty);
            jsonStreamString    = jsonStreamString.Remove(jsonStreamString.Length - 1);
            jsonStreamString    = jsonStreamString.Replace(segmentString, string.Empty);

            jsonStreamString = Regex.Unescape(jsonStreamString);
            JsonObject = "{" + jsonStreamString + "}";

            try
            {
                //check UrlEncoding Bug "]}}}}" << EOF has an extra end bracket ?
                if (JsonObject[JsonObject.LastIndexOf("]")..] == "]}}}}")
                    JsonObject = JsonObject.Remove(JsonObject.Length - 1);
            }
            catch { }


            return jsonString;

        }

        /// <summary>
        /// Returns an instance of an error object to serialize with json convert.
        /// </summary>
        /// <param name="e">The exception error object</param>
        /// <returns>The instance of and OApiJsonException object.</returns>
        public static OApiJsonQueryException BuildError(Exception e) 
        {

            return new OApiJsonQueryException()
            {
                Message    = e.Message,
                DateStamp  = DateTime.Now
            };

        }

        /// <summary>
        /// Returns an instance of an error object to serialize with json convert.
        /// </summary>
        /// <param name="e">The exception error message</param>
        /// <returns>The instance of and OApiJsonException object.</returns>
        public static OApiJsonQueryException BuildError(string e)
        {
            
            return new OApiJsonQueryException()
            {
                Message    = e,
                DateStamp  = DateTime.Now
            };

        }

        /// <summary>
        /// Returns an instance of an normal response object to serialize with json convert.
        /// </summary>
        /// <param name="e">The string message if any</param>
        /// <returns>The instance of and OApiJsonQueryResponse object.</returns>
        public static OApiJsonQueryResponse BuildResponse(string e, JContainer data)
        {
            
            if (data != null && data.GetType() == typeof(JObject))
                if(((JObject)data).Properties().Any() && ((JObject)data).Properties().First().Name.ToLower() == ODataJsonPathDefault)
                    data = (JArray)data.First().First();

            return new OApiJsonQueryResponse()
            {
                Message     = e,
                DateStamp   = DateTime.Now,
                Data        = data
            };

        }

        /// <summary>
        /// Returns an instance of an normal response object to serialize with json convert.
        /// </summary>
        /// <param name="e">The string message if any</param>
        /// <returns>The instance of and OApiJsonQueryResponse object.</returns>
        public static OApiJsonQueryResponsePaging BuildResponse(int drawNumber, long totalRecords, long totalRecordsFiltered, JContainer data)
        {
            return new OApiJsonQueryResponsePaging()
            {
                Draw            = drawNumber,
                RecordsTotal    = totalRecords,
                RecordsFiltered = totalRecordsFiltered,
                Data            = data
            };
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
