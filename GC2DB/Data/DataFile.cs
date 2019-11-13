using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GC2DB.Data
{
    public class DataFile
    {
        [Key]
        [Required]
        public long Id { get; set; }
        public byte[] Content { get; set; }
        public virtual Location Location { get; set; }
        public virtual GameTask Task { get; set; }
        public long? ChatId { get; set; }
    }
}
