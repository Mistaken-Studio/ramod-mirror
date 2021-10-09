// -----------------------------------------------------------------------
// <copyright file="OldLOFH.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
/*

using Exiled.API.Features;
using Gamer.Mistaken.Base.Staff;
using Gamer.Utilities;
using System.Collections.Generic;

namespace Mistaken.RAMod
{
    public static class OldLOFH
    {
        public static string[] GetFlags(string userId)
        {
            List<string> flags = NorthwoodLib.Pools.ListPool<string>.Shared.Rent();
            if (InVanish.TryGetValue(userId, out int vanishLevel))
                flags.Add($"Active flag: <color=#808080>Vanish: <{vanishLevel}></color>");
            if (Flags.TryGetValue(userId, out Flag flag))
                flags.Add((flag.AddSM ? "Active flag: " : "") + flag.Value);

            var tor = flags.ToArray();
            NorthwoodLib.Pools.ListPool<string>.Shared.Return(flags);
            return tor;
        }

        public static string GetPrefixes(string userId)
        {
            string tor = "";
            if (MenuSystem.ReportIssuers.Contains(userId))
                tor = $"[<color=#CCCC00>I</color>] ";
            if (MenuSystem.Reported.Contains(userId))
                tor = $"[<color=#FFFF00>R</color>] ";
            if (userId.IsStaff())
                return $"{tor}[<color=blue>M</color> <color=white>STAFF</color>] ";
            return tor;
        }
    }
}*/