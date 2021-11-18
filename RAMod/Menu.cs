// -----------------------------------------------------------------------
// <copyright file="Menu.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Features;
using Mistaken.API.Extensions;
using RemoteAdmin;

namespace Mistaken.RAMod
{

    public abstract class Menu
    {
        public abstract int Id { get; }

        public abstract int ParrentId { get; }

        public abstract string EnterButton(Player sender);

        public virtual string ExitButton(Player sender)
        {
            return $"<size=1px><color=0000>({8000 + this.ParrentId})</color></size><color=green>[<color=yellow>MENU</color>]         BACK</color>\n";
        }

        public abstract string HandlerPlayerlistRequest(Player sender);

        public abstract string HandlePlayerInfoRequest(Player sender, int type, params int[] playerIds);
    }
}
