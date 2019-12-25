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

namespace GC2.Engines
{
    public abstract class IEnCxGameEngine : IGameEngine
    {
        protected string? _enCxId;
        public abstract string MainUrlPart { get; }
        public string GamesCalendarUrl { get => $"{MainUrlPart}/GameCalendar.aspx"; }

        public override string LoginUrl { get => $"{MainUrlPart}/Login.aspx"; }
        public override List<KeyValuePair<string, string>>? LoginPostValues
        {
            get =>
                (String.IsNullOrWhiteSpace(_login) || String.IsNullOrWhiteSpace(_password))
                ? null
                : new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("socialAssign","0"),
                    new KeyValuePair<string, string>("Login",_login),
                    new KeyValuePair<string, string>("Password",_password),
                    new KeyValuePair<string, string>("EnButton1","Sign In"),
                    new KeyValuePair<string, string>("ddlNetwork","1"),
                };
        }
        public override string TaskUrl { get => $"{MainUrlPart}/gameengines/encounter/makefee/{_enCxId}"; }
        public string CodeUrl { get => $"{MainUrlPart}/gameengines/encounter/play/{_enCxId}"; }

        public override void Init(Game game)
        {
            this._login = game.Login ?? String.Empty;
            this._password = game.Password ?? String.Empty;
            this._enCxId = game.EnCxId;
            Login(game);
        }

