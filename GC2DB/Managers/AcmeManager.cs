using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GC2DB.Managers
{
    public static class AcmeManager
    {
        public static void Create(string key, string value)
        {
            var db = DBContext.Instance;
            db.AcmeValues.Add(new Data.AcmeData() 
            {
                Key =key,
                Value = value
            });
            db.SaveChanges();
        }

        public static string Get(string key)
        {
            var db = DBContext.Instance;
            return db.AcmeValues?.Where(x => x.Key == key)?.FirstOrDefault()?.Value ?? String.Empty;
        }
    }
}
