using GolfCore.Helpers;
using GolfCoreDB.Data;
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
        public string TaskUrl { get; set; }
        public string GamesUrl;

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

        public static IGameEngine Get(GameParticipant player)
        {
            var game = player.Game;
            switch(game.Type)
            {
                case GameType.IgraLv:
                    return new IgraLvGameEngine(player.ChatId);
                case GameType.EnCx:
                    return new EnCxQuestEngine(player.ChatId);
                default:
                    return null;
            }
        }

        public static IGameEngine Get(Game game, long chatId)
        {
            switch (game.Type)
            {
                case GameType.IgraLv:
                    return new IgraLvGameEngine(chatId);
                case GameType.EnCx:
                    return new EnCxQuestEngine(chatId);
                default:
                    return null;
            }
        }
    }
}
