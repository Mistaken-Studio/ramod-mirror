// -----------------------------------------------------------------------
// <copyright file="DefaultMenu.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Mistaken.API;
using Mistaken.API.Extensions;
using RemoteAdmin.Communication;

namespace Mistaken.RAMod.Menus
{
    internal class DefaultMenu : Menu
    {
        public static string GeneratePlayerlist(Player sender)
        {
            sender.ReferenceHub.queryProcessor.GameplayData = true;

            bool hiddenBages = sender.CheckPermissions(PlayerPermissions.ViewHiddenBadges);
            bool hiddenGlobalBages = sender.CheckPermissions(PlayerPermissions.ViewHiddenGlobalBadges);

            if (sender.ReferenceHub.serverRoles.Staff)
            {
                hiddenBages = true;
                hiddenGlobalBages = true;
            }

            bool generatePlayerList = true;
            string text3 = string.Empty; // $"\n{MenuSystem.GetMenu(sender, out bool GeneratePlayerList)}";
            if (generatePlayerList)
            {
                foreach (Player player in RealPlayers.List.OrderBy(i => i.Id))
                {
                    try
                    {
                        if (player == null)
                            continue;
                        string text4 = string.Empty;
                        string nickname = player.TryGetSessionVariable("REAL_NICKNAME", out string name) ? $"{player.ReferenceHub.nicknameSync.CombinedName}<color=#FF5439>**</color> ({name})" : player.ReferenceHub.nicknameSync.CombinedName;

                        ServerRoles serverRoles = player.ReferenceHub.serverRoles;

                        if (string.IsNullOrEmpty(serverRoles.HiddenBadge) || (serverRoles.GlobalHidden && hiddenGlobalBages) || (!serverRoles.GlobalHidden && hiddenBages))
                        {
                            if (serverRoles.RaEverywhere)
                                text4 = "<link=RA_RaEverywhere><color=white>[<color=#EFC01A></color><color=white>]</color></link> ";
                            else if (serverRoles.Staff)
                                text4 = "<link=RA_StudioStaff><color=white>[<color=#005EBC></color><color=white>]</color></link> ";
                        }

                        if (player.GetSessionVariable<bool>(SessionVarType.HIDDEN))
                        {
                            if (sender.Group?.KickPower >= player.Group?.KickPower)
                                text4 += "|HIDDEN| ";
                            else
                                continue;
                        }

                        if (player.UserId.IsDevUserId())
                            text4 += "[<color=#FF0000>|</color><b><color=#FFD700><b>DEV</b></color></b><color=#FF0000>|</color>] ";

                        if (player.GetSessionVariable<bool>(SessionVarType.VANISH))
                            text4 += $"[<V{VanishHandler.Vanished[player.Id]}>] ";

                        if (serverRoles.RemoteAdmin)
                            text4 += "<link=RA_Admin><color=white>[]</color></link> ";

                        if (player.IsOverwatchEnabled)
                            text4 += "<link=RA_OverwatchEnabled><color=white>[</color><color=#03f8fc></color><color=white>]</color></link> ";

                        if (player.IsMuted)
                            text4 += "[<color=red>MUTED</color>]";
                        else if (player.IsIntercomMuted)
                            text4 += "[<color=red><color=#FFC0CB>I</color>MUTED</color>]";

                        if (player.GetSessionVariable<bool>("WARNING"))
                            text4 += $"[<color=yellow>!</color>]<color=orange>WARNING</color>[<color=red>!</color>] ";

                        if (ModdedRAHandler.Prefixes.TryGetValue(player.UserId, out var prefix) && prefix.Count != 0)
                            text4 += $"{string.Join(" ", prefix.Select(x => x.Value))} ";

                        text3 += $"{text4}<color={RoleToColor(player)}>({player.Id}) {nickname.Replace("\n", string.Empty)}</color>\n";
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error(ex.Message);
                        Log.Error(ex.StackTrace);

                        text3 += $"[ERROR] <color={RoleToColor(player)}>({player.Id}){player.ReferenceHub.nicknameSync.CombinedName.Replace("\n", string.Empty)}</color>\n";
                    }
                }
            }

            return text3;
        }

