using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GC2DB.Data
{
    public class PlayersLocation
    {
        [Key]
        [Required]
        public long Id { get; set; }
        public long ChatId { get; set; }
        public virtual Location Location { get; set; }
        public string? Title { get; set; }
    }
}
