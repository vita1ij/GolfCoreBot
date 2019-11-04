﻿using GC2.Classes;
using GC2.Helpers;
using GC2DB.Data;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GC2.Engines
{
    public abstract class IEnCxGameEngine : IGameEngine
    {
        protected string _enCxId;
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

        public override string GetTask(out List<object> stuff)
        {
            Login();
            var data = WebConnectHelper.MakePost(TaskUrl, ConnectionCookie);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);

            var taskNodes = doc.DocumentNode?.SelectNodes("//div[@class='content']");
            string taskContent = FormatTask(taskNodes, out var imageResults);
            stuff = imageResults.Select(x => x as object).ToList();
            return taskContent;
        }

        public string FormatTask(HtmlNodeCollection input, out List<ImageResult> images)
            => FormatTask(input, 0, out images);

        public string FormatTask(HtmlNodeCollection input, long imgSeed, out List<ImageResult> images, bool canUseTags = true)
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
                        result += FormatTask(node.ChildNodes, imgSeed, out childImages, canUseTags);
                        result += $"\r\n";
                        imgSeed += (childImages ?? new List<ImageResult>()).Count;
                        images.AddRange(childImages);
                        continue;
                    //ignore
                    case "span":
                        break;
                    //Task Title
                    case "h2":
                        childImages = new List<ImageResult>();
                        result += $"\r\n {(canUseTags?"<b>":"")}{FormatTask(node.ChildNodes, imgSeed, out childImages, false).Trim()}{(canUseTags ? "</b>" : "")} \r\n";
                        imgSeed += (childImages ?? new List<ImageResult>()).Count;
                        images.AddRange(childImages);
                        continue;
                    //Important paragraphs
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6":
                        childImages = new List<ImageResult>();
                        result += $"\r\n {(canUseTags ? "<b>" : "")}{FormatTask(node.ChildNodes, imgSeed, out childImages, false).Trim()}{(canUseTags ? "</b>" : "")} \r\n";
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
                            if (canUseTags)
                            {
                                //  <a href="URL">inline URL</a>
                                result += $"<a href=\"{node.Attributes["href"].Value}\">{node.InnerText} |({GetFilename(node.Attributes["href"].Value)})</a>";
                            }
                            else
                            {
                                if (!String.IsNullOrWhiteSpace(node.InnerText))
                                {
                                    result += $"[Link] {node.InnerText} | {node.Attributes["href"].Value} [/Link]";
                                }
                                else
                                {
                                    result += node.Attributes["href"].Value;
                                }
                            }
                        }
                        
                        break;
                    case "img":
                        if (node?.Attributes?.Contains("src") ?? false)
                        {
                            if (canUseTags)
                            {
                                imgSeed++;
                                result += $"<a href=\"{node.Attributes["src"].Value}\">[IMG#{imgSeed}]{node.InnerText} {(node.Attributes.Contains("title")?"|"+node.Attributes["title"].Value:"")}{(node.Attributes.Contains("alt") ? "|" + node.Attributes["alt"].Value : "")}|({GetFilename(node.Attributes["src"].Value)})</a>";
                            }
                            else
                            {
                                imgSeed++;
                                result += $"[IMG#{imgSeed}]{GetFilename(node.Attributes["src"].Value)}[/IMG]";
                            }
                            images.Add(new ImageResult()
                            {
                                Url = node.Attributes["src"].Value,
                                ReferenceName = $"IMG#{imgSeed}"
                            }); 
                        }
                        break;
                    default:
                        result += $"{node.InnerText}";
                        break;
                }
                result += FormatTask(node.ChildNodes, imgSeed, out var newImgs, canUseTags)?.Trim() ?? "";
                images.AddRange(newImgs);
                imgSeed += (newImgs ?? new List<ImageResult>()).Count;
            }

            result = result.Replace("\t", " ");
            while (result.Contains("  "))
            {
                result = result.Replace("  ", " ");
            }
            if (String.IsNullOrWhiteSpace(result)) return null;
            var lines = result.Split("\r\n").ToList();
            lines = lines?.Where(x => !String.IsNullOrWhiteSpace(x))?.ToList();
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
    }
}
