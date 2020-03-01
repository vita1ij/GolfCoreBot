using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GC2;
using GC2DB.Data;
using GC2DB.Managers;
using GC2WH2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GC2WH2.Controllers
{
    public class MirrorController : Controller
    {
        public IActionResult Index(string id)
        {
            var model = new MirrorModel();
            if (!String.IsNullOrWhiteSpace(id))
            {
                Player player = GameManager.GetPlayerByMirrorId(id);
                if (player != null)
                {
                    model.Player = player;
                    model.Game = player.Game;
                    model.User = new GcUser()
                    {
                        TelegramUserId = player.ChatId
                    };

                }
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Game(GameInfo gi)
        {
            if (gi.Guid == null || gi.Key == null)
                return Index(null);
            if (!String.IsNullOrWhiteSpace(gi.Secret))
            {
                if (gi.Game == null)
                {
                    var games = GameManager.GetActiveGameByGuid(gi.Guid);
                    if (games != null && games.Count == 1)
                    {
                        gi.Game = games[0];
                    }
                    else
                    {
                        return View("Index", null);
                    }
                }
                ProcessingManager.CheckSecretForMirror(gi.Game, gi.Key, gi.Secret);

                if (gi.Tasks == null && gi.Game.LastTaskId.HasValue)
                {
                    var lastTask = GameManager.GetTaskById(gi.Game.LastTaskId.Value);
                    if (lastTask != null)
                    {
                        gi.Tasks = new List<GameTask>()
                        {
                            lastTask
                        };
                    }
                }
                return View("Game", gi);
            }
            else
            {
                return View("NoAuth", gi);
            }
            return Index(null);
        }

        public IActionResult Game(string id)
        {
            if (id == null)
            {
                Index(null);
            }
            else if (id.Length == 16)
            {
                GameInfo gi = ParseGameInfo(id);
                return View("NoAuth", gi);
            }

            return Index(null);
        }

        public GameInfo ParseGameInfo(string input)
        {
            GameInfo result = new GameInfo();

            if (input.Length != 16)
            {
                return null;
            }

            var parts = input.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2 || parts.Length > 3)
            {
                return null;
            }
            var games = GameManager.GetActiveGameByGuid(parts[0]);
            if (games == null || games.Count != 1)
            {
                return null;
            }
            else
            {
                result.Game = games[0];
            }
            if (!long.TryParse(parts[1], out var key))
            {
                return null;
            }
            if (parts.Length == 2)
            {
                result.Guid = parts[0];
                result.Key = key;
            }

            return result;
        }
    }
}