using GC2.Classes;
using GC2.Helpers;
using GC2DB.Data;
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
        public abstract string LoginPostData { get; }

        public bool Login()
        {
            if (LoginUrl == null || LoginPostData == null) return false;
            ConnectionCookie = WebConnectHelper.MakePost4Cookies(LoginUrl, LoginPostData);
            return (ConnectionCookie != null && ConnectionCookie.Count > 0);
        }

        public abstract bool IsLoginPage(string data);

        public abstract bool EnterCode(string code);

        public abstract string GetTask(out List<object> stuff);
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
