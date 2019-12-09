using System;
using System.Collections.Generic;
using System.Text;

namespace GC2.Constants
{
    public static class Exceptions
    {
        public enum ExceptionCode
        {
            UniqueGame4Chat
            ,NoActiveGame,
            Other
        }

        public static readonly Dictionary<ExceptionCode, string> CodeMessages = new Dictionary<ExceptionCode, string>
        {
            {ExceptionCode.UniqueGame4Chat , "There can be only one active game in chat"}
            ,{ExceptionCode.NoActiveGame, "You need to be in active game for that!"}
        };
    }
}
