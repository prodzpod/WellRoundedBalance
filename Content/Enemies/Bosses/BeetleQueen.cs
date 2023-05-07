﻿using EntityStates;
using Inferno.Stat_AI;
using RoR2.Skills;
using System;

namespace WellRoundedBalance.Enemies.Bosses
{
    internal class BeetleQueen : EnemyBase<BeetleQueen>
    {
        public override string Name => "::: Bosses :: Beetle Queen";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.BeetleQueenMonster.SummonEggs.OnEnter += SummonEggs_OnEnter;
            On.EntityStates.BeetleQueenMonster.FireSpit.OnEnter += FireSpit_OnEnter;
            On.EntityStates.BeetleQueenMonster.SpawnWards.OnEnter += SpawnWards_OnEnter;
            On.RoR2.CharacterAI.BaseAI.Awake += BaseAI_Awake;
            Changes();
        }

        private void BaseAI_Awake(On.RoR2.CharacterAI.BaseAI.orig_Awake orig, BaseAI self)
        {
            orig(self);
            if (self.master && self.master.masterIndex == MasterCatalog.FindMasterIndex("BeetleQueenMaster"))
            {
                Main.WRBLogger.LogError("beetle queen spawned");
                if (self.skillDrivers[0].customName != "SummonEarthquake")
                {
                    var driversList = self.skillDrivers.ToList();
                    var lastNumber = driversList[driversList.Count - 1];
                    driversList.RemoveAt(driversList.Count - 1);
                    driversList.Insert(0, lastNumber);
                    self.skillDrivers = driversList.ToArray();
                }
            }
        }

        public static CharacterMaster queen = Utils.Paths.GameObject.BeetleQueenMaster.Load<GameObject>().GetComponent<CharacterMaster>();

        private void SpawnWards_OnEnter(On.EntityStates.BeetleQueenMonster.SpawnWards.orig_OnEnter orig, EntityStates.BeetleQueenMonster.SpawnWards self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.BeetleQueenMonster.SpawnWards.baseDuration = 3f;
                EntityStates.BeetleQueenMonster.SpawnWards.orbTravelSpeed = 20f;
            }

