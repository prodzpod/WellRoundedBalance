﻿using MonoMod.Cil;

namespace UltimateCustomRun.Items.Whites
{
    public class SoldiersSyringe : ItemBase
    {
        public static float Duration;

        public override string Name => ":: Items : Whites :: Soldiers Syringe";
        public override string InternalPickupToken => "syringe";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "Increases <style=cIsDamage>attack Speed</style> by <style=cIsDamage>" + d(Duration) + " <style=cStack>(+" + d(Duration) + " per stack)</style></style>.";

        public override void Init()
        {
            Duration = ConfigOption(0.15f, "Attack Speed", "Decimal. Per Stack. Vanilla is 0.15");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += SoldiersSyringe.ChangeAS;
        }

        public static void ChangeAS(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.15f)
            );
            c.Index += 1;
            c.Next.Operand = Duration;
        }
    }
}