using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GC2WH.Models
{
    public class StatusModel
    {
        //public bool BotWebsiteOnline { get; set; }
        public bool BotWebhookSet { get; set; }
        [Display(Name = "Locations last update time")]
        public DateTime BotLocationDbLastUpdated { get; set; }
        public bool CanStartBot { get; set; }
        public bool CanStopBot { get; set; }

        public long PendingUpdates { get; set; }
        public DateTime LastErrorDate{ get; set; }
        public string LastErrorMessage { get; set; }
        public long MaxConnections { get; set; }
        public string AllowedUpdates { get; set; }

        public TestsClass Tests { get; set; } = new TestsClass();
        public class TestsClass
        {
            public string DBConnection { get; set; }
        }
        
    }
}
