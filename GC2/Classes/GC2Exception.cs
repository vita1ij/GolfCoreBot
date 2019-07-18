using System;
using System.Collections.Generic;
using System.Text;

namespace GC2
{
    public class GCException : Exception
    {
        public enum LevelType
        {
            Fatal,
            Chat,
            ChatFull,
            Log,
            Quiet
        }

        public string Code { get; set; }
        public LevelType Level { get; set; }
        public GCException(Constants.Exceptions.ExceptionCode code, LevelType lvl = LevelType.Log)
            :base(Constants.Exceptions.CodeMessages[code])
        {
            this.Code = code.ToString();
            this.Level = lvl;
        }
    }
}
