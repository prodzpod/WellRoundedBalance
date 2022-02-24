﻿using MonoMod.Cil;

namespace UltimateCustomRun.Items.Greens
{
    public class FuelCell : ItemBase
    {
        public static float CooldownReduction;

        public override string Name => ":: Items :: Greens :: Fuel Cell";
        public override string InternalPickupToken => "equipmentMagazine";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Hold an <style=cIsUtility>additional equipment charge</style> <style=cStack>(+1 per stack)</style>. <style=cIsUtility>Reduce equipment Cooldown</style> by <style=cIsUtility>" + d(CooldownReduction) + "</style> <style=cStack>(+" + d(CooldownReduction) + " per stack)</style>.";

        public override void Init()
        {
            CooldownReduction = ConfigOption(0.15f, "Equipment Cooldown Reduction", "Decimal. Per Stack. Vanilla is 0.15");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Inventory.CalculateEquipmentCooldownScale += ChangeCDR;
        }

        public static void ChangeCDR(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStloc(out _),
                x => x.MatchLdcR4(0.85f)
            );
            c.Index += 1;
            c.Next.Operand = 1f - CooldownReduction;
        }
    }
}