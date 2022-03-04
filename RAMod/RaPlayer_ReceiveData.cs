// -----------------------------------------------------------------------
// <copyright file="RaPlayer_ReceiveData.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using HarmonyLib;
using RemoteAdmin;
using RemoteAdmin.Communication;

namespace Mistaken.RAMod
{
    [HarmonyPatch(typeof(RaPlayer))]
    [HarmonyPatch(nameof(RaPlayer.ReceiveData))]
    [HarmonyPatch(new Type[] { typeof(CommandSender), typeof(string) })]
    internal static class RaPlayer_ReceiveData
    {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        public static bool Prefix(RaPlayer __instance, CommandSender sender, string data)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        {
            try
            {
                string[] array = data.Split(' ');
                if (array.Length != 2)
                    return false;

                if (!int.TryParse(array[0], out int requestType))
                    return false;

                var playerCommandSender = sender as PlayerCommandSender;
                if (playerCommandSender is null)
                    return true;

                if (LOFHPatch.DisabledFor.Contains(playerCommandSender.CharacterClassManager.UserId))
                    return true;

                bool basicInfoOnly = requestType == 1;

                if (!basicInfoOnly
                    && !playerCommandSender.ServerRoles.Staff
                    && !CommandProcessor.CheckPermissions(sender, global::PlayerPermissions.PlayerSensitiveDataAccess))
                    return false;

                List<global::ReferenceHub> list = Utils.RAUtils.ProcessPlayerIdOrNamesList(new ArraySegment<string>(array.Skip(1).ToArray<string>()), 0, out string[] array2, false);
                if (list.Count == 0)
                    return false;

                bool userIdAccess =
                    global::PermissionsHandler.IsPermitted(sender.Permissions, 18007046UL)
                    || playerCommandSender.ServerRoles.Staff
                    || playerCommandSender.ServerRoles.RaEverywhere;

                if (list.Count > 1)
                {
                    return true;
                    /*StringBuilder stringBuilder = StringBuilderPool.Shared.Rent("<color=white>");
                    stringBuilder.Append("Selecting multiple players:");
                    stringBuilder.Append("\nPlayer ID: <color=green><link=CP_ID></link></color>");
                    stringBuilder.Append("\nIP Address: " + ((!basicInfoOnly) ? "<color=green><link=CP_IP></link></color>" : "[REDACTED]"));
                    stringBuilder.Append("\nUser ID: " + (userIdAccess ? "<color=green><link=CP_USERID></link></color>" : "[REDACTED]"));
                    stringBuilder.Append("</color>");
                    string text = string.Empty;
                    string text2 = string.Empty;
                    string text3 = string.Empty;
                    foreach (global::ReferenceHub referenceHub in list)
                    {
                        text = text + referenceHub.playerId + ".";
                        if (!basicInfoOnly)
                            text2 = text2 + ((referenceHub.networkIdentity.connectionToClient.IpOverride != null) ? referenceHub.networkIdentity.connectionToClient.OriginalIpAddress : referenceHub.networkIdentity.connectionToClient.address) + ",";

                        if (userIdAccess)
                            text3 = text3 + referenceHub.characterClassManager.UserId + ".";
                    }

                    if (text.Length > 0)
                        RaClipboard.Send(sender, RaClipboard.RaClipBoardType.PlayerId, text);

                    if (text2.Length > 0)
                        RaClipboard.Send(sender, RaClipboard.RaClipBoardType.Ip, text2);

                    if (text3.Length > 0)
                        RaClipboard.Send(sender, RaClipboard.RaClipBoardType.UserId, text3);

                    sender.RaReply(string.Format("${0} {1}", __instance.DataId, stringBuilder), true, true, string.Empty);
                    StringBuilderPool.Shared.Return(stringBuilder);
                    return false;*/
                }

                try
                {
                    RaClipboard.Send(sender, RaClipboard.RaClipBoardType.PlayerId, string.Empty);
                    RaClipboard.Send(sender, RaClipboard.RaClipBoardType.UserId, string.Empty);
                    RaClipboard.Send(sender, RaClipboard.RaClipBoardType.Ip, string.Empty);

                    var senderPlayer = Player.Get(playerCommandSender);
                    var response = MenuManager.GetCurrentMenu(senderPlayer).HandlePlayerInfoRequest(senderPlayer, basicInfoOnly ? 0 : 1, array[1].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray());
                    sender.RaReply(string.Format("${0} {1}", __instance.DataId, response), true, true, string.Empty);
                    RaPlayerQR.Send(sender, false, string.IsNullOrEmpty(senderPlayer.UserId) ? "(no User ID)" : senderPlayer.UserId);
                    return false;
                }
                catch (Exception ex3)
                {
                    sender.RaReply(
                        string.Format("${0} {1}", __instance.DataId, string.Concat(new string[]
                        {
                            data.ToUpper(),
                            "#An unexpected problem has occurred!\nMessage: ",
                            ex3.Message,
                            "\nStackTrace: ",
                            ex3.StackTrace,
                            "\nAt: ",
                            ex3.Source,
                        })),
                        false,
                        true,
                        string.Empty);
                    throw;
                }
            }
            catch (System.Exception e)
            {
                Exiled.API.Features.Log.Error($"[LOFHBase Late Error Catch] {data}");
                Exiled.API.Features.Log.Error($"[LOFHBase Late Error Catch] {e.Message}");
                Exiled.API.Features.Log.Error($"[LOFHBase Late Error Catch] {e.StackTrace}");
                return true;
            }
        }
    }
}