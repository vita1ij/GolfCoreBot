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
        public static CookieCollection MakePost4Cookies(string url, string postData)
        {
            HttpWebRequest http = WebRequest.Create(url) as HttpWebRequest;
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
            HttpWebResponse httpResponse = http.GetResponse() as HttpWebResponse;

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
                                match.Groups[1].ToString(),
                                match.Groups[2].ToString(),
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
        public static string MakePostWithCookies(string url, CookieCollection cookies)
        {
            // Probably want to inspect the http.Headers here first
            HttpWebRequest http = WebRequest.Create(url) as HttpWebRequest;
            http.CookieContainer = new CookieContainer();
            http.CookieContainer.Add(cookies);
            HttpWebResponse response = http.GetResponse() as HttpWebResponse;

            string data = GetContentsFromResponse(response);

            return data;
        }

        public static string MakePost(string url, CookieCollection cookies, string loginUrl, string loginPostData, out CookieCollection newCookies)
        {
            string result = null;
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

        public static string GetContentsFromResponse(HttpWebResponse response)
        {
            string data = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
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
