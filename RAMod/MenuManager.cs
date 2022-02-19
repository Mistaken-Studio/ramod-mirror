// -----------------------------------------------------------------------
// <copyright file="MenuManager.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

#pragma warning disable

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

        public static readonly Dictionary<Player, (int Id, int Type)> LastSelectedPlayer = new Dictionary<Player, (int Id, int Type)>();

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
}
