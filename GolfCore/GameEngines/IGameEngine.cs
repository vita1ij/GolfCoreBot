using GolfCore.Helpers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GolfCore.GameEngines
{
    public abstract class IGameEngine
    {
        private string _login;
        private string _pass;

        public CookieCollection connectionCookie;

        public string ConnectionCookieName;

        public string LoginUrl;
        public string StatisticsUrl;
        public string TaskUrl;

        public string LoginPostData;

        //public IGameEngine(string login, string pass)
        //{
        //    _login = login;
        //    _pass = pass;
        //}

        public bool Login()
        {
            connectionCookie = WebConnectHelper.MakePost4Cookies(LoginUrl, LoginPostData);
            return (connectionCookie != null);
        }

        public abstract string GetTask();
        public abstract string GetStatistics();
    }
}
