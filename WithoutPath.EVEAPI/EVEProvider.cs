using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using WithoutPath.EVEAPI.JsonTypes;
using Ninject;
using WithoutPath.Global.Config;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace WithoutPath.EVEAPI
{
    public class EVEProvider : IEVEProvider
    {
        private const string authorizeUri = "https://login.eveonline.com/oauth/authorize/?response_type=code&redirect_uri={0}&client_id={1}&scope={2}&state=evesso";
        private const string tokenUri = "https://login.eveonline.com/oauth/token";
        private const string verifyUri = "https://login.eveonline.com/oauth/verify";
        private const string scope = "characterLocationRead%20esi-location.read_location.v1%20esi-location.read_ship_type.v1%20esi-ui.write_waypoint.v1";

        private const string character = "https://esi.tech.ccp.is/latest/characters/{0}/?datasource=tranquility";
        private const string characterLocationCrest = "https://crest-tq.eveonline.com/characters/{0}/location/";
        private const string characterLocation = "https://esi.tech.ccp.is/latest/characters/{0}/location/?datasource=tranquility&token={1}";
        private const string characterShip = "https://esi.tech.ccp.is/latest/characters/{0}/ship/?datasource=tranquility&token={1}";
        private const string shipType = "https://esi.tech.ccp.is/latest/universe/types/{0}/?datasource=tranquility&language=en-us";
        private const string waypoint = "https://esi.tech.ccp.is/latest/ui/autopilot/waypoint/?add_to_beginning=false&clear_other_waypoints=false&datasource=tranquility&destination_id={0}&token={1}";

        private static string system = "https://esi.tech.ccp.is/latest/universe/systems/{0}/?datasource=tranquility&language=en-us";

        [Inject]
        public IConfig Config { get; set; }
        private Subject<Task<HttpWebResponse>> AccessSubject { get; set; }
        private Subject<Task<HttpWebResponse>> ApiSubject { get; set; }

        public EVEProvider()
        {
            #region Access scheduler

            AccessSubject = new Subject<Task<HttpWebResponse>>();

            int AccessCount = 0;
            var AccessStart = DateTimeOffset.Now;
            var AccessEnd = DateTimeOffset.Now;
            TimeSpan AccessOffset;
            AccessSubject.Timestamp().Subscribe(x =>
            {
                if (AccessCount == 0)
                {
                    AccessStart = x.Timestamp;
                    AccessCount++;
                }
                else if (AccessCount == 19) // максимум 20 запросов в секунду
                {
                    AccessEnd = x.Timestamp;
                    AccessOffset = AccessEnd - AccessStart;
                    if (AccessOffset.TotalMilliseconds < 1000)
                    {
                        Thread.Sleep(1000 - AccessOffset.Milliseconds);
                    }
                    AccessCount = 0;
                }
                else
                {
                    AccessCount++;
                }
                x.Value.Start();
            });

            #endregion

            #region Api scheduler

            ApiSubject = new Subject<Task<HttpWebResponse>>();

            int ApiResponseCount = 0;
            var ApiResponseStart = DateTimeOffset.Now;
            var ApiResponseEnd = DateTimeOffset.Now;
            TimeSpan ApiResponseOffset;
            ApiSubject.Timestamp().Subscribe(x =>
            {
                if (ApiResponseCount == 0)
                {
                    ApiResponseStart = x.Timestamp;
                    ApiResponseCount++;
                }
                else if (ApiResponseCount == 149) // максимум 150 запросов в секунду
                {
                    ApiResponseEnd = x.Timestamp;
                    ApiResponseOffset = ApiResponseEnd - ApiResponseStart;
                    if (ApiResponseOffset.TotalMilliseconds < 1000)
                    {
                        Thread.Sleep(1000 - ApiResponseOffset.Milliseconds);
                    }
                    ApiResponseCount = 0;
                }
                else
                {
                    ApiResponseCount++;
                }
                x.Value.Start();
            });

            #endregion
        }

        private Task<HttpWebResponse> GetResponseAsync(HttpWebRequest request)
        {
            return new Task<HttpWebResponse>(() => { return (HttpWebResponse)request.GetResponse(); });
        }

        public string Authorize(string redirectTo)
        {
            return string.Format(authorizeUri, redirectTo, Config.EVESetting.AppID, scope);
        }

        public AuthResponse GetAccessToken(string code, bool isRefresh = false)
        {
            try
            {
                var authHeaderFormat = "Basic {0}";

                var authHeader = string.Format(authHeaderFormat,
                                                Convert.ToBase64String(
                                                    Encoding.UTF8.GetBytes(Uri.EscapeDataString(Config.EVESetting.AppID) + ":" +
                                                                            Uri.EscapeDataString(Config.EVESetting.AppSecret))
                                                    ));

                var postBody = (isRefresh ? "grant_type=refresh_token&refresh_token=" : "grant_type=authorization_code&code=") + code;
                var authRequest = (HttpWebRequest)WebRequest.Create(tokenUri);

                authRequest.Method = "POST";
                authRequest.Headers.Add("Authorization", authHeader);
                authRequest.ContentType = "application/x-www-form-urlencoded";
                authRequest.Host = "login.eveonline.com";
                using (var stream = authRequest.GetRequestStream())
                {
                    byte[] content = ASCIIEncoding.ASCII.GetBytes(postBody);
                    stream.Write(content, 0, content.Length);
                }

                using (var task = GetResponseAsync(authRequest))
                {
                    AccessSubject.OnNext(task);
                    task.Wait();
                    var webAuthResponse = task.Result;
                    if (webAuthResponse.StatusCode == HttpStatusCode.OK)
                    {
                        using (webAuthResponse)
                        {
                            using (var reader = new StreamReader(webAuthResponse.GetResponseStream()))
                            {
                                return JsonConvert.DeserializeObject<AuthResponse>(reader.ReadToEnd());
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public VerifyCharacterResponse VerifyCharacter(string Token)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(verifyUri);

                request.Method = "GET";
                request.UserAgent = "ASP.NET MVC";
                request.Headers.Add("Authorization", "Bearer " + Token);
                request.Host = "login.eveonline.com";

                using (var task = GetResponseAsync(request))
                {
                    ApiSubject.OnNext(task);
                    task.Wait();
                    var response = task.Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (response)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                return JsonConvert.DeserializeObject<VerifyCharacterResponse>(reader.ReadToEnd());
                            }
                        }
                    }
                }
                return null;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public CharacterResponse GetCharacter(long Id)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(string.Format(character, Id));

                request.Method = "GET";
                request.Accept = "application/json";
                request.UserAgent = "ASP.NET MVC";

                using (var task = GetResponseAsync(request))
                {
                    ApiSubject.OnNext(task);
                    task.Wait();
                    var response = task.Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (response)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                return JsonConvert.DeserializeObject<CharacterResponse>(reader.ReadToEnd());
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public LocationResponseCrest GetCharacterLocationCrest(long Id, string Token)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(string.Format(characterLocationCrest, Id));

                request.Method = "GET";
                request.Accept = "application/json";
                request.Headers.Add("Authorization", "Bearer " + Token);
                request.UserAgent = "ASP.NET MVC";

                using (var task = GetResponseAsync(request))
                {
                    ApiSubject.OnNext(task);
                    task.Wait();
                    var response = task.Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (response)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                return JsonConvert.DeserializeObject<LocationResponseCrest>(reader.ReadToEnd());
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public LocationResponse GetCharacterLocation(long Id, string Token)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(string.Format(characterLocation, Id, Token));

                request.Method = "GET";
                request.Accept = "application/json";
                request.UserAgent = "ASP.NET MVC";

                using (var task = GetResponseAsync(request))
                {
                    ApiSubject.OnNext(task);
                    task.Wait();
                    var response = task.Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (response)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                return JsonConvert.DeserializeObject<LocationResponse>(reader.ReadToEnd());
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public ShipResponse GetCharacterShip(long Id, string Token)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(string.Format(characterShip, Id, Token));

                request.Method = "GET";
                request.Accept = "application/json";
                request.UserAgent = "ASP.NET MVC";

                using (var task = GetResponseAsync(request))
                {
                    ApiSubject.OnNext(task);
                    task.Wait();
                    var response = task.Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (response)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                return JsonConvert.DeserializeObject<ShipResponse>(reader.ReadToEnd());
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public SolarSystemResponse GetSolarSystem(long systemId)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(string.Format(system, systemId));

                request.Method = "GET";
                request.Accept = "application/json";
                request.UserAgent = "ASP.NET MVC";
                using (var task = GetResponseAsync(request))
                {
                    ApiSubject.OnNext(task);
                    task.Wait();
                    var response = task.Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (response)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                return JsonConvert.DeserializeObject<SolarSystemResponse>(reader.ReadToEnd());
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public ShipTypeResponse GetShipType(long typeId)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(string.Format(shipType,typeId));

                request.Method = "GET";
                request.Accept = "application/json";
                request.UserAgent = "ASP.NET MVC";
                using (var task = GetResponseAsync(request))
                {
                    ApiSubject.OnNext(task);
                    task.Wait();
                    var response = task.Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (response)
                        {
                            using (var reader = new StreamReader(response.GetResponseStream()))
                            {
                                return JsonConvert.DeserializeObject<ShipTypeResponse>(reader.ReadToEnd());
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public HttpStatusCode SetWaypoint(long destinationId, string Token)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(string.Format(waypoint, destinationId, Token));

                request.Method = "POST";
                request.Accept = "application/json";
                request.ContentLength = 0;
                request.UserAgent = "ASP.NET MVC";

                using (var task = GetResponseAsync(request))
                {
                    ApiSubject.OnNext(task);
                    task.Wait();
                    var response = task.Result;

                    return response.StatusCode;
                }
            }
            catch (Exception e)
            {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
