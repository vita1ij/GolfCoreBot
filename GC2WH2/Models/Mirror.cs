using GC2DB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GC2WH2.Models
{
    public class MirrorModel
    {
        public GcUser User { get; set; }
        public Game Game { get; set; }
        public Player Player { get; set; }
    }
}
