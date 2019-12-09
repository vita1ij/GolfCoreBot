using GC2.Classes;
using GC2DB.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GC2.Engines
{
    public class QuestEnCxGameEngine : IEnCxGameEngine
    {
        public override string MainUrlPart => "http://quest.en.cx";

        public override GameStatistics GetStatistics()
        {
            return null;
        }

        public QuestEnCxGameEngine(Game game)
        {

        }
    }
}
