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

        public string GetUsage() => "modRA true/false";

        public override string[] Execute(ICommandSender sender, string[] args, out bool _s)
        {
            _s = false;
            if (args.Length == 0)
                return new string[] { GetUsage() };
            if (!bool.TryParse(args[0], out bool value))
                return new string[] { GetUsage() };
            if (!value) // false -> Default RA | true -> Modified RA
                LOFHPatch.DisabledFor.Add(sender.GetPlayer().UserId);
            else
                LOFHPatch.DisabledFor.Remove(sender.GetPlayer().UserId);
            _s = true;
            return new string[] { "Done" };
        }
    }
}