        public static string RoleToColor(Player player)
        {
            var data = Server.Host.GetSessionVariable<Dictionary<string, string>>("RAMod.CustomColors");
            if (data != null)
            {
                foreach (var item in data)
                {
                    if (player.GetSessionVariable<bool>(item.Key))
                        return item.Value;
                }
            }

            return RoleToColor(player.Role, player.IsOverwatchEnabled);
        }

        public static string RoleToColor(RoleType role, bool ovrm)
        {
            if (ovrm)
                return "#00264d"; // "#008080";
            switch (role)
            {
                // "#1d6f00";
                case RoleType.ChaosConscript:
                    return "#097c1b";
                case RoleType.ChaosMarauder:
                    return "#006826";
                case RoleType.ChaosRepressor:
                    return "#0d7d35";
                case RoleType.ChaosRifleman:
                    return "#097c1b";
                case RoleType.ClassD:
                    return "#ff8400";
                case RoleType.FacilityGuard:
                    return "#7795a9";
                case RoleType.None:
                    return "#afafaf";
                case RoleType.NtfPrivate:
                    return "#61beff";
                case RoleType.NtfSergeant:
                    return "#0096ff";
                case RoleType.NtfCaptain:
                    return "#1200ff";
                case RoleType.NtfSpecialist:
                    return "#4e63ff";
                case RoleType.Scientist:
                    return "#f1e96e";
                case RoleType.Scp93989:
                case RoleType.Scp93953:
                case RoleType.Scp173:
                case RoleType.Scp106:
                case RoleType.Scp096:
                case RoleType.Scp079:
                case RoleType.Scp049:
                    return "#FF0000";
                case RoleType.Scp0492:
                    return "#800000";
                case RoleType.Spectator:
                    return "#FFFFFF";
                case RoleType.Tutorial:
                    return "#00FF00";

                default:
                    return "#FF00FF";
            }
        }

        public override int Id => 0;

        public override int ParrentId => 0;

        public override string ExitButton(Player sender)
        {
            return string.Empty;
        }

        public override string EnterButton(Player sender)
        {
            return string.Empty;
        }

        public override string HandlerPlayerlistRequest(Player sender)
        {
            return GeneratePlayerlist(sender);
        }

        public override string HandlePlayerInfoRequest(Player sender, int type, params int[] playerIds)
        {
            if (playerIds.Length == 0)
                return "No player selected";

            var target = Player.Get(playerIds[0]);

            if (target == null)
                return $"Player with id {playerIds[0]} not found";

            MenuManager.LastSelectedPlayer[sender] = (playerIds[0], type);

            bool gameplayData = sender.CheckPermissions(PlayerPermissions.GameplayData);
            bool userIdAccess = sender.CheckPermissions((PlayerPermissions)18007046UL);
            bool ipAccess = type == 0;

            sender.ReferenceHub.queryProcessor.GameplayData = gameplayData;

            if (sender.ReferenceHub.serverRoles.Staff || sender.ReferenceHub.serverRoles.RaEverywhere)
                userIdAccess = true;

            if (ipAccess)
            {
                ServerLogs.AddLog(
                    ServerLogs.Modules.DataAccess,
                    $"{sender.Sender.LogName} accessed IP address of player {target.Id} ({target.Nickname}).",
                    ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging,
                    false);
            }

            RaClipboard.Send(sender.Sender, RaClipboard.RaClipBoardType.PlayerId, target.Id.ToString());

            if (userIdAccess)
                RaClipboard.Send(sender.Sender, RaClipboard.RaClipBoardType.UserId, target.UserId);

            if (ipAccess)
                RaClipboard.Send(sender.Sender, RaClipboard.RaClipBoardType.Ip, target.IPAddress);

            var playerInfo = PlayerInfoHandler.GetPattern(target, gameplayData, userIdAccess, ipAccess, type == 1);
            return playerInfo;
        }
    }
}
