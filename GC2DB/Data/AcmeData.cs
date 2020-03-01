using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GC2DB.Data
{
    public class AcmeData
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
