// -----------------------------------------------------------------------
// <copyright file="PlayerInfoHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Mistaken.API.Extensions;

namespace Mistaken.RAMod
{
    /// <summary>
    /// PlayerInfo Handler.
    /// </summary>
    public static class PlayerInfoHandler
    {
        /// <summary>
        /// Custom if arguments.
        /// </summary>
        public static readonly Dictionary<string, Func<Player, bool, bool>> IfParsers = new Dictionary<string, Func<Player, bool, bool>>
        {
            {
                "gameplayDataAccess",
                (player, gameplayData) => gameplayData
            },
            {
                "hasLocalHiddenRole",
                (player, gameplayData) => !string.IsNullOrEmpty(player.ReferenceHub.serverRoles.HiddenBadge)
            },
            {
                "hasGlobalHiddenRole",
                (player, gameplayData) => player.ReferenceHub.serverRoles.GlobalHidden && !string.IsNullOrEmpty(player.ReferenceHub.serverRoles.HiddenBadge)
            },
            {
                "viewLocalHiddenRoles",
                (player, gameplayData) => player.CheckPermissions(PlayerPermissions.ViewHiddenBadges) || player.ReferenceHub.serverRoles.Staff
            },
            {
                "viewGlobalHiddenRoles",
                (player, gameplayData) => player.CheckPermissions(PlayerPermissions.ViewHiddenGlobalBadges) || player.ReferenceHub.serverRoles.Staff
            },
            {
                "isNWGlobalStaff",
                (player, gameplayData) => player.ReferenceHub.serverRoles.RaEverywhere
            },
            {
                "isNWStaff",
                (player, gameplayData) => player.ReferenceHub.serverRoles.Staff
            },
            {
                "mute",
                (player, gameplayData) => player.IsMuted
            },
            {
                "imute",
                (player, gameplayData) => player.IsIntercomMuted
            },
            {
                "godmode",
                (player, gameplayData) => player.IsGodModeEnabled
            },
            {
                "bypass",
                (player, gameplayData) => player.IsBypassModeEnabled
            },
            {
                "noclip",
                (player, gameplayData) => player.NoClipEnabled
            },
            {
                "overwatch",
                (player, gameplayData) => player.IsOverwatchEnabled
            },
            {
                "dnt",
                (player, gameplayData) => player.DoNotTrack
            },
            {
                "ra",
                (player, gameplayData) => player.RemoteAdminAccess
            },
            {
                "rip",
                (player, gameplayData) => player.IsDead
            },
        };

