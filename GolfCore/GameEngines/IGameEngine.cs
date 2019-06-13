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
        public CookieCollection? ConnectionCookie { get; set; }
        public string? ConnectionCookieName { get; set; }
        public string? LoginUrl { get; set; }
        public string? StatisticsUrl { get; set; }
        public string? TaskUrl { get; set; }
        public string? LoginPostData { get; set; }

        public bool Login()
        {
            if (LoginUrl == null || LoginPostData == null) return false;
            ConnectionCookie = WebConnectHelper.MakePost4Cookies(LoginUrl, LoginPostData);
            return (ConnectionCookie != null && ConnectionCookie.Count > 0);
        }

        public abstract bool IsLoginPage(string data);

        public abstract bool EnterCode(string code);

        public abstract string? GetTask();
        public abstract Image<Rgba32>? GetStatistics();

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
                    return new EncxDemoEngine(chatId);
                case GameType.LvlUp:
                    return new LvlUpEngine(chatId);
                default:
                    throw new Exception("Wrong game mode");
            }
        }
    }
}
