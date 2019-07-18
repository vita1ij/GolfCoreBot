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
            get => "http://igra.lv/igra.php?s=login";
        }
        public override string LoginPostData
        {
            get => $"login={_login ?? ""}&password={_password ?? ""}";
        }
        public override string TaskUrl
        {
            get => "http://www.igra.lv/igra.php";
        }
        #endregion

        public void Init(Game game)
        {
            _login = game.Login;
            _password = game.Password;
            Login();
        }

        public IgraLvGameEngine(Game game)
        {
            Init(game);
        }

        public IgraLvGameEngine(long chatId)
        {
            using (var db = DBContext.Instance)
            {
                var game = db.Players.Where(x => x.isActive && x.ChatId == chatId)
                    .Include(x => x.Game)
                    .First()
                    .Game;
                if (game != null && game.isActive)
                {
                    Init(game);
                }
            }
        }

        public override bool EnterCode(string code)
        {
            throw new NotImplementedException();
        }

        public override GameStatistics GetStatistics()
        {
            throw new NotImplementedException();
        }

        public override string? GetTask()
        {
            try
            {
                if (TaskUrl == null) return null;
                if (ConnectionCookie == null)
                {
                    if (LoginUrl == null || LoginPostData == null) return null;
                    ConnectionCookie = WebConnectHelper.MakePost4Cookies(LoginUrl, LoginPostData);
                    if (ConnectionCookie == null) return null;
                }
                else
                {
                    //fix    Expires: {1/1/0001 12:00:00 AM}
                    ConnectionCookie["agt_session"].Expires = DateTime.MinValue;
                }
                var data = WebConnectHelper.MakePost(TaskUrl, ConnectionCookie);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(data);
                string taskContent = (doc.GetElementbyId("general-puzzle") ?? doc.GetElementbyId("general")).InnerText;

                return taskContent;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override bool IsLoginPage(string data)
        {
            throw new NotImplementedException();
        }
    }
}
