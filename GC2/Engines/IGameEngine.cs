using GC2.Classes;
using GC2.Helpers;
using GC2DB.Data;
using GC2DB.Managers;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace GC2.Engines
{
    public abstract class IGameEngine
    {
        protected string _login;
        protected string _password;

        public CookieCollection ConnectionCookie { get; set; }
        public abstract string LoginUrl { get; }
        public abstract string TaskUrl { get; }
        public string StatisticsUrl { get; set; }
        public abstract List<KeyValuePair<string, string>> LoginPostValues { get; }

        public bool Login(Game game = null)
        {
            //Check DB
            if (game != null && !String.IsNullOrEmpty(game.Cookies))
            {
                if (!game.HadErrorsWhileLogin)
                {
                    return true;
                }
                else if (!game.TryToLogIn)
                {
                    return false;
                }
            }

            //Check Settings
            if (LoginUrl == null || LoginPostValues == null) return false;
            if (_login == null || _password == null) return false;

            //get Cookies
            ConnectionCookie = WebConnectHelper.MakePost4Cookies(LoginUrl, LoginPostValues);

            //Save Changes
            if (ConnectionCookie != null && ConnectionCookie.Count > 0)
            {
                if (game != null)
                {
                    List<string> cookiesStringList = new List<string>();
                    foreach (Cookie cookie in ConnectionCookie)
                    {
                        cookiesStringList.Add($"{cookie.Name}={cookie.Value}");
                    }
                    string cookiesString = String.Join("; ", cookiesStringList);
                    game.Cookies = cookiesString;

                    game.HadErrorsWhileLogin = false;
                    game.TryToLogIn = true;
                    GameManager.Update(game);
                }
                return true;
            }
            else
            {
                if (game != null)
                {
                    game.Cookies = null;
                    game.TryToLogIn = !game.HadErrorsWhileLogin;
                    game.HadErrorsWhileLogin = true;
                    game.Login = null;
                    game.Password = null;
                    GameManager.Update(game);
                }
                return false;
            }
        }

        public abstract bool IsLoginPage(HtmlDocument data);

        public abstract bool EnterCode(string code);

        public abstract GameTask GetTask(out List<object> stuff);
        public abstract GameStatistics GetStatistics();

        public virtual void Init(Game game)
        {
            this._login = game.Login;
            this._password = game.Password;
            Login();
        }

        public static IGameEngine Get(Game game)
        {
            IGameEngine engine;
            switch(game.Type)
            {
                case GameType.IgraLv: 
                    engine = new IgraLvGameEngine(game) as IGameEngine; 
                    break;
                case GameType.Demo:   
                    engine = new DemoEnCxGameEngine(game); 
                    break;
                case GameType.EnCx:   
                    engine = new DemoEnCxGameEngine(game); 
                    break;
                case GameType.LvlUp:
                    engine = new DemoEnCxGameEngine(game);
                    break;
                default:
                    throw new NotImplementedException();
            };
            engine.Init(game);
            return engine;
        }
    }
}
