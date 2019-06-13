using System;
using System.Collections.Generic;
using System.Text;

namespace GolfCore.GameEngines
{
    public class EncxDemoEngine : EnCxEngine
    {
        public EncxDemoEngine(long chatId)
            : base(chatId, "http://demo.en.cx")
        {

        }
    }
}
