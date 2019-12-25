using GC2.Classes;
using GC2.Helpers;
using GC2DB.Data;
using GC2DB.Managers;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace GC2.Engines
{
    public abstract class IGameEngine
    {
        protected string _login = String.Empty;
        protected string _password = String.Empty;

        //https://docs.microsoft.com/en-us/dotnet/api/system.threading.mutex?view=netframework-4.8
        private static readonly Dictionary<long, Mutex> SafeLoginDict = new Dictionary<long, Mutex>();
        private const int MutexWaitTime = 10000;

        public CookieCollection? ConnectionCookie { get; set; }
        public abstract string LoginUrl { get; }
        public abstract string TaskUrl { get; }
        public string? StatisticsUrl { get; set; }
        public abstract List<KeyValuePair<string, string>>? LoginPostValues { get; }

        public bool Login(Game game)
        {
            if (game == null || !game.Id.HasValue) return false;
            //Get Mutex
            if (!SafeLoginDict.ContainsKey(game.Id.Value))
            {
                SafeLoginDict.Add(game.Id.Value, new Mutex());
            }
            var mutex = SafeLoginDict[game.Id.Value];
            if (mutex.WaitOne(MutexWaitTime))
            {
                //action
                try
                {

                    //Check DB
                    if (game != null && !String.IsNullOrEmpty(game.Cookies))
                    {
                        if (!game.HadErrorsWhileLogin)
                        {
                            ConnectionCookie = new CookieCollection();
                            game.Cookies.Split(";")
                                        .ToList()
                                        .Select(x => new Cookie(x.Split("=")[0].Trim(), x.Split("=")[1].Trim()))
                                        .ToList()
                                        .ForEach(x => ConnectionCookie.Add(x));
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
                            foreach (Cookie? cookie in ConnectionCookie)
                                if (cookie != null)
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
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            else
            {
                return false;
            }

        }

        public abstract bool IsLoginPage(HtmlDocument data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="game"></param>
        /// <returns>null if error; true/false = correct code</returns>
        public abstract bool? EnterCode(string code, Game game);

        public abstract GameTask GetTask(out List<object>? stuff);
        public abstract GameStatistics? GetStatistics();

        public virtual void Init(Game game)
        {
            this._login = game.Login ?? String.Empty;
            this._password = game.Password ?? String.Empty;
            Login(game);
        }

        public static IGameEngine Get(Game game)
        {
            var engine = game.Type switch
            {
                GameType.IgraLv => new IgraLvGameEngine(game) as IGameEngine,
                GameType.Demo => new DemoEnCxGameEngine(game),
                GameType.EnCx => new QuestEnCxGameEngine(game),
                GameType.LvlUp => new DemoEnCxGameEngine(game),
                _ => throw new NotImplementedException(),
            };
            ;
            engine.Init(game);
            return engine;
        }
    }
}
