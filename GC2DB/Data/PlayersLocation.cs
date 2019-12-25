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
        public virtual Location Loc { get; set; }
        public string? Title { get; set; }

        public PlayersLocation(long chatId, Location location)
        {
            Loc = location;
            ChatId = ChatId;
        }

        /// <summary>
        /// EF. Do not use
        /// </summary>
        [Obsolete]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public PlayersLocation()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {

        }
    }
}