        /// <summary>
        /// Custom vars used to display data.
        /// </summary>
        public static readonly Dictionary<string, Func<Player, bool, string>> NormalParsers = new Dictionary<string, Func<Player, bool, string>>
        {
            {
                "emptyLine",
                (player, gameplayData) => string.Empty
            },
            {
                "nickname",
                (player, gameplayData) => player.TryGetSessionVariable("REAL_NICKNAME", out string nickname) ? $"{player.ReferenceHub.nicknameSync.CombinedName}<color=#FF5439>**</color> ({nickname})" : player.ReferenceHub.nicknameSync.CombinedName
            },
            {
                "playerId",
                (player, gameplayData) => player.Id.ToString()
            },
            {
                "ip",
                (player, gameplayData) => player.IPAddress
            },
            {
                "userId",
                (player, gameplayData) => player.UserId
            },
            {
                "userId2",
                (player, gameplayData) => player.CustomUserId
            },
            {
                "role",
                (player, gameplayData) => player.ReferenceHub.serverRoles.GetColoredRoleString(false)
            },
            {
                "hiddenRole",
                (player, gameplayData) => player.ReferenceHub.serverRoles.HiddenBadge
            },
            {
                "group",
                (player, gameplayData) => player.GroupName
            },
            {
                "class",
                (player, gameplayData) => player.Role.ToString()
            },
            {
                "classColor",
                (player, gameplayData) => Menus.DefaultMenu.RoleToColor(player.Role, false)
            },
            {
                "hp",
                (player, gameplayData) => player.Health.ToString()
            },
            {
                "maxhp",
                (player, gameplayData) => player.MaxHealth.ToString()
            },
            {
                "ahp",
                (player, gameplayData) => player.ArtificialHealth.ToString()
            },
            {
                "maxahp",
                (player, gameplayData) => player.MaxArtificialHealth.ToString()
            },
            {
                "textHP",
                (player, gameplayData) => $"{player.Health}/{player.MaxHealth} ({(player.MaxHealth == 0 ? "Infinity" : ((player.Health / player.MaxHealth) * 100).ToString())}%)"
            },
            {
                "textAHP",
                (player, gameplayData) => $"{player.ArtificialHealth}/{player.MaxArtificialHealth} ({(player.MaxArtificialHealth == 0 ? "Infinity" : ((player.ArtificialHealth / player.MaxArtificialHealth) * 100).ToString())}%)"
            },
            {
                "position",
                (player, gameplayData) => player.Position.ToString()
            },
            {
                "room",
                (player, gameplayData) =>
                {
                    var room = player.CurrentRoom;
                    if (room == null)
                        return "Unknown (Unknown)";
                    else
                        return $"{room.Name} ({room.Type})";
                }
            },
            {
                "effects",
                (player, gameplayData) =>
                {
                    string tor = string.Empty;

                    foreach (var item in player.ReferenceHub.playerEffectsController.AllEffects)
                    {
                        if (item.Value.IsEnabled)
                            tor += $"\n- {item.Key.Name} ({item.Value.Intensity})" + (item.Value.Duration > 0 ? $" {item.Value.Duration}s left" : string.Empty);
                    }

                    return tor;
                }
            },
            {
                "effects-2columns",
                (player, gameplayData) =>
                {
                    string tor = string.Empty;

                    bool reverse = false;
                    var effects = player.ReferenceHub.playerEffectsController.AllEffects.Where(x => x.Value.IsEnabled).ToArray();
                    int size = effects.Length > 8 ? 50 : 75;
                    for (int i = 0; i < effects.Length; i++)
                    {
                        if (i != 0)
                            tor += "\n";
                        string effect = $"{effects[i].Key.Name} ({effects[i].Value.Intensity})" + (effects[i].Value.Duration > 0 ? $" {effects[i].Value.Duration}s left" : string.Empty);
                        if (reverse)
                            tor += "</line-height><align=right>" + effect + " -</align>";
                        else
                        {
                            if (i + 1 == effects.Length)
                                tor += "- " + effect;
                            else
                                tor += "<align=left>- " + effect + "</align><line-height=1px>";
                        }

                        reverse = !reverse;
                    }

                    return $"<size={size}%>{tor}</size>";
                }
            },
            {
                "effects-3columns",
                (player, gameplayData) =>
                {
                    string tor = string.Empty;

                    int state = 0;
                    var effects = player.ReferenceHub.playerEffectsController.AllEffects.Where(x => x.Value.IsEnabled).ToArray();
                    int size = 75;
                    for (int i = 0; i < effects.Length; i++)
                    {
                        if (i != 0)
                            tor += "\n";
                        string effect = EffectToString(effects[i]);

                        if (state == 2)
                        {
                            if (i == 8)
                            {
                                int left = effect.Length - 8;
                                tor += $"</line-height><align=right>And {left} other effect{(left == 1 ? string.Empty : "s")} -</align>";
                                break;
                            }

                            tor += "</line-height><align=right>" + effect + " -</align>";
                        }
                        else if (state == 1)
                            tor += $"</line-height><align=center>{effect}</align><line-height=1px>";
                        else
                        {
                            if (i + 2 > effects.Length)
                            {
                                tor += "- " + effect;
                                break;
                            }
                            else if (i + 2 == effects.Length)
                            {
                                tor += "<align=left>- " + effect + "</align><line-height=1px>";
                                tor += "\n</line-height><align=center>" + EffectToString(effects[i + 1]) + "</align>";
                                break;
                            }
                            else
                                tor += "<align=left>- " + effect + "</align><line-height=1px>";
                        }

                        state++;
                        if (state > 2)
                            state = 0;
                    }

                    return $"<size={size}%>{tor}</size>";
                }
            },
            {
                "currentItem",
                (player, gameplayData) =>
                {
                    if (player.CurrentItem != null)
                        return ItemToString(player.CurrentItem, false, false);
                    else
                        return "None";
                }
            },
            {
                "items",
                (player, gameplayData) =>
                {
                    string tor = string.Empty;

                    foreach (var item in player.Items)
                        tor += ItemToString(item, false);

                    return $"<size=75%>{tor}</size>";
                }
            },
            {
                "items-2columns",
                (player, gameplayData) =>
                {
                    string tor = string.Empty;

                    bool reverse = false;
                    var items = player.Items.ToArray();
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (i != 0)
                           tor += "\n";

                        if (reverse)
                            tor += "</line-height><align=right>" + ItemToString(items[i], false) + " -</align>";
                        else
                        {
                            if (i + 1 == items.Length)
                                tor += "- " + ItemToString(items[i], false);
                            else
                                tor += "<align=left>- " + ItemToString(items[i], false) + "</align><line-height=1px>";
                        }

                        reverse = !reverse;
                    }

                    return $"<size=70%>{tor}</size>";
                }
            },
            {
                "coloredCurrentItem",
                (player, gameplayData) =>
                {
                    if (player.CurrentItem != null)
                        return ItemToString(player.CurrentItem, true, false);
                    else
                        return "None";
                }
            },
            {
                "coloredItems",
                (player, gameplayData) =>
                {
                    string tor = string.Empty;

                    foreach (var item in player.Items)
                        tor += "\n- " + ItemToString(item, true);

                    return $"<size=75%>{tor}</size>";
                }
            },
            {
                "coloredItems-2columns",
                (player, gameplayData) =>
                {
                    string tor = string.Empty;

                    bool reverse = false;
                    var items = player.Items.ToArray();
                    for (int i = 0; i < items.Length; i++)
                    {
                        if (i != 0)
                           tor += "\n";

                        if (reverse)
                            tor += "</line-height><align=right>" + ItemToString(items[i], true) + " -</align>";
                        else
                        {
                            if (i + 1 == items.Length)
                                tor += "- " + ItemToString(items[i], true);
                            else
                                tor += "<align=left>- " + ItemToString(items[i], true) + "</align><line-height=1px>";
                        }

                        reverse = !reverse;
                    }

                    return $"<size=75%>{tor}</size>";
                }
            },
            {
                "ammo",
                (player, gameplayData) =>
                {
                    return "W.I.P";

                    // return $"<align=left></align>{player.Ammo[(int)AmmoType.Nato556]}x5.56mm<line-height=1px>\n</line-height><align=center>{player.Ammo[(int)AmmoType.Nato762]}x7.62mm</align><line-height=1px>\n</line-height><align=right>{player.Ammo[(int)AmmoType.Nato9]}x9mm</align>";
                }
            },
            {
                "unit",
                (player, gameplayData) => player.UnitName
            },
            /*{
                "shield",
                (player, gameplayData) =>
                {
                    if (API.Shield.ShieldedManager.TryGet(player, out var data))
                        return $"Max: {data.MaxShield} | Regeneration: {data.Regeneration}";

                    return string.Empty;
                }
            },*/
            {
                "countryCode",
                (player, gameplayData) =>
                {
                    if (ModdedRAHandler.CountryCodes.TryGetValue(player.UserId, out string countryCode))
                        return countryCode;

                    return "<color=red>Failed to find country code</color>";
                }
            },
        };

