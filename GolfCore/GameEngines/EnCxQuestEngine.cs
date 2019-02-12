using GolfCore.Helpers;
using GolfCoreDB.Data;
using GolfCoreDB.Managers;
using HtmlAgilityPack;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GolfCore.GameEngines
{
    public class EnCxQuestEngine : IGameEngine
    {
        public const string HTMLPATH_ACTIVE_GAMES = "boxCenterActiveGames";
        public const string HTMLPATH_ACTIVE_GAME = "div.boxGameInfo table.gameInfo table tr::first td table ";
#pragma warning disable CS0108 // 'EnCxQuestEngine.TaskUrl' hides inherited member 'IGameEngine.TaskUrl'. Use the new keyword if hiding was intended.
        public string TaskUrl = "";
#pragma warning restore CS0108 // 'EnCxQuestEngine.TaskUrl' hides inherited member 'IGameEngine.TaskUrl'. Use the new keyword if hiding was intended.

        public EnCxQuestEngine(long chatId)
        {
#pragma warning disable CS0168 // The variable 'pass' is declared but never used
#pragma warning disable CS0168 // The variable 'login' is declared but never used
            string login, pass;
#pragma warning restore CS0168 // The variable 'login' is declared but never used
#pragma warning restore CS0168 // The variable 'pass' is declared but never used
            using (var db = GolfCoreDB.DBContext.Instance)
            {
                //GameManager.GetAuthForActiveGame(chatId, out login, out pass);
                var game = GameManager.GetActiveGameByChatId(chatId);
                this.LoginPostData = String.Format("socialAssign=0&Login={0}&Password={1}&EnButton1=Sign In&ddlNetwork=1", game.Login, game.Password);
                this.LoginUrl = "http://quest.en.cx/Login.aspx";
                this.GamesUrl = "http://quest.en.cx";
                this.TaskUrl = String.Format("/gameengines/encounter/play/{0}",game.EnCxId);
                this.Login();
            }
        }

        public override Image<Rgba32> GetStatistics()
        {
            throw new NotImplementedException();
        }

        public override string GetTask()
        {
            throw new NotImplementedException();
        }

        public List<Game> GetAllEnCxActiveGames()
        {
            List<Game> result = new List<Game>();

            var data = WebConnectHelper.MakePostWithCookies(GamesUrl, connectionCookie);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);
            HtmlNode allGamesHtml = doc.GetElementbyId(HTMLPATH_ACTIVE_GAMES);
            //boxGameInfo
            //foreach (var child in allGamesHtml.ChildNodes)
            //{
            //    if (child.Name == "div" && child.HasClass("boxGameInfo"))
            //    {
                    //this is active game block
            var gameInfoSpan = allGamesHtml.SelectNodes("//a[@id='lnkGameTitle']");

            foreach (var ga in gameInfoSpan)
            {
                var newGame = new Game()
                {
                    Href = ga.GetAttributeValue("href", ""),
                    Title = ga.InnerText
                };
                newGame.EnCxId = (String.IsNullOrEmpty(newGame.Href)) ? "" : newGame.Href.Substring(newGame.Href.IndexOf('=') + 1);
                result.Add(newGame);
            }

            return result;
        }
    }
}
