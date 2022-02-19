// -----------------------------------------------------------------------
// <copyright file="RaPlayerList_ReceiveData.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Features;
using HarmonyLib;
using RemoteAdmin;
using RemoteAdmin.Communication;

namespace Mistaken.RAMod
{
    [HarmonyPatch(typeof(RaPlayerList))]
    [HarmonyPatch(nameof(RaPlayerList.ReceiveData))]
    [HarmonyPatch(new Type[] { typeof(CommandSender), typeof(string) })]
    internal static class RaPlayerList_ReceiveData
    {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        public static bool Prefix(RaPlayerList __instance, CommandSender sender, string data)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        {
            try
            {
                string[] array = data.Split(' ');
                if (array.Length != 1)
                    return false;

                if (!int.TryParse(array[0], out int type))
                    return false;

                bool silent = type == 1;
                PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;

                if (playerCommandSender is null || LOFHPatch.DisabledFor.Contains(playerCommandSender.CharacterClassManager.UserId))
                    return true;

                try
                {
                    var senderPlayer = Player.Get(playerCommandSender);
                    var response = MenuManager.GetCurrentMenu(senderPlayer).HandlerPlayerlistRequest(senderPlayer);
                    sender.RaReply(string.Format("${0} {1}", __instance.DataId, response), true, !silent, string.Empty);
                    return false;
                }
                catch (Exception ex2)
                {
                    sender.RaReply(
                        string.Concat(
                            new string[]
                            {
                                data.ToUpper(),
                                ":PLAYER_LIST#An unexpected problem has occurred!\nMessage: ",
                                ex2.Message,
                                "\nStackTrace: ",
                                ex2.StackTrace,
                                "\nAt: ",
                                ex2.Source,
                            }),
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