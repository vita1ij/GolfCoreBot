﻿using System;
using System.Collections.Generic;
using System.Text;
using GC2.Classes;
using GC2DB.Data;

namespace GC2.Engines
{
    public class DemoEnCxGameEngine : IEnCxGameEngine
    {
        public override string MainUrlPart => "http://demo.en.cx";

        public override bool EnterCode(string code)
        {
            throw new NotImplementedException();
        }

        public override GameStatistics GetStatistics()
        {
            throw new NotImplementedException();
        }

        public override bool IsLoginPage(string data)
        {
            throw new NotImplementedException();
        }

        public DemoEnCxGameEngine(Game game)
        {
                
        }
    }
}