        static PlayerInfoHandler()
        {
            proccessedPattern = string.Join("\n", PreprocessPattern(Pattern));
        }

        /// <summary>
        /// Gets or sets pattern used to display Player Info.
        /// </summary>
        public static string Pattern
        {
            get
            {
                if (pattern == null)
                {
                    pattern = @"
#list
    Player ID: {playerId}
    Nickname: {nickname}                             <color=#0000> .</color>

    #if {userIdAccess}
        User ID: {userId}
    #else
        User ID: <color=#D4AF37>INSUFFICIENT PERMISSIONS</color>
    #endif

    #if {ipAccess}
        IP: {ip}                             <color=#0000> .</color>
    #else
        IP: [REDACTED]                             <color=#0000> .</color>
    #endif
#endlist

#if {userIdAccess}&!null-{userId2}
    User ID 2: {userId2}
#endif
#if !null-{role}|!null-{group}
    #list
        #if !null-{role}
            Server role: {role}
        #endif

        #if !null-{group}
            Group: {group}                             <color=#0000> .</color>
        #else
            <color=#0000>.</color>
        #endif
    #endlist
#endif

#if {hasLocalHiddenRole}&{viewLocalHiddenRoles}
    <color=#DC143C>Hidden role: </color>{hiddenRole} (LOCAL)
#endif

#if {hasGlobalHiddenRole}&{viewGlobalHiddenRoles}
    <color=#DC143C>Hidden role: </color>{hiddenRole} (GLOBAL)
#endif

#if {isNWGlobalStaff}&{viewGlobalHiddenRoles}|{viewLocalHiddenRoles}
    Active flag: <color=#BCC6CC>Studio GLOBAL Staff (management or global moderation)</color>
#endif

#if !{isNWGlobalStaff}&{isNWStaff}&{viewGlobalHiddenRoles}|{viewLocalHiddenRoles}
    Active flag: Studio Staff
#endif

Country Code: {countryCode}

#list
    #if {mute}|{imute}
        #if {mute}
            Active flag: <color=#F70D1A>SERVER MUTED</color>
        #else
            <color=#0000>.</color>
        #endif

        #if {imute}
            Active flag: <color=#F70D1A>INTERCOM MUTED</color>                             <color=#0000> .</color>
        #else
            <color=#0000>.</color>                             <color=#0000> .</color>
        #endif
    #endif

    #if {godmode}|{bypass}
        #if {godmode}
            Active flag: <color=#659EC7>GOD MODE</color>
        #else
            <color=#0000>.</color>
        #endif

        #if {bypass}
            Active flag: <color=#BFFF00>BYPASS MODE</color>                             <color=#0000> .</color>
        #else
            <color=#0000>.</color>                             <color=#0000> .</color>
        #endif
    #endif

    #if {overwatch}|{noclip}
        #if {overwatch}
            Active flag: <color=#008080>OVERWATCH MODE</color>
        #else
            <color=#0000>.</color>
        #endif

        #if {noclip}
            Active flag: <color=#DC143C>NOCLIP ENABLED</color>                             <color=#0000> .</color>
        #else
            <color=#0000>.</color>                             <color=#0000> .</color>
        #endif
    #endif

    #if {ra}|{dnt}
        #if {ra}
            Active flag: <color=#43C6DB>REMOTE ADMIN AUTHENTICATED</color>
        #else
            <color=#0000>.</color>
        #endif

        #if {dnt}
            Active flag: <color=#BFFF00>DO NOT TRACK</color>                             <color=#0000> .</color>
        #else
            <color=#0000>.</color>                             <color=#0000> .</color>
        #endif
    #endif
#endlist

#if {ipAccess}
    #if !{gameplayDataAccess}
        Class: <color=#D4AF37>INSUFFICIENT PERMISSIONS</color>
        #if !{rip}
            #list
                HP: <color=#D4AF37>INSUFFICIENT PERMISSIONS</color>
                AHP: <color=#D4AF37>INSUFFICIENT PERMISSIONS</color>
                Position: <color=#D4AF37>INSUFFICIENT PERMISSIONS</color>
                Room: <color=#D4AF37>INSUFFICIENT PERMISSIONS</color>
            #endlist
        #endif
        <color=#D4AF37>* GameplayData permission required</color>
    #else
        #list
            Class: <color={classColor}>{class}</color>
            #if !null-{unit}
                Unit: {unit}
            #else
                <color=#0000>.</color>                             <color=#0000> .</color>
            #endif
        #endlist
        #if !{rip}
            #list
                HP: {textHP}
                AHP: {textAHP}

                Position: {position}
                Room: {room}
            #endlist

            #if !null-{shield}
            #    Shield: {shield}
            #endif

            #list
                Items: 
                Current Item: {coloredCurrentItem}
            #endlist

            {coloredItems-2columns}

            Ammo: {ammo}

            #if !null-{effects}
                Effects: 
                {effects-3columns}
            #endif
        #endif
    #endif
#else
    #if !{gameplayDataAccess}
        Class: <color=#D4AF37>INSUFFICIENT PERMISSIONS</color>
        <color=#D4AF37>* GameplayData permission required</color>
    #else
        Class: <color={classColor}>{class}</color>
    #endif

    #AdminData

#endif
";
                    proccessedPattern = string.Join("\n", PreprocessPattern(pattern));
                }

                return pattern;
            }

            set
            {
                pattern = value;
                proccessedPattern = string.Join("\n", PreprocessPattern(pattern));
            }
        }

