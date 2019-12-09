using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace GC2.Helpers
{
    public class WebConnectHelper
    {
        /// <summary>
        /// For login
        /// </summary>
        /// <param name="url">url for login</param>
        /// <param name="values">login / pass data</param>
        /// <returns></returns>
        public static CookieCollection MakePost4Cookies(string url, List<KeyValuePair<string, string>> values)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", "PostmanRuntime/7.19.0");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            var postdata = String.Join("&", values.Select(x => $"{x.Key}={x.Value}"));
            request.AddParameter("undefined", postdata, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            if (response.Cookies?.Any() ?? false)
            {
                var cookieJar = new CookieCollection();
                foreach (var c in response.Cookies)
                {
                    cookieJar.Add(new Cookie(c.Name, c.Value)
                    {
                        Domain = c.Domain,
                        Expires = c.Expires,
                        Path = c.Path
                    });
                }
                return cookieJar;
            }
            return null;
        }

        /// <summary>
        /// For getting info using login auth cookies
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static string MakeGetPost(string url, CookieCollection cookies, List<KeyValuePair<string, string>> values = null)
        {
            var client = new RestClient(url);
            var request = (values == null) ? new RestRequest(Method.GET) : new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Referer", url);
            if (cookies != null)
            {
                List<string> cookiesStringList = new List<string>();
                foreach (Cookie cookie in cookies)
                {
                    cookiesStringList.Add($"{cookie.Name}={cookie.Value}");
                }
                string cookiesString = String.Join("; ", cookiesStringList);
                request.AddHeader("Cookie", cookiesString);
            }

            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            if (values != null && values.Any())
            {
                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                var postdata = String.Join("&", values.Select(x => $"{x.Key}={x.Value}"));
                request.AddParameter("undefined", postdata, ParameterType.RequestBody);
            }

            IRestResponse response = client.Execute(request);

            return GetContentsFromResponse(response);
        }

        public static string GetContentsFromResponse(IRestResponse response)
        {
            string data = null;
            if (response == null) return null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Content;
            }
            return data;
        }
    }
}
