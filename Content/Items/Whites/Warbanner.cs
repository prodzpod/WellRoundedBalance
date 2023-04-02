﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.Items;
using System;

namespace WellRoundedBalance.Items.Whites
{
    public class Warbanner : ItemBase
    {
        public override string Name => ":: Items : Whites :: Warbanner";
        public override ItemDef InternalPickup => RoR2Content.Items.WardOnLevel;

        public override string PickupText => $"Drop a Warbanner upon levelling up or encountering a boss. Grants allies{(enableMovementSpeed ? " movement speed" : "")}{(enableAttackSpeed ? (enableMovementSpeed ? " and" : "") + " attack speed" : "")}.";

        public override string DescText => "Upon <style=cIsUtility>levelling up</style> or <style=cIsUtility>encountering a boss</style>, drop a banner that strengthens all allies" +
            StackDesc(baseRadius, radiusPerStack, init => $" within <style=cIsUtility>{m(init)}</style>{{Stack}}", m) + "." +
            StackDesc(attackSpeedAndMovementSpeed, attackSpeedAndMovementSpeedStack, init => $" Raise {(enableMovementSpeed ? " <style=cIsUtility>movement speed</style>" : "")}{(enableAttackSpeed ? (enableMovementSpeed ? " and" : "") + " <style=cIsDamage>attack speed</style>" : "")} by <style=cIsDamage>{d(init)}</style>{{Stack}}.", d);

        [ConfigField("Base Radius", 20f)]
        public static float baseRadius;

        [ConfigField("Radius Per Stack", 0f)]
        public static float radiusPerStack;

        [ConfigField("Radius is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float radiusIsHyperbolic;

        [ConfigField("Attack Speed and Movement Speed", "Decimal.", 0.3f)]
        public static float attackSpeedAndMovementSpeed;

        [ConfigField("Attack Speed and Movement Speed Per Stack", "Decimal.", 0.15f)]
        public static float attackSpeedAndMovementSpeedStack;

        [ConfigField("Attack Speed and Movement Speed is Hyperbolic", "Decimal, Max value. Set to 0 to make it linear.", 0f)]
        public static float attackSpeedAndMovementSpeedIsHyperbolic;

        [ConfigField("Increase Movement Speed", true)]
        public static bool enableMovementSpeed;

        [ConfigField("Increase Attack Speed", true)]
        public static bool enableAttackSpeed;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.TeleporterInteraction.ChargingState.OnEnter += Change;
            IL.RoR2.Items.WardOnLevelManager.OnCharacterLevelUp += Change;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            // On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.OnEnter += BrotherEncounterPhaseBaseState_OnEnter;
            // On.EntityStates.Missions.Goldshores.GoldshoresBossfight.SpawnBoss += GoldshoresBossfight_SpawnBoss;
            On.RoR2.ScriptedCombatEncounter.BeginEncounter += ScriptedCombatEncounter_BeginEncounter;
        }

        private void ScriptedCombatEncounter_BeginEncounter(On.RoR2.ScriptedCombatEncounter.orig_BeginEncounter orig, ScriptedCombatEncounter self)
        {
            orig(self);
            if (NetworkServer.active)
                SpawnWarbanner();
        }

        private void GoldshoresBossfight_SpawnBoss(On.EntityStates.Missions.Goldshores.GoldshoresBossfight.orig_SpawnBoss orig, EntityStates.Missions.Goldshores.GoldshoresBossfight self)
        {
            orig(self);
            SpawnWarbanner();
        }

        private void SpawnWarbanner()
        {
            foreach (CharacterBody body in CharacterBody.readOnlyInstancesList)
            {
                if (body.isPlayerControlled && body.inventory)
                {
                    var stack = body.inventory.GetItemCount(RoR2Content.Items.WardOnLevel);
                    if (stack > 0)
                    {
                        var warbanner = UnityEngine.Object.Instantiate(WardOnLevelManager.wardPrefab, body.transform.position, Quaternion.identity);
                        warbanner.GetComponent<TeamFilter>().teamIndex = body.teamComponent.teamIndex;
                        warbanner.GetComponent<BuffWard>().Networkradius = baseRadius + radiusPerStack * (stack - 1);
                        NetworkServer.Spawn(warbanner);
                    }
                }
            }
        }

        private void BrotherEncounterPhaseBaseState_OnEnter(On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState self)
        {
            orig(self);
            SpawnWarbanner();
        }

        public static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = Util.GetItemCountForTeam(sender.teamComponent.teamIndex, RoR2Content.Items.WardOnLevel.itemIndex, false);
                if (sender.HasBuff(RoR2Content.Buffs.Warbanner.buffIndex))
                {
                    float ret = StackAmount(attackSpeedAndMovementSpeed, attackSpeedAndMovementSpeedStack, stack, attackSpeedAndMovementSpeedIsHyperbolic);
                    if (enableAttackSpeed) args.baseAttackSpeedAdd += ret - 0.3f;
                    if (enableMovementSpeed) args.moveSpeedMultAdd += ret - 0.3f;
                }
            }
        }

        public static void Change(ILContext il)
        {
            ILCursor c = new(il);
            int stack = GetItemLoc(c, nameof(RoR2Content.Items.WardOnLevel));
            if (stack != -1 && c.TryGotoNext(x => x.MatchCallOrCallvirt<BuffWard>("set_" + nameof(BuffWard.Networkradius))))
            {
                c.Emit(OpCodes.Pop);
                c.Emit(OpCodes.Ldloc, stack);
                c.EmitDelegate<Func<int, float>>(stack => StackAmount(baseRadius, radiusPerStack, stack, radiusIsHyperbolic));
            }
            else Logger.LogError("Failed to apply Warbanner Radius hook");
        }
    }
}