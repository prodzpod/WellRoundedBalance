﻿using R2API.Utils;
using RoR2;
using RoR2.Skills;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using BepInEx.Configuration;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    public class Ukulele : ItemBase
    {
        public static float damage;
        public static float chance;
        public static int targets;
        public static int targetstack;
        public static float range;
        public static int rangestack;
        public static float procco;

        public override string Name => ":: Items :: Greens :: Ukulele";
        public override string InternalPickupToken => "chainLightning";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsDamage>" + chance + "%</style> chance to fire <style=cIsDamage>chain lightning</style> for <style=cIsDamage>" + d(damage) + "</style> TOTAL damage on up to <style=cIsDamage>" + targets + " <style=cStack>(+" + targetstack + " per stack)</style></style> targets within <style=cIsDamage>" + range + "m</style> <style=cStack>(+" + rangestack + "m per stack)</style>.";


        public override void Init()
        {
            damage = ConfigOption(0.8f, "Total Damage", "Decimal. Vanilla is 0.8");
            chance = ConfigOption(25f, "Chance", "Vanilla is 25");
            procco = ConfigOption(0.2f, "Proc Coefficient", "Decimal. Vanilla is 0.2");
            range = ConfigOption(20f, "Base Range", "Vanilla is 20");
            rangestack = ConfigOption(20, "Stack Range", "Per Stack. Vanilla is 2");
            targets = ConfigOption(3, "Base Max Targets", "Vanilla is 3");
            targetstack = ConfigOption(2, "Stack Max Targets", "Per Stack. Vanilla is 2");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeChance;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeDamage;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeProcCo;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeRangeStack;
            IL.RoR2.GlobalEventManager.OnHitEnemy += ChangeTargetCountStack;
            ChangeTargetCountBase();
            ChangeRangeBase();
        }
        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt(typeof(RoR2.Util).GetMethod("CheckRoll", new Type[] { typeof(float), typeof(CharacterMaster) } )),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdcR4(0.8f)
            );
            c.Index += 2;
            c.Next.Operand = damage;
            // oh wow util.checkroll is stupid why tf are there two methods named the same
            // thank you harb :)
        }
        public static void ChangeChance(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "ChainLightning"),
                x => x.MatchCallOrCallvirt<RoR2.Inventory>("GetItemCount"),
                x => x.MatchStloc(out _),
                x => x.MatchLdcR4(25f)
            );
            c.Index += 3;
            c.Next.Operand = chance;
        }
        public static void ChangeTargetCountStack(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchStfld<RoR2.Orbs.LightningOrb>("isCrit"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(2)
            );
            c.Index += 2;
            c.Next.Operand = targetstack;
        }

        public static void ChangeTargetCountBase()
        {
            On.RoR2.Orbs.LightningOrb.Begin += (orig, self) =>
            {
                orig(self);
                if (self.lightningType is LightningOrb.LightningType.Ukulele)
                {
                    self.bouncesRemaining = targets;
                }
            };
        }

        public static void ChangeRangeBase()
        {
            On.RoR2.Orbs.LightningOrb.Begin += (orig, self) =>
            {
                orig(self);
                if (self.lightningType is LightningOrb.LightningType.Ukulele)
                {
                    self.range = range;
                    // self.canBounceOnSameTarget = :TROLLGE:
                    // im scared of the warning :IL:
                }
                
            };
        }

        public static void ChangeRangeStack(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld<RoR2.Orbs.LightningOrb>("range"),
                x => x.MatchLdcI4(2)
            );
            c.Index += 1;
            c.Next.Operand = rangestack;
        }

        public static void ChangeProcCo(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdflda<RoR2.Orbs.LightningOrb>("procChainMask"),
                x => x.MatchLdcI4(3),
                x => x.MatchCallOrCallvirt<RoR2.ProcChainMask>("AddProc"),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcR4(0.2f)

            );
            c.Index += 4;
            c.Next.Operand = procco;
        }
    }
}
