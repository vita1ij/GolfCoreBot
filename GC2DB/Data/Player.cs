﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GC2DB.Data
{
    public class Player
    {
        [Key]
        [Required]
        public long Id { get; set; }
        public virtual Game Game { get; set; }
        public long ChatId { get; set; }
        public bool IsActive { get; set; }
        public PlayerUpdateStatusInfo UpdateTaskInfo { get; set; } = 0; //0/1/2
        public PlayerUpdateStatusInfo UpdateStatisticsInfo { get; set; } = 0;
        public string MirrorId { get; set; }
        public string MirrorPass { get; set; }

        public Player(Game game, long chatId)
        {
            this.Game = game;
            this.ChatId = chatId;

            var gid = Guid.NewGuid().ToString().Replace("-","");
            this.MirrorId = gid[..8];
            this.MirrorPass = gid[^8..];
        }

        public enum PlayerUpdateStatusInfo
        {
            DontUpdate = 0,
            UpdateStatus = 1,
            UpdateText = 2
        }
        /// <summary>
        /// only for EF. Do not use it
        /// </summary>
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public Player()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        {
        }
    }
}
