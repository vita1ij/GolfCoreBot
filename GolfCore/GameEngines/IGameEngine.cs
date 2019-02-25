using GolfCore.Helpers;
using GolfCoreDB.Data;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace GolfCore.GameEngines
{
    public abstract class IGameEngine
    {
        public CookieCollection connectionCookie;
        public string ConnectionCookieName;

        public string LoginUrl;
        public string StatisticsUrl;
        public string TaskUrl { get; set; }
        public string GamesUrl;

        public string LoginPostData;

        public bool Login()
        {
            connectionCookie = WebConnectHelper.MakePost4Cookies(LoginUrl, LoginPostData);
            return (connectionCookie != null && connectionCookie.Count > 0);
        }

        public abstract string GetTask();
        public abstract Image<Rgba32> GetStatistics();

        public static IGameEngine Get(GameParticipant player)
        {
            var game = player.Game;
            return Get(game, player.ChatId);
        }

        public static IGameEngine Get(Game game, long chatId)
        {
            switch (game.Type)
            {
                case GameType.IgraLv:
                    return new IgraLvGameEngine(chatId);
                case GameType.EnCx:
                    return new EnCxQuestEngine(chatId);
                case GameType.Demo:
                    return new EnCxQuestEngine(chatId, "http://demo.en.cx");
                default:
                    throw new Exception("Wrong game mode");
            }
        }
    }
}
