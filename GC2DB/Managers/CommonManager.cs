using GC2DB.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace GC2DB.Managers
{
    public static class CommonManager
    {
        public static void LogError(Error e)
        {
            using var db = DBContext.Instance;
            db.Errors.Add(e);
        }
    }
}
