using GolfCore.Helpers;
using GolfCoreDB.Data;
using GolfCoreDB.Managers;
using HtmlAgilityPack;
using Microsoft.AspNetCore.WebUtilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace GolfCore.GameEngines
{
    public class EnCxEngine : IEnCxEngine
    {
        public EnCxEngine(long chatId, string url)
        {
            url = url.TrimEnd('/').TrimEnd('\\');
            using (var db = GolfCoreDB.DBContext.Instance)
            {
                //GameManager.GetAuthForActiveGame(chatId, out login, out pass);
                var game = GameManager.GetActiveGameByChatId(chatId);
                this.LoginPostData = $"socialAssign=0&Login={game.Login}&Password={game.Password}&EnButton1=Sign In&ddlNetwork=1";
                this.LoginUrl = $"{url}/Login.aspx";
                this.GamesUrl = $"{url}/GameCalendar.aspx";
                this.TaskUrl = $"{url}/gameengines/encounter/makefee/{game.EnCxId}";
                this.Login();
            }
        }

        public override string GetFullGamesLink(string? type, string? status, string? zone)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (type != null) parameters.Add("type", type);
            if (zone != null) parameters.Add("zone", zone);
            if (status != null) parameters.Add("status", status);
            return QueryHelpers.AddQueryString(GamesUrl, parameters);
        }

        public override List<string> GetAllGameZones()
        {
            var result = new List<string>()
            {
                "Real"
            };
            //var data = WebConnectHelper.MakePostWithoutCookies(GamesUrl);
            //HtmlDocument doc = new HtmlDocument();
            //doc.LoadHtml(data);
            //var allTypes = doc.DocumentNode.SelectNodes("//body//tr//div[@class='tabCal']//li[@id='liTab']//a[@id='lnkTab']");
            //if (allTypes == null) return result;
            //foreach (var type in allTypes)
            //{
            //    string href = type.GetAttributeValue("href", "");
            //    if (string.IsNullOrWhiteSpace(href) || href.ToLower().IndexOf("zone=") == -1) continue;
            //    var name = href.Substring(href.ToLower().IndexOf("zone=") + 5);
            //    if (name.IndexOf('&') > -1) name = name.Substring(0, name.IndexOf('&'));
            //    if (String.IsNullOrWhiteSpace(name)) continue;
            //    result.Add(name);
            //}
            return result;
        }

        public override List<Game> GetEnCxGames(string? type, string? status, string? zone, out List<(string,string)> pages)
        {
            List<Game> result = new List<Game>();
            string url = GetFullGamesLink(type, status, zone);
        
            var data = WebConnectHelper.MakePostWithoutCookies(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);
            var allGames = doc.DocumentNode.SelectNodes("//td[@class='infoCell'][5]//a");
            for (int i = 1; i<allGames.Count; i++)
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
            pages = new List<(string, string)>();
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

        public override bool IsLoginPage(string data)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);
            return doc.DocumentNode.SelectNodes("//input[@id='txtLogin']") != null && doc.DocumentNode.SelectNodes("//input[@id='txtPassword']") != null;
        }

        public override string? GetTask()
        {
            ConnectionCookie = null;
            Login();
            if (TaskUrl == null) return null;
            if (ConnectionCookie == null)
            {
                if (LoginUrl == null || LoginPostData == null) return null;
                ConnectionCookie = WebConnectHelper.MakePost4Cookies(LoginUrl, LoginPostData);
                if (ConnectionCookie == null) return null;
            }
            var data = WebConnectHelper.MakePostWithCookies(TaskUrl, ConnectionCookie);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);
            
            string taskContent = doc.DocumentNode.SelectNodes("//div[@class='content']")[0].InnerText;

            return taskContent;
        }

        public override Image<Rgba32>? GetStatistics()
        {
            throw new NotImplementedException();
        }

        public override bool EnterCode(string code)
        {
            throw new NotImplementedException();
        }
    }
}
