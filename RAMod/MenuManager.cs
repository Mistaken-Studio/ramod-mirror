// -----------------------------------------------------------------------
// <copyright file="MenuManager.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Features;
using Mistaken.API.Extensions;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mistaken.RAMod
{
    public static class MenuManager
    {
        public static readonly Dictionary<int, Menu> Menus = new Dictionary<int, Menu>();

        public static readonly Dictionary<Player, int> SelectedMenus = new Dictionary<Player, int>();

        public static readonly Dictionary<Player, int> LastSelectedPlayer = new Dictionary<Player, int>();

        static MenuManager()
        {
            Menus.Add(0, new RAMod.Menus.DefaultMenu());
        }

        public static Menu GetCurrentMenu(Player player)
        {
            if (SelectedMenus.TryGetValue(player, out int menuId))
                return Menus[menuId];
            else
                return Menus[0];
        }
    }

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
