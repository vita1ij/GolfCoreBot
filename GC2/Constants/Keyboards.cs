using GC2DB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

namespace GC2.Constants
{
    public static class Keyboards
    {
        public static InlineKeyboardMarkup CoordinatesKeyboard(Coordinates coordinates)
        {
            return new InlineKeyboardMarkup(new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Google", Url=$"https://www.google.com/maps/search/?api=1&query={coordinates.Lat},{coordinates.Lon}"},
                new InlineKeyboardButton() { Text = "Waze", Url = $"https://www.waze.com/ul?ll={coordinates.Lat},{coordinates.Lon}&navigate=yes"},
                new InlineKeyboardButton() { Text = "Save", CallbackData=$"/savelocation {coordinates.Lat},{coordinates.Lon}" }
            });
        }

        public static InlineKeyboardMarkup CreateNewGame = new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>() {
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Quest", CallbackData = $"/{Constants.Commands.CreateQuestGame}" },
                new InlineKeyboardButton() { Text = "Demo", CallbackData = $"/{Constants.Commands.CreateDemoGame}" }
            },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "En.Cx", CallbackData = $"/{Constants.Commands.CreateEncxGame}" }
            },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Igra.lv", CallbackData = $"/{Constants.Commands.CreateIgraGame}" },
            },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Cancel", CallbackData = $"/{Constants.Commands.DeleteMessage}" }
            }
        });

        public static InlineKeyboardMarkup GameEnCxZone => new InlineKeyboardMarkup(
            StaticData.EnCxGameList.Zones.Select(x => 
                new List<InlineKeyboardButton>{new InlineKeyboardButton() { Text = x, CallbackData = $"/{Constants.Commands.FindEncxGame} {x}" } }
                )
            .ToList()
            .Append(new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Refresh", CallbackData = $"UpdateZones" }, 
                new InlineKeyboardButton() { Text = "Cancel", CallbackData = $"/{Constants.Commands.ExitGame}" }
            }).ToList()
        );

        public static InlineKeyboardMarkup GameEnCxStatus(string zone) => new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Active", CallbackData = $"/{Constants.Commands.FindEncxGame} {zone} Active" }
            },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Coming", CallbackData = $"/{Constants.Commands.FindEncxGame} {zone} Coming" },
                },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Finished", CallbackData = $"/{Constants.Commands.FindEncxGame} {zone} Finished" },
                },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Cancel", CallbackData = $"/{Constants.Commands.FindEncxGame}" }
            }
        });

        public static InlineKeyboardMarkup GameEnCxGames(List<Game> games) => new InlineKeyboardMarkup(
                games.Select(x => new List<InlineKeyboardButton> { new InlineKeyboardButton() { Text=x.Title, CallbackData = $"/{Constants.Commands.FoundEncxGame} {x.EnCxId}"} }).ToList()
            .Append(new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Cancel", CallbackData = $"/{Constants.Commands.ExitGame}" }
            }).ToList()
            );

        public static InlineKeyboardMarkup GameEnCxType(string zone, string status) => new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Team", CallbackData = $"/{Constants.Commands.FindEncxGame} {zone} {status} Team" }
            },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Single", CallbackData = $"/{Constants.Commands.FindEncxGame} {zone} {status} Single" },
                },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Cancel", CallbackData = $"/{Constants.Commands.FindEncxGame} {zone}" }
            }
        });
        
        public static InlineKeyboardMarkup GameWithoutAuth(Game game) => new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>() {
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Set auth", CallbackData = $"/{Constants.Commands.SetAuth}" },
                new InlineKeyboardButton() { Text = "Join link", SwitchInlineQuery=$"/{Constants.Commands.JoinGameLink} {game.Guid}" }
            },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Refresh", CallbackData = $"/{Constants.Commands.GameSetup}" },
                new InlineKeyboardButton() { Text = "Cancel", CallbackData = $"/{Constants.Commands.ExitGame}" }
            }
        });

        public static InlineKeyboardMarkup GameCommands(Game game) => new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton { Text = "Refresh", CallbackData = $"/{Constants.Commands.GameSetup}" }
            },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton { Text = "Mirror", CallbackData = $"/{Constants.Commands.MirrorLink}" },
                new InlineKeyboardButton() { Text = "Join link", SwitchInlineQuery=$"/{Constants.Commands.JoinGameLink} {game.Guid}" },
                new InlineKeyboardButton() { Text = "Exit game", CallbackData = $"/{Constants.Commands.ExitGame}"}
            },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Get Task", CallbackData = $"/{Commands.GetTask}" }
            },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Settings", CallbackData = $"/{Commands.ShowGameSettings}" },
                new InlineKeyboardButton() { Text = "Done", CallbackData = $"/{Constants.Commands.DeleteMessage}" },
            }
        });

        public static InlineKeyboardMarkup GameSettingsAndCommands(Game game, Player player) => new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
        {
            new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton { Text = "Refresh", CallbackData = $"/{Constants.Commands.ShowGameSettings}" },
                new InlineKeyboardButton() { Text = "Join link", SwitchInlineQuery=$"/{Constants.Commands.JoinGameLink} {game.Guid}" },
                new InlineKeyboardButton() { Text = "Exit game", CallbackData = $"/{Constants.Commands.ExitGame}"}
            },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Get Task", CallbackData = $"/{Commands.GetTask}" },
                new InlineKeyboardButton() { Text = $"{((player.UpdateTaskInfo == Player.PlayerUpdateStatusInfo.DontUpdate) ? "[" : String.Empty) }Disabled{((player.UpdateTaskInfo == Player.PlayerUpdateStatusInfo.DontUpdate) ? "]" : String.Empty) }", CallbackData = $"/{Commands.GameTaskNoUpdates}" },
                new InlineKeyboardButton() { Text = $"{((player.UpdateTaskInfo == Player.PlayerUpdateStatusInfo.UpdateStatus) ? "[" : String.Empty) }Up only{((player.UpdateTaskInfo == Player.PlayerUpdateStatusInfo.UpdateStatus) ? "]" : String.Empty) }", CallbackData = $"/{Commands.GameTaskUpdateStatus}" },
                new InlineKeyboardButton() { Text = $"{((player.UpdateTaskInfo == Player.PlayerUpdateStatusInfo.UpdateText) ? "[" : String.Empty) }With text{((player.UpdateTaskInfo == Player.PlayerUpdateStatusInfo.UpdateText) ? "]" : String.Empty) }", CallbackData = $"/{Commands.GameTaskUpdateText}" }
            },
            //new List<InlineKeyboardButton>()
            //{
            //    new InlineKeyboardButton() { Text = "Get Statistics", CallbackData = "GetStatistics" },
            //    new InlineKeyboardButton() { Text = "No Statistics", CallbackData = "DisableStatisticUpdates"},
            //    new InlineKeyboardButton() { Text = "Same lvl", CallbackData = "EnableStatisticLvl" },
            //    new InlineKeyboardButton() { Text = "All Statistics", CallbackData = "EnableStatistics" }
            //},
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = $"Set Prefix (Current:{game.Prefix ?? ""})", CallbackData = $"/{Commands.SetPrefix} {game.Id}" }
            },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = $"Set radius(m) Current: {game.Radius?.ToString()}m", CallbackData = $"/{Commands.SetRadius} {game.Id}" }
            },
            new List<InlineKeyboardButton>()
            {
                new InlineKeyboardButton() { Text = "Done", CallbackData = $"/{Constants.Commands.DeleteMessage}" },
            },
        });
    }
}
