using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;

namespace Secretariat
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private ModConfig _config;
        //private int _addedSpeed = 0;
        private Dictionary<long, int> _buffByPlayerId = new Dictionary<long, int>();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            _config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsPlayerFree || !Context.IsWorldReady || Game1.paused
                || Game1.activeClickableMenu != null || !Context.IsMainPlayer)
                return;

            UpdateAddedSpeed();
        }

        private void UpdateAddedSpeed()
        {
            if (!Game1.IsMasterGame) { return; }

            foreach (var player in Game1.getAllFarmers())
            {
                log("player: " + player.UniqueMultiplayerID);

                int existingBuff = 0;
                int addedSpeed;
                _buffByPlayerId.TryGetValue(player.UniqueMultiplayerID, out addedSpeed);

                log("oldAddedSpeed: " + addedSpeed);

                if (player.addedSpeed > addedSpeed)
                {
                    // There's an existing buff
                    existingBuff = player.addedSpeed - addedSpeed;
                    log("existingBuff: " + existingBuff);
                }

                if (player.mount != null && addedSpeed == 0)
                {
                    addedSpeed = _config.SpeedBoost;
                }
                else if (player.mount == null && addedSpeed != 0)
                {
                    addedSpeed = 0;
                }

                log("newAddedSpeed: " + addedSpeed);

                _buffByPlayerId[player.UniqueMultiplayerID] = addedSpeed;
                player.addedSpeed = addedSpeed + existingBuff;
            }
        }

        private void log(string message, LogLevel level = LogLevel.Debug)
        {
#if DEBUG
            Monitor.Log(message, level);
#endif
        }
    }
}

