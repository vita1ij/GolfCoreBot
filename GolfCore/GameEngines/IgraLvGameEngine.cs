using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using GolfCoreDB.Managers;
using HtmlAgilityPack;
using GolfCore.Helpers;

namespace GolfCore.GameEngines
{
    public class IgraLvGameEngine : IGameEngine
    {
        public IgraLvGameEngine(string gameId)
        {
            
        }

        public IgraLvGameEngine(long chatId)
        {
            string login, pass;
            using (var db = GolfCoreDB.DBContext.Instance)
            {
                GameManager.GetAuthForActiveGame(chatId, out login, out pass);
                this.LoginPostData = String.Format("login={0}&password={1}", login, pass);
                this.LoginUrl = "http://igra.lv/igra.php?s=login";
                this.TaskUrl = "http://www.igra.lv/igra.php";
                this.Login();
            }
        }

        public override string GetStatistics()
        {
            throw new NotImplementedException();
        }

        public override string GetTask()
        {
            var data = WebConnectHelper.MakePostWithCookies(TaskUrl, this.connectionCookie);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(data);
            string taskContent = (doc.GetElementbyId("general-puzzle") ?? doc.GetElementbyId("general")).InnerText;

            return taskContent;
        }
    }
}
