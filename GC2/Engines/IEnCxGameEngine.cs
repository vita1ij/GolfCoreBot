using GC2.Helpers;
using GC2DB.Data;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Text;

namespace GC2.Engines
{
    public abstract class IEnCxGameEngine : IGameEngine
    {
        protected string? _enCxId;
        public abstract string MainUrlPart { get; }
        public string GamesCalendarUrl { get => $"{MainUrlPart}/GameCalendar.aspx"; }

        public override string LoginUrl { get => $"{MainUrlPart}/Login.aspx"; }
        public override string LoginPostData { get => $"socialAssign=0&Login={_login}&Password={_password}&EnButton1=Sign In&ddlNetwork=1"; }
        public override string TaskUrl { get => $"{MainUrlPart}/gameengines/encounter/makefee/{_enCxId}"; }

        public override void Init(Game game)
        {
            this._login = game.Login;
            this._password = game.Password;
            this._enCxId = game.EnCxId;
            Login();
        }

        public List<Game> GetEnCxGames(string zone, string status, string type)
        {
            List<Game> result = new List<Game>();
            string url = $"{GamesCalendarUrl}?type={type}&status={status}&zone={zone}";

            var data = WebConnectHelper.MakePost(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);
            var allGames = doc.DocumentNode.SelectNodes("//td[@class='infoCell'][5]//a");
            for (int i = 1; i < allGames.Count; i++)
            {
                var newGame = new Game()
                {
                    Href = allGames[i].GetAttributeValue("href", ""),
                    Title = allGames[i].InnerText
                };
                newGame.EnCxId = (String.IsNullOrEmpty(newGame.Href)) ? "" : newGame.Href.Substring(newGame.Href.IndexOf("gid=") + 4);
                newGame.EnCxId = (newGame.EnCxId.IndexOf('=') == -1) ? newGame.EnCxId : newGame.EnCxId.Substring(0, newGame.EnCxId.IndexOf('='));
                result.Add(newGame);
            }

            var pageNodes = doc.DocumentNode.SelectNodes("//table[@class='tabCalContainer']//table//div//a");
            var pages = new List<(string, string)>();
            if (pageNodes != null)
            {
                foreach (var p in pageNodes)
                {
                    pages.Add((p.Attributes["href"].ToString(), p.InnerText));
                }
            }
            //            doc.DocumentNode.SelectNodes("//table[@class='tabCalContainer']//table//div//a")[0].InnerText
            //"2"
            //doc.DocumentNode.SelectNodes("//table[@class='tabCalContainer']//table//div//a")[0].Attributes["href"]
            return result;
        }

        public override string GetTask()
        {
            Login();
            var data = WebConnectHelper.MakePost(TaskUrl, ConnectionCookie);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);

            string taskContent = doc.DocumentNode.SelectNodes("//div[@class='content']")[0].InnerText;

            return taskContent;
        }
    }
}
