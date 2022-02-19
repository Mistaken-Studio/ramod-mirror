// -----------------------------------------------------------------------
// <copyright file="LOFHPatch.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using HarmonyLib;
using Mistaken.API.Extensions;
using RemoteAdmin;
using UnityEngine;

namespace Mistaken.RAMod
{
    [HarmonyPatch(typeof(CommandProcessor))]
    [HarmonyPatch("ProcessQuery")]
    [HarmonyPatch(new Type[] { typeof(string), typeof(CommandSender) })]
    internal static class LOFHPatch
    {
        public static readonly HashSet<string> DisabledFor = new HashSet<string>();

        public static bool Prefix(string q, CommandSender sender)
        {
            try
            {
                if (sender == null)
                    return true;
                if (!sender.IsPlayer())
                    return true;
                var senderPlayer = sender.GetPlayer();
                if (DisabledFor.Contains(senderPlayer.UserId))
                    return true;
                GameObject senderGameObject = senderPlayer.GameObject;

                string[] query = q.Split(' ');

                switch (query[0].ToUpper())
                {
                    case "BAN":
                        {
                            // Only if CustomMenu is active
                            return true;

                            /* if (query[1].Contains("."))
                             {
                                 var tmp = query[1].Split('.');
                                 query[1] = tmp[0];
                             }
                             if (!int.TryParse(query[1], out int playerId))
                             {
                                 sender.RaReply(query[0].ToUpper() + "#Invalid Player: " + query[1], false, true, "");
                                 return false;
                             }
                             string reason = string.Empty;
                             if (query.Length > 3)
                             {
                                 reason = query.Skip(3).Aggregate((string current, string n) => current + " " + n);
                             }
                             int duration = 0;
                             try
                             {
                                 duration = Misc.RelativeTimeToSeconds(query[2], 60);
                             }
                             catch
                             {
                                 sender.RaReply(query[0].ToUpper() + "#Invalid time: " + query[2], false, true, "");
                                 return false;
                             }
                             if (duration < 0)
                             {
                                 duration = 0;
                                 query[2] = "0";
                             }
                             sender.RaReply($"{query[0].ToUpper()}#<size=100%>{MenuSystem.ProccessPress(senderPlayer, playerId, duration, reason)}</size>", true, true, "");
                             return false;*/
                        }

                    default:
                        return true;
                }
            }
            catch (System.Exception e)
            {
                Exiled.API.Features.Log.Error($"[LOFHBase Late Error Catch] {q}");
                Exiled.API.Features.Log.Error($"[LOFHBase Late Error Catch] {e.Message}");
                Exiled.API.Features.Log.Error($"[LOFHBase Late Error Catch] {e.StackTrace}");
                return true;
            }
        }
    }
}