using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        /// <param name="postData">login / pass data</param>
        /// <returns></returns>
        public static CookieCollection MakePost4Cookies(string url, string postData)
        {
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(url); //do we need query?
            if (http == null) return null;
            var cookieJar = new CookieContainer();
            http.KeepAlive = true;
            http.Method = "POST";
            http.ContentType = "application/x-www-form-urlencoded";
            http.CookieContainer = cookieJar;
            byte[] dataBytes = UTF8Encoding.UTF8.GetBytes(postData);
            http.ContentLength = dataBytes.Length;
            using (Stream postStream = http.GetRequestStream())
            {
                postStream.Write(dataBytes, 0, dataBytes.Length);
            }
            HttpWebResponse httpResponse;
            try
            {
                httpResponse = http.GetResponse() as HttpWebResponse;
            }
            catch (Exception ex)
            {
                Log.New(ex);
                return null;
            }
            if (httpResponse == null) return null;

            if (httpResponse.Headers.AllKeys.ToList().Contains("Set-Cookie"))
            {
                return cookieJar.GetCookies(http.RequestUri);
            }
            return null;
        }

        /// <summary>
        /// For getting info using login auth cookies
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static string MakePost(string url, CookieCollection cookies = null)
        {
            // Probably want to inspect the http.Headers here first
            HttpWebRequest http = (HttpWebRequest)WebRequest.Create(url); //do we need query?
            if (http == null) return null;
            if (cookies != null)
            {
                http.CookieContainer = new CookieContainer();
                foreach(Cookie c in cookies)
                {
                    http.CookieContainer.Add(new Cookie
                    {
                        Domain = c.Domain,
                        Name = c.Name,
                        Path = c.Path,
                        Value = c.Value
                    });
                }
            }
            HttpWebResponse response;
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
