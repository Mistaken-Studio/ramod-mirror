// -----------------------------------------------------------------------
// <copyright file="PluginHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;

namespace Mistaken.RAMod
{
    /// <inheritdoc/>
    public class PluginHandler : Plugin<Config>
    {
        /// <inheritdoc/>
        public override string Author => "Mistaken Devs";

        /// <inheritdoc/>
        public override string Name => "ModdedRA";

        /// <inheritdoc/>
        public override string Prefix => "MMODRA";

        /// <inheritdoc/>
        public override PluginPriority Priority => PluginPriority.Medium + 1;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => new Version(3, 0, 0, 57);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            var harmony = new Harmony("com.mistaken.modra");
            harmony.PatchAll();

            new ModdedRAHandler(this);

            API.Diagnostics.Module.OnEnable(this);

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            API.Diagnostics.Module.OnDisable(this);

            base.OnDisabled();
        }

        internal static PluginHandler Instance { get; private set; }
    }
}
