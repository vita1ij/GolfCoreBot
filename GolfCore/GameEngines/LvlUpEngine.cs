using System;
using System.Collections.Generic;
using System.Text;
using GolfCore.Helpers;
using GolfCoreDB.Managers;
using HtmlAgilityPack;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GolfCore.GameEngines
{
    public class LvlUpEngine : IGameEngine
    {
        public LvlUpEngine(long chatId)
        {
            GameManager.GetAuthForActiveGame(chatId, out string? login, out string? pass);
            this.LoginPostData = $"_formname=login&next=//&password={pass ?? ""}&username ={login ?? ""}";
            this.LoginUrl = "http://game.lvlup.lv/user/login";
            this.TaskUrl = "http://game.lvlup.lv/system/game";
            //base.StatisticsUrl = "http://igra.lv/img_level_times.php";
            this.Login();
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

            string taskContent = doc.DocumentNode.SelectNodes("//div[@class='container-fluid main-container']")[0].InnerText; //ytest

            return taskContent;
        }

        public override bool EnterCode(string code)
        {
            throw new NotImplementedException();
        }

        public override Image<Rgba32> GetStatistics()
        {
            throw new NotImplementedException();
        }

        public override bool IsLoginPage(string data)
        {
            throw new NotImplementedException();
        }
    }
}
