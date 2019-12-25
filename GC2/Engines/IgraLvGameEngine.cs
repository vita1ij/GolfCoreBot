using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GC2.Classes;
using GC2.Helpers;
using GC2DB;
using GC2DB.Data;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace GC2.Engines
{
    public class IgraLvGameEngine : IGameEngine
    {
        #region Properties

        public override string LoginUrl
        {
            get => "https://igra.lv/igra.php?s=login";
        }
        public override List<KeyValuePair<string, string>>? LoginPostValues
        {
            get => (String.IsNullOrWhiteSpace(_login) || String.IsNullOrWhiteSpace(_password))
                ? null
                : new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("login", _login),
                    new KeyValuePair<string, string>("password", _password)
                };
        }
        public override string TaskUrl
        {
            get => "https://www.igra.lv/igra.php";
        }
        #endregion

        public override void Init(Game game)
        {
            _login = game.Login ?? String.Empty;
            _password = game.Password ?? String.Empty;
            Login(game);
        }

        public IgraLvGameEngine(Game game)
        {
            Init(game);
        }

        public IgraLvGameEngine(long chatId)
        {
            using var db = DBContext.Instance;
            var game = db.Players.Where(x => x.IsActive && x.ChatId == chatId)
                .Include(x => x.Game)
                .First()
                .Game;
            if (game != null && game.isActive)
            {
                Init(game);
            }
        }

        public override bool? EnterCode(string code, Game game)
        {
            return null;
        }

        public override GameStatistics GetStatistics()
        {
            throw new NotImplementedException();
        }

        public override GameTask? GetTask(out List<object>? stuff)
        {
            stuff = null;
            try
            {
                if (TaskUrl == null) return null;
                if (ConnectionCookie == null)
                {
                    if (LoginUrl == null || LoginPostValues == null) return null;
                    ConnectionCookie = WebConnectHelper.MakePost4Cookies(LoginUrl, LoginPostValues);
                    if (ConnectionCookie == null) return null;
                }
                else
                {
                    //fix    Expires: {1/1/0001 12:00:00 AM}
                    ConnectionCookie["agt_session"].Expires = DateTime.MinValue;
                }
                var data = WebConnectHelper.MakeGetPost(TaskUrl, ConnectionCookie);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(data);
                string taskContent = (doc.GetElementbyId("general-puzzle") ?? doc.GetElementbyId("general")).InnerText;

                return new GameTask(null, taskContent);
            }
            catch (Exception ex)
            {
                Log.New(ex);
                return null;
            }
        }

        public override bool IsLoginPage(HtmlDocument doc)
            =>
            (
                (doc.DocumentNode?.SelectNodes("//input[@name='login']")?.Any() ?? false) &&
                (doc.DocumentNode?.SelectNodes("//input[@name='password']")?.Any() ?? false)
            );
    }
}
