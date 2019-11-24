using System;
using System.Collections.Generic;
using System.Text;
using GC2.Classes;
using GC2DB.Data;

namespace GC2.Engines
{
    public class DemoEnCxGameEngine : IEnCxGameEngine
    {
        public override string MainUrlPart => "http://demo.en.cx";

        public override GameStatistics GetStatistics()
        {
            throw new NotImplementedException();
        }

        public DemoEnCxGameEngine(Game game)
        {
                
        }
    }
}
