// -----------------------------------------------------------------------
// <copyright file="Handler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;

namespace Mistaken.RAMod
{
    internal class Handler : Module
    {
        public Handler(PluginHandler plugin)
            : base(plugin)
        {
        }

        public override bool IsBasic => true;

        public override string Name => "LOFH";

        public override void OnEnable()
        {
            // Exiled.Events.Handlers.Server.RestartingRound += this.Handle(() => Server_RestartingRound(), "RoundRestart");
            // Exiled.Events.Handlers.Server.RoundStarted += this.Handle(() => Server_RoundStarted(), "RoundStart");

            /*this.CallDelayed(10, () =>
            {
                SSL.Client.Send(MessageType.BLACKLIST_GET_REQUEST, null).GetResponseDataCallback((response) =>
                {
                    if (response.Type == MistakenSocket.Shared.API.ResponseType.OK)
                    {
                        var data = response.Payload.Deserialize<MistakenSocket.Shared.Blacklist.BlacklistEntry[]>(0, 0, out _, false);

                        OldLOFH.WarningFlags.Clear();
                        foreach (var item in data)
                            OldLOFH.WarningFlags.Add(item.UserId, item);
                    }
                });
            }, "RequestBlacklist");*/
        }

        public override void OnDisable()
        {
            // Exiled.Events.Handlers.Server.RestartingRound -= this.Handle(() => Server_RestartingRound(), "RoundRestart");
            // Exiled.Events.Handlers.Server.RoundStarted -= this.Handle(() => Server_RoundStarted(), "RoundStart");
        }

        /*
        private void Server_RoundStarted()
        {
            this.RunCoroutine(DoRoundLoop(), "RoundLoop");
        }

        private IEnumerator<float> DoRoundLoop()
        {
            yield return Timing.WaitForSeconds(1);
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(3);
                try
                {
                    foreach (var player in RealPlayers.List.Where(p => p.RemoteAdminAccess))
                    {
                        try
                        {
                            if (OldLOFH.LastSelectedPlayer.TryGetValue(player, out string query) && MenuSystem.CurrentMenus[player.Id] == 0)
                            {
                                int id = int.Parse(query.Split(' ')[2].Split('.')[0]);
                                if (Player.Get(id) != null)
                                    LOFHPatch.Prefix(query + " SILENT", player.Sender);
                                else
                                {
                                    Log.Debug($"Player with id {id} not found | {query}");
                                    player.SendConsoleMessage($"Player with id {id} not found | {query}", "grey");
                                    OldLOFH.LastSelectedPlayer.Remove(player);
                                }
                            }
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error(ex.Message);
                            Log.Error(ex.StackTrace);
                        }
                    }
                }
                catch(System.Exception ex)
                {
                    Log.Error(ex.Message);
                    Log.Error(ex.StackTrace);
                }
            }
        }
        private void Server_RestartingRound()
        {
            MenuSystem.CurrentMenus.Clear();
            MenuSystem.PlayerLogSelectedRound.Clear();
            MenuSystem.InMuteMode.Clear();
            MenuSystem.AutoRepeat.Clear();
            MenuSystem.ReportSelectedReport.Clear();
        }
        */
    }
}
