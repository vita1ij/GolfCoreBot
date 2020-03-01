using GC2.Classes;
using GC2DB.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GC2.Engines
{
    public class CustomEnCxGameEngine : IEnCxGameEngine
    {
        private string mainUrlPart = "";
        public override string MainUrlPart
        {
            get
            {
                return mainUrlPart;
            }
        }

        public override GameStatistics? GetStatistics()
        {
            return null;
        }

        public CustomEnCxGameEngine(Game game)
        {
            if (game.CustomEnCxDomain == null)
            {
                Log.New("No Custom domain");
                game.CustomEnCxDomain = "demo.en.cx";
            }
            mainUrlPart = game.CustomEnCxDomain;
        }
    }
}
