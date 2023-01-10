﻿using MonoMod.Cil;
using R2API;
using RoR2;

namespace WellRoundedBalance.Items.Yellows
{
    public class Shatterspleen : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Shatterspleen";
        public override string InternalPickupToken => "bleedOnHitAndExplode";

        public override string PickupText => "Critical strikes always bleed enemies. Bleeding enemies now explode.";

        public override string DescText => "<style=cIsDamage>Critical Strikes bleed</style> enemies for <style=cIsDamage>240%</style> base damage. <style=cIsDamage>Bleeding</style> enemies <style=cIsDamage>explode</style> on death for <style=cIsDamage>200%</style> <style=cStack>(+200% per stack)</style> damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += Changes;
        }

        public static void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(4f),
                    x => x.MatchLdcI4(1),
                    x => x.MatchLdloc(out _)))
            {
                c.Next.Operand = 2f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Shatterspleen Base Explosion Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.15f),
                    x => x.MatchLdcI4(1),
                    x => x.MatchLdloc(out _)))
            {
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Shatterspleen Percent Explosion Damage hook");
            }
        }
    }
}