using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GolfCoreDB.Data
{
    public class GameParticipant
    {
        [Key]
        public long Id { get; set; }
        public Game Game { get; set; }
        public long ChatId { get; set; }
        public bool MonitorUpdates { get; set; }
        public bool GetUpdates { get; set; }
        public bool MonitorStatistics { get; set; }
        public bool GetStatistics { get; set; }

        public int TaskMonitoring
        {
            get
            {
                if (!MonitorUpdates) return 0;
                return GetUpdates ? 2 : 1;
            }
        }

    }
}
