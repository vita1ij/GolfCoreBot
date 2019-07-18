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
        protected string? _login;
        protected string? _password;

        public CookieCollection? ConnectionCookie { get; set; }
        public abstract string LoginUrl { get; }
        public abstract string TaskUrl { get; }
        public string? StatisticsUrl { get; set; }
        public abstract string LoginPostData { get; }

        public bool Login()
        {
            if (LoginUrl == null || LoginPostData == null) return false;
            ConnectionCookie = WebConnectHelper.MakePost4Cookies(LoginUrl, LoginPostData);
            return (ConnectionCookie != null && ConnectionCookie.Count > 0);
        }

        public abstract bool IsLoginPage(string data);

        public abstract bool EnterCode(string code);

        public abstract string? GetTask();
        public abstract GameStatistics GetStatistics();

        public virtual void Init(Game game)
        {
            this._login = game.Login;
            this._password = game.Password;
            Login();
        }

        public static IGameEngine Get(Game game)
        {
            var engine = game.Type switch
            {
                GameType.IgraLv => new IgraLvGameEngine(game) as IGameEngine,
                GameType.Demo => new DemoEnCxGameEngine(game),
                GameType.EnCx => new DemoEnCxGameEngine(game),
                GameType.LvlUp => new DemoEnCxGameEngine(game),
                _ => throw new NotImplementedException()
            };
            engine.Init(game);
            return engine;
        }
    }
}