            orig(self);
        }

        private void FireSpit_OnEnter(On.EntityStates.BeetleQueenMonster.FireSpit.orig_OnEnter orig, EntityStates.BeetleQueenMonster.FireSpit self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.BeetleQueenMonster.FireSpit.damageCoefficient = 0.4f;
                EntityStates.BeetleQueenMonster.FireSpit.force = 1200f;
                EntityStates.BeetleQueenMonster.FireSpit.yawSpread = 20f;
                EntityStates.BeetleQueenMonster.FireSpit.minSpread = 15f;
                EntityStates.BeetleQueenMonster.FireSpit.maxSpread = 30f;
                EntityStates.BeetleQueenMonster.FireSpit.projectileHSpeed = 40f;
                EntityStates.BeetleQueenMonster.FireSpit.projectileCount = 10;
            }
            orig(self);
        }

        private void SummonEggs_OnEnter(On.EntityStates.BeetleQueenMonster.SummonEggs.orig_OnEnter orig, EntityStates.BeetleQueenMonster.SummonEggs self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.BeetleQueenMonster.SummonEggs.summonInterval = 2f;
                EntityStates.BeetleQueenMonster.SummonEggs.randomRadius = 13f;
                EntityStates.BeetleQueenMonster.SummonEggs.baseDuration = 3f;
            }

            orig(self);
        }

        private void Changes()
        {
            ContentAddition.AddEntityState(typeof(Earthquake), out _);

            var beetleQueen = Utils.Paths.GameObject.BeetleQueen2Body9.Load<GameObject>();

            var esm = beetleQueen.AddComponent<EntityStateMachine>();
            esm.customName = "Earthquake";
            esm.initialStateType = new(typeof(EntityStates.BeetleQueenMonster.SpawnState));
            esm.mainStateType = new(typeof(GenericCharacterMain));

            var nsm = beetleQueen.GetComponent<NetworkStateMachine>();
            Array.Resize(ref nsm.stateMachines, nsm.stateMachines.Length + 1);
            nsm.stateMachines[nsm.stateMachines.Length - 1] = esm;

            var utilitySD = ScriptableObject.CreateInstance<SkillDef>();
            utilitySD.activationState = new SerializableEntityStateType(typeof(Earthquake));
            utilitySD.activationStateMachineName = "Earthquake";
            utilitySD.interruptPriority = InterruptPriority.Skill;
            utilitySD.baseRechargeInterval = 10f;
            utilitySD.baseMaxStock = 1;
            utilitySD.rechargeStock = 1;
            utilitySD.requiredStock = 1;
            utilitySD.stockToConsume = 1;
            utilitySD.resetCooldownTimerOnUse = false;
            utilitySD.fullRestockOnAssign = true;
            utilitySD.dontAllowPastMaxStocks = false;
            utilitySD.beginSkillCooldownOnSkillEnd = false;
            utilitySD.cancelSprintingOnActivation = true;
            utilitySD.forceSprintDuringState = false;
            utilitySD.beginSkillCooldownOnSkillEnd = true;
            utilitySD.isCombatSkill = true;
            utilitySD.mustKeyPress = false;
            (utilitySD as ScriptableObject).name = "EarthquakeSkill";

            ContentAddition.AddSkillDef(utilitySD);

            var utilityFamily = ScriptableObject.CreateInstance<SkillFamily>();
            Array.Resize(ref utilityFamily.variants, 1);
            utilityFamily.variants[0].skillDef = utilitySD;
            (utilityFamily as ScriptableObject).name = "UtilityFamily";

            ContentAddition.AddSkillFamily(utilityFamily);

            var utility = beetleQueen.AddComponent<GenericSkill>();
            utility._skillFamily = utilityFamily;

            var master = Utils.Paths.GameObject.BeetleQueenMaster.Load<GameObject>();
            var ed = master.AddComponent<AISkillDriver>();
            ed.customName = "SummonEarthquake";
            ed.skillSlot = SkillSlot.Utility;
            ed.requireSkillReady = true;
            ed.minUserHealthFraction = Mathf.NegativeInfinity;
            ed.maxUserHealthFraction = Mathf.Infinity;
            ed.minTargetHealthFraction = Mathf.NegativeInfinity;
            ed.maxTargetHealthFraction = Mathf.Infinity;
            ed.minDistance = 13f;
            ed.maxDistance = 50f;
            ed.selectionRequiresTargetLoS = false;
            ed.selectionRequiresOnGround = false;
            ed.selectionRequiresAimTarget = false;
            ed.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            ed.activationRequiresAimTargetLoS = false;
            ed.activationRequiresAimConfirmation = false;
            ed.activationRequiresTargetLoS = false;
            ed.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            ed.moveInputScale = 1;
            ed.aimType = AISkillDriver.AimType.AtMoveTarget;
            ed.ignoreNodeGraph = false;
            ed.shouldSprint = false;
            ed.shouldFireEquipment = false;
            ed.shouldTapButton = false;
            ed.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            ed.driverUpdateTimerOverride = -1;
            ed.resetCurrentEnemyOnNextDriverSelection = false;
            ed.noRepeat = false;

            var locator = beetleQueen.GetComponent<SkillLocator>();
            locator.utility = utility;

            var summonBeetleGuards = Utils.Paths.SkillDef.BeetleQueen2BodySummonEggs.Load<SkillDef>();
            summonBeetleGuards.baseRechargeInterval = 60f;

            var spitProjectile = Utils.Paths.GameObject.BeetleQueenSpit.Load<GameObject>();
            var projectileImpactExplosion = spitProjectile.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;

            var spitDoT = Utils.Paths.GameObject.BeetleQueenAcid.Load<GameObject>();
            var projectileDotZone = spitDoT.GetComponent<ProjectileDotZone>();
            projectileDotZone.lifetime = 9f;
            projectileDotZone.damageCoefficient = 3f;
            spitDoT.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);

            var hitBox = spitDoT.transform.GetChild(0).GetChild(2);
            hitBox.localPosition = new Vector3(0f, 0f, -0.5f);
            hitBox.localScale = new Vector3(4f, 1.5f, 4f);

            var beetleWard = Utils.Paths.GameObject.BeetleWard.Load<GameObject>();
            var buffWard = beetleWard.GetComponent<BuffWard>();
            buffWard.radius = 7f;
            buffWard.interval = 0.5f;
            buffWard.buffDuration = 3f;
            buffWard.expireDuration = 10f;

            var egg = Utils.Paths.SkillDef.BeetleQueen2BodySpawnWards.Load<SkillDef>();
            egg.baseRechargeInterval = 12f;
        }
    }

    public class Earthquake : BaseState
    {
        public static float baseDuration = 3f;

        public static float baseDurationUntilRecastInterrupt = 1.5f;

        public static string soundString = "Play_beetle_guard_impact";

        public static GameObject waveProjectilePrefab = Utils.Paths.GameObject.BrotherSunderWave.Load<GameObject>();

        public static int waveProjectileCount = 12;

        public static float waveProjectileDamageCoefficient = 1f;

        public static float waveProjectileForce = 1000f;

        public static SkillDef replacementSkillDef;

        public float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            Util.PlaySound(soundString, gameObject);
            PlayAnimation("Body", "ExitSkyLeap", "SkyLeap.playbackRate", duration);
            PlayAnimation("FullBody Override", "BufferEmpty");
            characterBody.AddTimedBuff(RoR2Content.Buffs.ArmorBoost, baseDuration);
            AimAnimator aimAnimator = GetAimAnimator();
            if (aimAnimator)
            {
                aimAnimator.enabled = true;
            }
            if (isAuthority)
            {
                FireRingAuthority();
            }
        }

        private void FireRingAuthority()
        {
            float num = 360f / waveProjectileCount;
            Vector3 vector = Vector3.ProjectOnPlane(inputBank.aimDirection, Vector3.up);
            Vector3 footPosition = characterBody.footPosition;
            for (int i = 0; i < waveProjectileCount; i++)
            {
                Vector3 vector2 = Quaternion.AngleAxis(num * i, Vector3.up) * vector;
                if (isAuthority)
                {
                    ProjectileManager.instance.FireProjectile(waveProjectilePrefab, footPosition, Util.QuaternionSafeLookRotation(vector2), gameObject, characterBody.damage * waveProjectileDamageCoefficient, waveProjectileForce, Util.CheckRoll(characterBody.crit, characterBody.master), DamageColorIndex.Default, null, -1f);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (fixedAge > duration)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}