// -----------------------------------------------------------------------
// <copyright file="ModRACommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;

namespace Mistaken.RAMod
{
    [CommandSystem.CommandHandler(typeof(CommandSystem.RemoteAdminCommandHandler))]
    [CommandSystem.CommandHandler(typeof(CommandSystem.ClientCommandHandler))]
    internal class ModRACommand : IBetterCommand
    {
        public override string Description => "Toggle default RA";

        public override string Command => "modRA";

        public override string[] Aliases => new string[] { "raMod" };

        public string GetUsage() => "modRA enable/disable/stream";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            if (args.Length == 0)
                return new string[] { this.GetUsage() };
            bool value;
            var player = sender.GetPlayer();
            switch (args[0].ToLower())
            {
                case "true":
                case "enable":
                    value = true;
                    break;

                case "false":
                case "disable":
                    value = false;
                    break;

                case "stream":
                case "smode":
                case "stream-mode":
                    success = true;
                    player.SetSessionVariable(API.SessionVarType.STREAMER_MODE, !player.GetSessionVariable<bool>(API.SessionVarType.STREAMER_MODE));
                    return new string[] { "Streamer mode " + (player.GetSessionVariable<bool>(API.SessionVarType.STREAMER_MODE) ? "<color=green>Enabled</color>" : "<color=red>Disabled</color>") };

                default:
                    return new string[] { "Inavlid argument", this.GetUsage() };
            }

            if (!value) // false -> Default RA | true -> Modified RA
                LOFHPatch.DisabledFor.Add(player.UserId);
            else
                LOFHPatch.DisabledFor.Remove(player.UserId);
            success = true;
            return new string[] { "Done" };
        }
    }
}
