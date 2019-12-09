using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GC2DB.Data
{
    public class GcDbException
    {
        [Key]
        public int Id { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
    }
}
