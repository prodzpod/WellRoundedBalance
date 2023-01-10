﻿using Mono.Cecil.Cil;
using MonoMod.Cil;

using RoR2;
using RoR2.Items;
using System;
using System.Collections.ObjectModel;

namespace WellRoundedBalance.Items.Yellows
{
    public class QueensGland : ItemBase
    {
        public override string Name => ":: Items :::: Yellows :: Queens Gland";
        public override string InternalPickupToken => "beetleGland";

        public override string PickupText => "Recruit 3 Beetle Guards.";

        public override string DescText => "<style=cIsUtility>Summon 3 Beetle Guards</style> with bonus <style=cIsDamage>300%</style> <style=cStack>(+200% per stack)</style> damage and <style=cIsHealing>100%</style> <style=cIsHealing>health</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += ChangeLimit2;
            IL.RoR2.Items.BeetleGlandBodyBehavior.FixedUpdate += FuckYou;
            On.RoR2.CharacterMaster.OnBodyStart += CharacterMaster_OnBodyStart;
        }

        private void CharacterMaster_OnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);
            int stack = 0;
            ReadOnlyCollection<CharacterMaster> readOnlyInstancesList = CharacterMaster.readOnlyInstancesList;
            for (int i = 0; i < readOnlyInstancesList.Count; i++)
            {
                CharacterMaster characterMaster = readOnlyInstancesList[i];
                if (characterMaster.teamIndex == TeamIndex.Player)
                {
                    stack += characterMaster.inventory.GetItemCount(RoR2Content.Items.BeetleGland);
                }
            }
            if (self.name == "BeetleGuardAllyMaster(Clone)")
            {
                self.inventory.GiveItem(RoR2Content.Items.BoostDamage, 20 * (stack - 1));
                // this works I checked :smirk_cat:
            }
        }

        private void FuckYou(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld("RoR2.Items.BaseItemBodyBehavior", "stack")))
            {
                c.Index += 1;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<int, BaseItemBodyBehavior, int>>((useless, self) =>
                {
                    return 3;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Queens Gland Count hook");
            }
        }

        private int ChangeLimit2(On.RoR2.CharacterMaster.orig_GetDeployableSameSlotLimit orig, CharacterMaster self, DeployableSlot slot)
        {
            if (slot is DeployableSlot.BeetleGuardAlly)
            {
                return 3;
            }
            else
            {
                return orig(self, slot);
            }
        }
    }
}