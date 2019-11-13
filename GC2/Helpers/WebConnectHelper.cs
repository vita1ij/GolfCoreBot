using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

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
            var cookieJar = new CookieContainer();
            var uri = new Uri(url);
            using (var httpClientHandler = new HttpClientHandler
            {
                CookieContainer = cookieJar
            })
            {
                using (var client = new HttpClient(httpClientHandler))
                {
                    // add values to data for post
                    FormUrlEncodedContent content = new FormUrlEncodedContent(values);

                    // Post data
                    var result = client.PostAsync(uri, content).Result;

                    if (result.Headers.Contains("Set-Cookie"))
                    {
                        return cookieJar.GetCookies(uri);
                    }
                    return null;
                }
            }
        }

        /// <summary>
        /// For getting info using login auth cookies
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static string MakePost(string url, CookieCollection cookies, List<KeyValuePair<string, string>> values = null)
        {
            var cookieJar = new CookieContainer();
            if (cookies != null)
            {
                foreach(var cookie in cookies)
                {
                    cookieJar.Add(cookie as Cookie);
                }
            }
            var uri = new Uri(url);
            using (var httpClientHandler = new HttpClientHandler { CookieContainer = cookieJar })
            {
                using (var client = new HttpClient(httpClientHandler))
                {
                    HttpResponseMessage result; 

                    if (values != null)
                    {
                        FormUrlEncodedContent content = new FormUrlEncodedContent(values);
                        result = client.PostAsync(uri, content).Result;
                    }
                    else
                    {
                        result = client.GetAsync(uri).Result;
                    }
                    if (result != null)
                    {
                        try
                        {
                            return result.Content.ReadAsStringAsync().Result;
                        }
                        catch(Exception ex)
                        {
                            Log.New(ex);
                            return null;
                        }
                    }
                }
            }

            //    if (http == null) return null;
            //if (cookies != null)
            //{
            //    http.CookieContainer = new CookieContainer();
            //    foreach(Cookie c in cookies)
            //    {
            //        http.CookieContainer.Add(new Cookie
            //        {
            //            Domain = c.Domain,
            //            Name = c.Name,
            //            Path = c.Path,
            //            Value = c.Value
            //        });
            //    }
            //}
            //if (values != null)
            //{
            //    http.Method = "POST";

            //}

            //HttpWebResponse response;
            //try
            //{
            //    response = http.GetResponse() as HttpWebResponse;
            //}
            //catch (Exception ex)
            //{
            //    Log.New(ex);
            //    return null;
            //}
            //if (response == null) return null;

            //return GetContentsFromResponse(response);
            return null;
        }

        public static string GetContentsFromResponse(HttpWebResponse response)
        {
            string data = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream;

                if (String.IsNullOrWhiteSpace(response.CharacterSet))
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
            }
            return data;
        }
    }
}
