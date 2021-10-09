﻿// -----------------------------------------------------------------------
// <copyright file="ModdedRAHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using MEC;
using Mistaken.API;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;

namespace Mistaken.RAMod
{
    public class ModdedRAHandler : Module
    {
        public static readonly Dictionary<string, string> CountryCodes = new Dictionary<string, string>();
        public static readonly Dictionary<string, Dictionary<string, string>> Prefixes = new Dictionary<string, Dictionary<string, string>>();
        public static readonly Dictionary<string, Dictionary<string, string>> Flags = new Dictionary<string, Dictionary<string, string>>();

        public static void SetPrefix(string userId, string key, string value)
        {
            if (!Prefixes.ContainsKey(userId))
                Prefixes[userId] = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(value))
                Prefixes[userId].Remove(key);
            else
                Prefixes[userId][key] = value;
        }

        public ModdedRAHandler(PluginHandler plugin)
            : base(plugin)
        {
        }

        public override string Name => nameof(ModdedRAHandler);

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RestartingRound -= this.Handle(() => this.Server_RestartingRound(), "RestartingRound");
            Exiled.Events.Handlers.Server.WaitingForPlayers -= this.Handle(() => this.Server_WaitingForPlayers(), "WaitingForPlayers");
            Exiled.Events.Handlers.Player.PreAuthenticating -= this.Handle<Exiled.Events.EventArgs.PreAuthenticatingEventArgs>((ev) => this.Player_PreAuthenticating(ev));
        }

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RestartingRound += this.Handle(() => this.Server_RestartingRound(), "RestartingRound");
            Exiled.Events.Handlers.Server.WaitingForPlayers += this.Handle(() => this.Server_WaitingForPlayers(), "WaitingForPlayers");
            Exiled.Events.Handlers.Player.PreAuthenticating += this.Handle<Exiled.Events.EventArgs.PreAuthenticatingEventArgs>((ev) => this.Player_PreAuthenticating(ev));
        }

        private void Server_WaitingForPlayers()
        {
            this.RunCoroutine(this.RoundLoop(), "RoundLoop");
        }

        private IEnumerator<float> RoundLoop()
        {
            int rid = RoundPlus.RoundId;
            while (rid == RoundPlus.RoundId)
            {
                yield return Timing.WaitForSeconds(3);
                foreach (var player in RealPlayers.List.Where(x => x.RemoteAdminAccess))
                {
                    if (MenuManager.LastSelectedPlayer.TryGetValue(player, out var selected))
                    {
                        var response = MenuManager.Menus[0].HandlePlayerInfoRequest(player, selected.Type, selected.Id);
                        player.Sender.RaReply($"REQUEST_DATA:PLAYER#{response}", true, false, "PlayerInfo");
                    }
                }
            }
        }

        private void Server_RestartingRound()
        {
            MenuManager.LastSelectedPlayer.Clear();
        }

        private void Player_PreAuthenticating(Exiled.Events.EventArgs.PreAuthenticatingEventArgs ev)
        {
            if (((CentralAuthPreauthFlags)ev.Flags).HasFlagFast(CentralAuthPreauthFlags.NorthwoodStaff) && !ev.UserId.IsDevUserId())
                LOFHPatch.DisabledFor.Add(ev.UserId);

            ModdedRAHandler.CountryCodes[ev.UserId] = ev.Country;
        }
    }
}
