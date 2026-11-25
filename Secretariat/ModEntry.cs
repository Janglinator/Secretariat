using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buffs;

namespace Secretariat;

public sealed class ModEntry : Mod
{
    private int speedBoost;

    public override void Entry(IModHelper helper)
    {
        this.speedBoost = helper.ReadConfig<ModConfig>().SpeedBoost;
        helper.Events.GameLoop.UpdateTicking += this.OnUpdateTicking;
    }

    private void OnUpdateTicking(object sender, UpdateTickingEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        // SMAPI raises this event in each local screen's own player context.
        Farmer rider = Game1.player;
        string buffId = this.ModManifest.UniqueID;
        bool shouldApply = this.speedBoost != 0 && rider.isRidingHorse();
        if (rider.buffs.IsApplied(buffId) == shouldApply)
            return;

        if (shouldApply)
        {
            rider.applyBuff(new Buff(buffId, duration: Buff.ENDLESS,
                effects: new BuffEffects { Speed = { Value = this.speedBoost } })
            {
                visible = false
            });
        }
        else
        {
            rider.buffs.Remove(buffId);
        }
    }
}
