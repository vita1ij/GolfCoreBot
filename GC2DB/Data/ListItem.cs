using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GC2DB.Data
{
    public class ListItem
    {
        [Key]
        [Required]
        public long Id { get; set; }
        public long ChatId { get; set; }
        public string Text { get; set; }

        public ListItem(long chatId, string text)
        {
            this.ChatId = chatId;
            this.Text = text;
        }
    }
}
