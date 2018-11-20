using GolfCore.Helpers;
using GolfCoreDB.Data;
using GolfCoreDB.Managers;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace GolfCore.GameEngines
{
    public class EnCxQuestEngine : IGameEngine
    {
        public const string HTMLPATH_ACTIVE_GAMES = "boxCenterActiveGames";
        public const string HTMLPATH_ACTIVE_GAME = "div.boxGameInfo table.gameInfo table tr::first td table ";
        public string TaskUrl = "";

        public EnCxQuestEngine(long chatId)
        {
            string login, pass;
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

        public override string GetStatistics()
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