        internal static string GetPattern(Player target, bool gameplayData, bool userId, bool ip)
        {
            StringBuilder builder = NorthwoodLib.Pools.StringBuilderPool.Shared.Rent();

            Dictionary<string, bool> ifCache = new Dictionary<string, bool>()
            {
                { "{userIdAccess}", userId },
                { "{ipAccess}", ip },
            };
            Dictionary<string, string> normalCache = new Dictionary<string, string>();

            // IfParsers.Add("userIdAccess", (p, _) => userId);
            // IfParsers.Add("ipAccess", (p, _) => ip);
            var lines = proccessedPattern.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    if (lines[i] == "#end_of_pattern")
                        break;

                    if (lines[i].StartsWith("#goto"))
                    {
                        var arg = int.Parse(lines[i].Split(' ')[1]);
                        i = arg - 1;
                        continue;
                    }

                    if (lines[i].StartsWith("#if "))
                    {
                        var arg = lines[i].Split(' ')[1];
                        bool result;
                        if (ifCache.ContainsKey(arg))
                            result = ifCache[arg];
                        else
                            result = HandleIf(arg, target, gameplayData, ifCache, normalCache);
                        if (result)
                            i++;
                        continue;
                    }

                    if (lines[i].StartsWith("#"))
                        continue;

                    foreach (var parser in NormalParsers)
                    {
                        if (lines[i].Contains("{" + parser.Key + "}"))
                        {
                            if (normalCache.ContainsKey(parser.Key))
                                lines[i] = lines[i].Replace("{" + parser.Key + "}", normalCache[parser.Key]);
                            else
                            {
                                string result = parser.Value(target, gameplayData);
                                lines[i] = lines[i].Replace("{" + parser.Key + "}", result);
                                normalCache[parser.Key] = result;
                            }
                        }
                    }

                    builder.AppendLine(lines[i]);
                }
                catch (Exception ex)
                {
                    Exiled.API.Features.Log.Error($"Exception when parsing line {i}, \"{lines[i]}\"");
                    Exiled.API.Features.Log.Error(ex.Message);
                    Exiled.API.Features.Log.Error(ex.StackTrace);
                    builder.AppendLine($"Failed to parse \"{lines[i]}\"");
                }
            }

            string builded = NorthwoodLib.Pools.StringBuilderPool.Shared.ToStringReturn(builder);

            var buildedLines = builded.Split('\n');

            int rawLines = buildedLines.Count();
            Exiled.API.Features.Log.Debug($"Raw Lines = {rawLines}", PluginHandler.Instance.Config.VerbouseOutput);
            int realLines = rawLines - buildedLines.Where(x => x.Contains("<line-height=1px>")).Count();
            Exiled.API.Features.Log.Debug($"Real Lines = {realLines}", PluginHandler.Instance.Config.VerbouseOutput);

            if (realLines > 20)
            {
                for (int i = 0; i < buildedLines.Length; i++)
                {
                    if (!buildedLines[i].Contains("<size="))
                        continue;
                    int oldSize = int.Parse(buildedLines[i].Split(new string[] { "<size=" }, StringSplitOptions.None)[1].Split('%', '>')[0]);
                    float newSize = oldSize * 0.65f;
                    buildedLines[i] = buildedLines[i].Replace($"<size={oldSize}", $"<size={newSize}");
                }

                builded = $"<size=65%>{string.Join("\n", buildedLines).Replace("<color=#0000> .</color>", "             <color=#0000> .</color>")}</size>";
            }
            else if (realLines > 17)
            {
                for (int i = 0; i < buildedLines.Length; i++)
                {
                    if (!buildedLines[i].Contains("<size="))
                        continue;
                    int oldSize = int.Parse(buildedLines[i].Split(new string[] { "<size=" }, StringSplitOptions.None)[1].Split('%', '>')[0]);
                    float newSize = oldSize * 0.70f;
                    buildedLines[i] = buildedLines[i].Replace($"<size={oldSize}", $"<size={newSize}");
                }

                builded = $"<size=70%>{string.Join("\n", buildedLines).Replace("<color=#0000> .</color>", "          <color=#0000> .</color>")}</size>";
            }
            else if (realLines > 14)
            {
                for (int i = 0; i < buildedLines.Length; i++)
                {
                    if (!buildedLines[i].Contains("<size="))
                        continue;
                    int oldSize = int.Parse(buildedLines[i].Split(new string[] { "<size=" }, StringSplitOptions.None)[1].Split('%', '>')[0]);
                    float newSize = oldSize * 0.80f;
                    buildedLines[i] = buildedLines[i].Replace($"<size={oldSize}", $"<size={newSize}");
                }

                builded = $"<size=80%>{string.Join("\n", buildedLines).Replace("<color=#0000> .</color>", "     <color=#0000> .</color>")}</size>";
            }

            return "<color=white>" + builded + "</color>";
        }

        private static string pattern = null;
        private static string proccessedPattern = "CRITICAL ERROR";

        private static string EffectToString(KeyValuePair<Type, CustomPlayerEffects.PlayerEffect> effect)
        {
            string tor = $"{effect.Key.Name}";
            if (effect.Value.Intensity != 0)
                tor += $" ({effect.Value.Intensity})";
            if (effect.Value.Duration > 0)
                tor += $" {effect.Value.Duration}s left";
            return tor;
        }

        private static string GetItemColor(ItemType item)
        {
            switch (item)
            {
                case ItemType.Adrenaline:
                case ItemType.Medkit:
                case ItemType.Painkillers:
                    return "#86ff52";

                case ItemType.SCP018:
                case ItemType.SCP207:
                case ItemType.SCP268:
                case ItemType.SCP500:
                    return "#b80000";

                case ItemType.MicroHID:
                case ItemType.GrenadeFlash:
                case ItemType.GrenadeHE:
                case ItemType.GunCOM15:
                case ItemType.GunE11SR:
                case ItemType.GunLogicer:
                case ItemType.GunFSP9:
                case ItemType.GunCrossvec:
                case ItemType.GunCOM18:
                    return "#73b0ff";

                case ItemType.Flashlight:
                case ItemType.Coin:
                case ItemType.Radio:
                    return "#a9bd64";

                case ItemType.KeycardChaosInsurgency:
                    return "#35493e";
                case ItemType.KeycardContainmentEngineer:
                    return "#b6887f";
                case ItemType.KeycardFacilityManager:
                    return "#ba1846";
                case ItemType.KeycardGuard:
                    return "#606770";
                case ItemType.KeycardJanitor:
                    return "#bcb1e4";
                case ItemType.KeycardNTFCommander:
                    return "#1841c8";
                case ItemType.KeycardNTFLieutenant:
                    return "#5180f7";
                case ItemType.KeycardO5:
                    return "#5b5b5b";
                case ItemType.KeycardScientist:
                    return "#e7d678";
                case ItemType.KeycardResearchCoordinator:
                    return "#ddab20";
                case ItemType.KeycardNTFOfficer:
                    return "#a2cade";
                case ItemType.KeycardZoneManager:
                    return "#217778";

                default:
                    return "#9900ff";
            }
        }

        private static string ItemToString(Item item, bool colored, bool showMods = true)
        {
            // It's too long :/
            showMods = false;

            if (colored)
            {
                string color = GetItemColor(item.Type);

                var weapon = item as Firearm;
                if (weapon != null)
                    return $"<color={color}>{weapon.Type}</color> Ammo: {weapon.Ammo}" + (!showMods ? string.Empty : $"Mods: {string.Join(" | ", weapon.Attachments.Where(x => x.IsEnabled).Select(x => $"{x.Name} ({x.Slot})"))}");
                else
                    return $"<color={color}>{item.Type}</color>";
            }
            else
            {
                var weapon = item as Firearm;
                if (weapon != null)
                    return $"{weapon.Type} Ammo: {weapon.Ammo}" + (!showMods ? string.Empty : $"Mods: {string.Join(" | ", weapon.Attachments.Where(x => x.IsEnabled).Select(x => $"{x.Name} ({x.Slot})"))}");
                else
                    return $"{item.Type}";
            }
        }

        private static string[] PreprocessPattern(string rawPattern)
        {
            List<string> lines = NorthwoodLib.Pools.ListPool<string>.Shared.Rent(rawPattern.Split('\n'));

            Exiled.API.Features.Log.Debug($"Preprocessing started, raw: ", PluginHandler.Instance.Config.VerbouseOutput);
            for (int i = 0; i < lines.Count; i++)
                Exiled.API.Features.Log.Debug($"[{i:00}] {lines[i]}", PluginHandler.Instance.Config.VerbouseOutput);

            bool hasEndOfPattern = false;

            for (int i = 0; i < lines.Count; i++)
            {
                lines[i] = lines[i].Trim().Replace("\t", string.Empty).Replace("\\t", "\t");
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    lines.RemoveAt(i);
                    i--;
                }
                else if (lines[i] == "#end_of_pattern")
                    hasEndOfPattern = true;
            }

            if (!hasEndOfPattern)
                lines.Add("#end_of_pattern");

            bool list = false;
            bool reverse = false;
            List<bool> reverses = new List<bool>();

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] == "#list")
                {
                    list = true;
                    lines.RemoveAt(i);
                    i--;
                    continue;
                }
                else if (lines[i] == "#endlist")
                {
                    list = false;
                    lines.RemoveAt(i);
                    i--;
                    continue;
                }

                if (!list)
                    continue;
                if (lines[i].StartsWith("#"))
                {
                    if (lines[i].StartsWith("#if"))
                    {
                        reverses.Add(reverse);
                    }
                    else if (lines[i].StartsWith("#else"))
                    {
                        reverse = reverses.Last();
                    }
                    else if (lines[i].StartsWith("#endif"))
                    {
                        // reverse = reverses.Last();
                        reverses.RemoveAt(reverses.Count - 1);
                    }

                    continue;
                }

                if (!reverse)
                    lines[i] = $"<align=left>{lines[i]}</align><line-height=1px>";
                else
                    lines[i] = $"</line-height><align=right>{lines[i]}</align>";
                reverse = !reverse;
            }

            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("#if "))
                {
                    int j = i;
                    int elsePos = -1;
                    bool ended = false;
                    int indentation = 0;
                    while (j + 1 < lines.Count)
                    {
                        j++;
                        if (lines[j].StartsWith("#if"))
                            indentation++;
                        else if (lines[j].StartsWith("#endif"))
                        {
                            if (indentation == 0)
                            {
                                if (elsePos == -1)
                                {
                                    j++;
                                    lines.Insert(i + 1, $"#goto {j}");
                                }
                                else
                                    lines[elsePos] = $"#goto {j}";
                                lines.RemoveAt(j);

                                ended = true;
                                break;
                            }
                            else
                                indentation--;
                        }
                        else if (lines[j].StartsWith("#else"))
                        {
                            if (indentation == 0)
                            {
                                j++;
                                lines.Insert(i + 1, $"#goto {j + 1}");
                                elsePos = j;
                            }
                        }
                    }

                    if (!ended)
                        throw new Exception($"Failed to preproces pattern, Not closed if on line {i}");
                }
            }

            Exiled.API.Features.Log.Debug($"Preprocessing complete, result: ", PluginHandler.Instance.Config.VerbouseOutput);
            for (int i = 0; i < lines.Count; i++)
                Exiled.API.Features.Log.Debug($"[{i:00}] {lines[i]}", PluginHandler.Instance.Config.VerbouseOutput);

            var torArray = lines.ToArray();
            NorthwoodLib.Pools.ListPool<string>.Shared.Return(lines);
            return torArray;
        }

        private static bool HandleIf(string arg, Player target, bool gameplayData, Dictionary<string, bool> ifCache, Dictionary<string, string> normalCache)
        {
            if (ifCache.ContainsKey(arg))
                return ifCache[arg];

            if (arg.Contains("&"))
            {
                foreach (var item in arg.Split('&'))
                {
                    if (!HandleIf(item, target, gameplayData, ifCache, normalCache))
                    {
                        ifCache[arg] = false;
                        return false;
                    }
                }

                ifCache[arg] = true;
                return true;
            }
            else if (arg.Contains("|"))
            {
                foreach (var item in arg.Split('|'))
                {
                    if (HandleIf(item, target, gameplayData, ifCache, normalCache))
                    {
                        ifCache[arg] = true;
                        return true;
                    }
                }

                ifCache[arg] = false;
                return false;
            }
            else if (arg.StartsWith("!"))
            {
                bool result = !HandleIf(arg.Substring(1), target, gameplayData, ifCache, normalCache);
                ifCache[arg] = result;
                return result;
            }
            else if (arg.StartsWith("null-"))
            {
                var tmp = arg.Substring(5);

                foreach (var parser in NormalParsers)
                {
                    if (tmp.Contains("{" + parser.Key + "}"))
                    {
                        if (normalCache.ContainsKey(parser.Key))
                            tmp = tmp.Replace("{" + parser.Key + "}", normalCache[parser.Key]);
                        else
                        {
                            var result = parser.Value(target, gameplayData);
                            tmp = tmp.Replace("{" + parser.Key + "}", result);
                            normalCache[parser.Key] = result;
                        }
                    }
                }

                ifCache[arg] = string.IsNullOrWhiteSpace(tmp);
                return string.IsNullOrWhiteSpace(tmp);
            }
            else
            {
                try
                {
                    arg = arg.Trim().Substring(1, arg.Length - 2);

                    if (!IfParsers.TryGetValue(arg, out var func))
                    {
                        ifCache["{" + arg + "}"] = true;
                        return true;
                    }

                    bool funcResult = func(target, gameplayData);

                    ifCache["{" + arg + "}"] = funcResult;
                    return funcResult;
                }
                catch (Exception ex)
                {
                    Exiled.API.Features.Log.Error($"Failed to parse if \"{arg}\"");
                    Exiled.API.Features.Log.Error(ex.Message);
                    Exiled.API.Features.Log.Error(ex.StackTrace);
                    return false;
                }
            }
        }
    }
}
