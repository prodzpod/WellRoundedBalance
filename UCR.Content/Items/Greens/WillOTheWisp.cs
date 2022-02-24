﻿using MonoMod.Cil;
using UnityEngine;

namespace UltimateCustomRun.Items.Greens
{
    public class WillOTheWisp : ItemBase
    {
        public static float Damage;
        public static float StackDamage;
        public static float Radius;
        public static float StackRadius;
        public static float ProcCoefficient;

        public override string Name => ":: Items :: Greens :: Will O The Wisp";
        public override string InternalPickupToken => "explodeOnDeath";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "On killing an enemy, spawn a <style=cIsDamage>lava pillar</style> in a <style=cIsDamage>" + Radius + "m</style> <style=cStack>(+" + StackRadius + "m per stack)</style> Radius for <style=cIsDamage>" + d(Damage) + "</style> <style=cStack>(+" + d(StackDamage) + " per stack)</style> base Damage.";

        public override void Init()
        {
            Damage = ConfigOption(3.5f, "Base Damage", "Decimal. Vanilla is 3.5");
            StackDamage = ConfigOption(2.8f, "Stack Damage", "Decimal. Per Stack. Vanilla is 2.8");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient", "Decimal. Vanilla is 1");
            Radius = ConfigOption(12f, "Base Range", "Vanilla is 12");
            StackRadius = ConfigOption(2.4f, "Stack Range", "Per Stack. Vanilla is 2.4");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeDamage;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeRange;
            ChangeProc();
        }

        public static void ChangeDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(3.5f),
                x => x.MatchLdcR4(1f),
                x => x.MatchLdloc(out _),
                x => x.MatchLdcI4(1),
                x => x.MatchSub(),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.8f)
                // ik this is guh huge but i wanted to uhh Change all this stupid shit
            );
            c.Next.Operand = Damage;
            c.Index += 6;
            c.Next.Operand = StackDamage / Damage;
        }

        public static void ChangeRange(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(12f),
                x => x.MatchLdcR4(2.4f)
            );
            c.Next.Operand = Radius;
            c.Index += 1;
            c.Next.Operand = StackRadius;
        }

        public static void ChangeProc()
        {
            var w = Resources.Load<GameObject>("prefabs/networkedobjects/WilloWispDelay").GetComponent<RoR2.DelayBlast>();
            w.procCoefficient = ProcCoefficient;
        }
    }
}