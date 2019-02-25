using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace GolfCore.Helpers
{
    public class WebConnectHelper
    {
        /// <summary>
        /// For login
        /// </summary>
        /// <param name="url">url for login</param>
        /// <param name="postData">login / pass data</param>
        /// <returns></returns>
        public static CookieCollection? MakePost4Cookies(string url, string postData)
        {
            HttpWebRequest? http= (HttpWebRequest)WebRequest.Create(url);
            if (http == null) return null;

            http.KeepAlive = true;
            http.Method = "POST";
            http.ContentType = "application/x-www-form-urlencoded";
            //string postData = ""; //enter login pass
            byte[] dataBytes = UTF8Encoding.UTF8.GetBytes(postData);
            http.ContentLength = dataBytes.Length;
            using (Stream postStream = http.GetRequestStream())
            {
                postStream.Write(dataBytes, 0, dataBytes.Length);
            }
            HttpWebResponse? httpResponse;
            try
            {
                httpResponse = http.GetResponse() as HttpWebResponse;
            }
            catch(Exception ex)
            {
                Log.New(ex);
                return null;
            }
            if (httpResponse == null) return null; 

            if (httpResponse.Headers.AllKeys.ToList().Contains("Set-Cookie"))
            {
                for (int i = 0; i < httpResponse.Headers.Count; i++)
                {
                    string name = httpResponse.Headers.GetKey(i);
                    if (name != "Set-Cookie")
                        continue;
                    string value = httpResponse.Headers.Get(i);
                    value = Regex.Replace(value, "(e|E)xpires=(.+?)(;|$)|(P|p)ath=(.+?);", "");
                    foreach (var singleCookie in value.Split(','))
                    {
                        Match match = Regex.Match(singleCookie, "(.+?)=(.+?);");
                        if (match.Captures.Count == 0)
                            continue;
                        httpResponse.Cookies.Add(
                            new Cookie(
                                match.Groups[1].ToString().Trim(),
                                match.Groups[2].ToString().Trim(),
                                "/",
                                httpResponse.ResponseUri.Host.Split(':')[0]));
                    }
                }
            }

            CookieCollection result = httpResponse.Cookies;
            return result;
        }

        /// <summary>
        /// For getting info using login auth cookies
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static string? MakePostWithCookies(string url, CookieCollection cookies)
        {
            // Probably want to inspect the http.Headers here first
            HttpWebRequest? http = (HttpWebRequest)WebRequest.Create(url);
            if (http == null) return null;
            if (cookies != null)
            {
                http.CookieContainer = new CookieContainer();
                http.CookieContainer.Add(cookies);
            }
            HttpWebResponse? response;
            try
            {
                response = http.GetResponse() as HttpWebResponse;
            }
            catch(Exception ex)
            {
                Log.New(ex);
                return null;
            }
            if (response == null) return null;

            return GetContentsFromResponse(response);
        }

        /// <summary>
        /// For retrieving common info
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static string? MakePostWithoutCookies(string url)
        {
            // Probably want to inspect the http.Headers here first
            HttpWebRequest? http = (HttpWebRequest)WebRequest.Create(url);
            if (http == null) return null;
            
            HttpWebResponse? response;
            try
            {
                response = http.GetResponse() as HttpWebResponse;
            }
            catch (Exception ex)
            {
                Log.New(ex);
                return null;
            }
            if (response == null) return null;

            return GetContentsFromResponse(response);
        }

        public static string? MakePost(string url, CookieCollection cookies, string loginUrl, string loginPostData, out CookieCollection? newCookies)
        {
            string? result = null;
            newCookies = null;
            if (cookies != null)
            {
                result = MakePostWithCookies(url, cookies);
            }

            if (result == null)
            {
                newCookies = MakePost4Cookies(loginUrl, loginPostData);
                result = MakePostWithCookies(url, cookies);
            }
            return result;
        }

        public static HttpWebResponse? MakePostRaw(string url, string? postData, string? accept = null)
        {
            HttpWebRequest? http = (HttpWebRequest)WebRequest.Create(url);
            if (http == null) return null;
            http.KeepAlive = true;
            http.Method = "POST";
            http.ContentType = "application/x-www-form-urlencoded";
            if (accept != null) http.Accept = accept;

            if (postData != null)
            {
                byte[] dataBytes = UTF8Encoding.UTF8.GetBytes(postData);
                http.ContentLength = dataBytes.Length;
                using (Stream postStream = http.GetRequestStream())
                {
                    postStream.Write(dataBytes, 0, dataBytes.Length);
                }
            }
            HttpWebResponse? httpResponse;
            try
            {
                httpResponse = http.GetResponse() as HttpWebResponse;
            }
            catch(Exception ex)
            {
                Log.New(ex);
                return null;
            }

            return httpResponse;
        }

        public static string? MakePost(string url, string? postData, string? accept = null)
        {

            HttpWebResponse? httpResponse = MakePostRaw(url, postData, accept);
            if (httpResponse == null) return null;
            return GetContentsFromResponse(httpResponse);
        }

        public static string? GetContentsFromResponse(HttpWebResponse response)
        {
            string? data = null;
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