        public List<Game> GetEnCxGames(string zone, string status, string type)
        {
            List<Game> result = new List<Game>();
            string url = $"{GamesCalendarUrl}?type={type}&status={status}&zone={zone}";

            var data = WebConnectHelper.MakeGetPost(url, null);
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
                    pages.Add((p.Attributes["href"].ToString() ?? String.Empty, p.InnerText));
                }
            }
            //            doc.DocumentNode.SelectNodes("//table[@class='tabCalContainer']//table//div//a")[0].InnerText
            //"2"
            //doc.DocumentNode.SelectNodes("//table[@class='tabCalContainer']//table//div//a")[0].Attributes["href"]
            return result;
        }

        public override bool IsLoginPage(HtmlDocument doc)
            =>
            (
                (doc.DocumentNode?.SelectNodes("//input[@id='txtLogin']")?.Any() ?? false) &&
                (doc.DocumentNode?.SelectNodes("//input[@id='txtPassword']")?.Any() ?? false)
            );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stuff"></param>
        /// <returns></returns>
        /// <exception cref="GCException" />
        public override GameTask? GetTask(out List<object> stuff)
        {
            stuff = new List<object>();
            var data = WebConnectHelper.MakeGetPost(TaskUrl, ConnectionCookie);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);

            if (IsLoginPage(doc))
            {
                throw new GCException(Constants.Exceptions.ExceptionCode.CantLogIn, GCException.LevelType.Chat);
            }

            var taskNodes = doc.DocumentNode?.SelectNodes("//div[@class='content']");
            string? taskContent = FormatTask(taskNodes, out var imageResults);
            if (taskContent == null) return null;
            stuff = imageResults.Select(x => x as object).ToList();
            string? levelId = doc.DocumentNode?.SelectNodes("//input[@name='LevelId']")?.First()?.GetAttributeValue("Value", null);
            string? levelNumberStr = doc.DocumentNode?.SelectNodes("//input[@name='LevelNumber']")?.First()?.GetAttributeValue("Value", null);
            long? levelNumber = null;
            if (long.TryParse(levelNumberStr, out var lvlnum))
            {
                levelNumber = lvlnum;
            }
            return new GameTask(taskContent)
            {
                EnCxId = levelId,
                Number = levelNumber
            };
        }

        public string? FormatTask(HtmlNodeCollection input, out List<ImageResult> images)
            => FormatTask(input, 0, out images, false, false);

        public string? FormatTask(HtmlNodeCollection input, long imgSeed, out List<ImageResult> images, bool openBTag, bool openITag)
        {
            images = new List<ImageResult>();

            var childImages = new List<ImageResult>();

            if (input == null) return null;
            string result = "";
            foreach(var node in input)
            {
                switch (node.Name.ToLower())
                {
                    //add paragraph
                    case "div":
                    case "p":
                        childImages = new List<ImageResult>();
                        result += $"\r\n";
                        result += FormatTask(node.ChildNodes, imgSeed, out childImages, openBTag, openITag);
                        result += $"\r\n";
                        imgSeed += (childImages ?? new List<ImageResult>()).Count;
                        images.AddRange(childImages);
                        continue;
                    //ignore
                    case "span":
                        break;
                    //Task Title
                    case "h2":
                    //Important paragraphs
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6":
                        childImages = new List<ImageResult>();
                        result += $"\r\n {(openITag?"</i>":"")}{(openBTag ? "": "<b>")}{FormatTask(node.ChildNodes, imgSeed, out childImages, true, openITag)?.Trim() ?? String.Empty}{(openBTag ? "" : "</b>")}{(openITag ? "<i>" : "")} \r\n";
                        imgSeed += (childImages ?? new List<ImageResult>()).Count;
                        images.AddRange(childImages);
                        continue;
                    //Line break
                    case "br":
                        result += "\r\n";
                        break;
                    //Images
                    case "a":
                        if (node?.Attributes?.Contains("href") ?? false)
                        {
                            if (openITag) result += "</i>";
                            if (openBTag) result += "</b>";
                            result += $"<a href=\"{node.Attributes["href"].Value}\">{node.InnerText} |({GetFilename(node.Attributes["href"].Value)})</a>";
                            if (openITag) result += "<i>";
                            if (openBTag) result += "<b>";
                        }
                        break;
                    case "img":
                        if (node?.Attributes?.Contains("src") ?? false)
                        {
                            if (openITag) result += "</i>";
                            if (openBTag) result += "</b>";
                            imgSeed++;
                            result += $"<a href=\"{node.Attributes["src"].Value}\">[IMG#{imgSeed}]{node.InnerText} {(node.Attributes.Contains("title") ? "|" + node.Attributes["title"].Value : "")}{(node.Attributes.Contains("alt") ? "|" + node.Attributes["alt"].Value : "")}|({GetFilename(node.Attributes["src"].Value)})</a>";
                            if (openITag) result += "<i>";
                            if (openBTag) result += "<b>";
                            
                            images.Add(new ImageResult()
                            {
                                Url = node.Attributes["src"].Value,
                                ReferenceName = $"IMG#{imgSeed}"
                            }); 
                        }
                        break;
                    case "script":
                        break;
                    default:
                        result += $"{node.InnerText}";
                        break;
                }
                if (node?.ChildNodes != null)
                {
                    result += FormatTask(node.ChildNodes, imgSeed, out var newImgs, openBTag, openITag)?.Trim() ?? "";
                    images.AddRange(newImgs);
                    imgSeed += (newImgs ?? new List<ImageResult>()).Count;
                }
            }

            result = result.Replace("\t", " ");
            while (result.Contains("  "))
            {
                result = result.Replace("  ", " ");
            }
            if (String.IsNullOrWhiteSpace(result)) return null;
            var lines = result.Split("\r\n").ToList();
            lines = lines?.Where(x => !String.IsNullOrWhiteSpace(x))?.ToList() ?? new List<string>();
            if (lines == null || !lines.Any()) return null;

            return String.Join("\r\n", lines);
        }

        public string GetFilename(string input)
        {
            if (String.IsNullOrWhiteSpace(input)) return "";
            var lines = input.Split(new char[]{ '\\','/'}, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Any()) return lines.Last();
            return "";
        }

        public override bool? EnterCode(string code, Game game)
        {
            if (ConnectionCookie != null && ConnectionCookie.Count>0)
            {
                bool? result = postCode(ConnectionCookie, code, game);
                if (result.HasValue)
                {
                    return result.Value;
                }
            }
            
            return null;
        }

        private bool? postCode(CookieCollection connectionCookie, string code, Game game)
        {
            if (!game.LastTaskId.HasValue) return null;
            var task = GameManager.GetTaskById(game.LastTaskId.Value);

            if (String.IsNullOrWhiteSpace(task?.EnCxId) || !task.Number.HasValue) return null;

            var resultContents = WebConnectHelper.MakeGetPost(CodeUrl, connectionCookie, new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("LevelId", task.EnCxId),
                new KeyValuePair<string, string>("LevelNumber", task.Number.Value.ToString()),
                new KeyValuePair<string, string>("LevelAction.Answer",code)
            });

            //process result

            return null;
        }
    }
}
