// -----------------------------------------------------------------------
// <copyright file="PlayerCommandSender_RaReply.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Exiled.API.Features;
using HarmonyLib;
using Mistaken.API.Extensions;
using RemoteAdmin;
using RemoteAdmin.Communication;

namespace Mistaken.RAMod
{
    [HarmonyPatch(typeof(PlayerCommandSender), nameof(PlayerCommandSender.RaReply))]
    internal static class PlayerCommandSender_RaReply
    {
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        public static bool Prefix(PlayerCommandSender __instance, ref string text, bool success, bool logToConsole, string overrideDisplay)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        {
            try
            {
                if (!__instance.GetPlayer().GetSessionVariable<bool>(API.SessionVarType.STREAMER_MODE))
                    return true;
                text = Regex.Replace(text, "(\\d{17}@steam)|(\\d{18}@discord)|(\\w*@northwood)|(\\d{1,3}\\.\\d{1,3}\\.\\d{1,3}\\.\\d{1,3})", "<color=purple>Hidden By Streamer Mode</color>");
            }
            catch (System.Exception e)
            {
                Exiled.API.Features.Log.Error($"[StreamerMode Error Catch] {text}");
                Exiled.API.Features.Log.Error($"[StreamerMode Error Catch] {e.Message}");
                Exiled.API.Features.Log.Error($"[StreamerMode Error Catch] {e.StackTrace}");
            }

            return true;
        }
    }

}