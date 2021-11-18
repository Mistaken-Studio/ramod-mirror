// -----------------------------------------------------------------------
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
    /// <summary>
    /// Modded RA Handler.
    /// </summary>
    public class ModdedRAHandler : Module
    {
        /// <summary>
        /// Dictionary containing country codes.
        /// </summary>
        public static readonly Dictionary<string, string> CountryCodes = new Dictionary<string, string>();

        /// <summary>
        /// Dictionary containing player prefixes (On RA's player list).
        /// </summary>
        public static readonly Dictionary<string, Dictionary<string, string>> Prefixes = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Sets prefix.
        /// </summary>
        /// <param name="userId">Target's userId.</param>
        /// <param name="key">Prefix name.</param>
        /// <param name="value">Prefix value.</param>
        public static void SetPrefix(string userId, string key, string value)
        {
            if (!Prefixes.ContainsKey(userId))
                Prefixes[userId] = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(value))
                Prefixes[userId].Remove(key);
            else
                Prefixes[userId][key] = value;
        }

        /// <inheritdoc/>
        public override string Name => nameof(ModdedRAHandler);

        /// <inheritdoc/>
        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RestartingRound -= this.Server_RestartingRound;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= this.Server_WaitingForPlayers;
            Exiled.Events.Handlers.Player.PreAuthenticating -= this.Player_PreAuthenticating;
        }

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RestartingRound += this.Server_RestartingRound;
            Exiled.Events.Handlers.Server.WaitingForPlayers += this.Server_WaitingForPlayers;
            Exiled.Events.Handlers.Player.PreAuthenticating += this.Player_PreAuthenticating;
        }

        internal ModdedRAHandler(PluginHandler plugin)
            : base(plugin)
        {
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
