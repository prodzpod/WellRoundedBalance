﻿using RoR2;
using UnityEngine;
using System.Linq;
using RoR2.CharacterAI;
using RoR2.Projectile;
using Rewired.ComponentControls.Effects;
using RoR2.Skills;
using UnityEngine.Networking;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
// fuck off

namespace UltimateCustomRun.Enemies.Bosses
{
    public static class MithrixPhase1And3
    {
        public static CharacterMaster master;
        public static void Buff()
        {
            master = Resources.Load<CharacterMaster>("prefabs/charactermasters/BrotherMaster").GetComponent<CharacterMaster>();

            AISkillDriver ai = (from x in master.GetComponents<AISkillDriver>()
                                where x.customName == "Sprint and FireLunarShards"
                                select x).First();
            ai.minDistance = 13f;
            ai.maxUserHealthFraction = Mathf.Infinity;

            AISkillDriver ai2 = (from x in master.GetComponents<AISkillDriver>()
                                 where x.customName == "CastUlt"
                                 select x).First();
            ai2.movementType = AISkillDriver.MovementType.StrafeMovetarget;

            On.EntityStates.BrotherMonster.HoldSkyLeap.OnEnter += (orig, self) =>
            {
                EntityStates.BrotherMonster.HoldSkyLeap.duration = 2f;
                orig(self);
            };

            On.EntityStates.BrotherMonster.ExitSkyLeap.OnEnter += (orig, self) =>
            {
                EntityStates.BrotherMonster.ExitSkyLeap.waveProjectileCount = 20;
                EntityStates.BrotherMonster.ExitSkyLeap.waveProjectileDamageCoefficient = 1f;
                orig(self);
            };

            On.EntityStates.BrotherMonster.WeaponSlam.OnEnter += (orig, self) =>
            {
                EntityStates.BrotherMonster.WeaponSlam.duration = 3f;
                EntityStates.BrotherMonster.WeaponSlam.waveProjectileArc = 360f;
                EntityStates.BrotherMonster.WeaponSlam.waveProjectileCount = 6;
                EntityStates.BrotherMonster.WeaponSlam.waveProjectileDamageCoefficient = 1f;
                EntityStates.BrotherMonster.WeaponSlam.waveProjectileForce = -200f;
                EntityStates.BrotherMonster.WeaponSlam.damageCoefficient = 8f;
                EntityStates.BrotherMonster.WeaponSlam.weaponDamageCoefficient = 0f;
                EntityStates.BrotherMonster.WeaponSlam.weaponForce = 2000f;
                if (self.modelAnimator && self.modelAnimator.GetFloat("blast.hitBoxActive") > 0.1f)
                {
                    FireWavesSlam(self);
                }
                orig(self);
            };
            On.EntityStates.BrotherMonster.WeaponSlam.OnExit += (orig, self) =>
            {
                // FireWavesSlam(self);
                orig(self);
            };

            On.EntityStates.BrotherMonster.BaseSlideState.OnEnter += (orig, self) =>
            {
                if (self is EntityStates.BrotherMonster.SlideBackwardState)
                {
                    self.slideRotation = Quaternion.identity;
                }
                orig(self);
            };

            On.EntityStates.BrotherMonster.Weapon.FireLunarShards.OnEnter += (orig, self) =>
            {
                EntityStates.BrotherMonster.Weapon.FireLunarShards.spreadBloomValue = 20f;
                EntityStates.BrotherMonster.Weapon.FireLunarShards.recoilAmplitude = 2f;
                EntityStates.BrotherMonster.Weapon.FireLunarShards.baseDuration = 0.03f;
                orig(self);
            };

            On.EntityStates.BrotherMonster.SprintBash.OnEnter += (orig, self) =>
            {
                EntityStates.BrotherMonster.SprintBash.durationBeforePriorityReduces = 0.2f;
                self.baseDuration = 1.5f;
                self.damageCoefficient = 1.5f;
                self.pushAwayForce = 2000f;
                self.forceVector = new Vector3(0f, 1000f, 0f);
                orig(self);
            };

            var sunder = Resources.Load<GameObject>("prefabs/projectiles/BrotherSunderWave");
            sunder.GetComponent<ProjectileDamage>().damageType |= DamageType.NonLethal;

            var a = sunder.GetComponent<ProjectileCharacterController>();
            a.velocity = 60f;
            a.lifetime = 10f;
            sunder.AddComponent(typeof(RotateAroundAxis));
            var b = sunder.GetComponent<RotateAroundAxis>();
            b.enabled = true;
            b.speed = RotateAroundAxis.Speed.Fast;
            b.fastRotationSpeed = 25f;
            b.rotateAroundAxis = RotateAroundAxis.RotationAxis.Y;
            b.relativeTo = Space.World;
            b.reverse = false;

            var sundere = Resources.Load<GameObject>("prefabs/projectiles/BrotherSunderWave, Energized");
            sundere.GetComponent<ProjectileDamage>().damageType = DamageType.NonLethal;

            var pillar = Resources.Load<GameObject>("prefabs/projectiles/BrotherFirePillar");
            pillar.transform.localScale = new Vector3(4f, 1f, 4f);

            var pillarg = Resources.Load<GameObject>("prefabs/projectileghosts/BrotherFirePillarGhost");
            pillarg.transform.localScale = new Vector3(4f, 1f, 4f);

            pillar.AddComponent(typeof(RotateAroundAxis));

            var r = pillar.GetComponent<RotateAroundAxis>();
            r.enabled = true;
            r.speed = RotateAroundAxis.Speed.Fast;
            r.fastRotationSpeed = 20f;
            r.rotateAroundAxis = RotateAroundAxis.RotationAxis.Y;
            r.relativeTo = Space.World;
            r.reverse = false;

            var shardprojectile = Resources.Load<GameObject>("prefabs/projectiles/LunarShardProjectile");
            var shardp = shardprojectile.GetComponent<ProjectileSimple>();
            var shardt = shardprojectile.GetComponent<ProjectileSteerTowardTarget>();
            shardp.desiredForwardSpeed = 140f;
            shardp.lifetime = 10f;
            shardt.rotationSpeed = 35f;

            var slide = Resources.Load<SkillDef>("skilldefs/brotherbody/Slide");

            slide.baseRechargeInterval = 2.25f;

            var slam = Resources.Load<SkillDef>("skilldefs/brotherbody/weaponslam");

            slam.baseRechargeInterval = 3.25f;

            var shards = Resources.Load<SkillDef>("skilldefs/brotherbody/FireLunarShards");

            shards.baseMaxStock = 32;
            shards.rechargeStock = 32;
        }
        public static void FireWavesSlam(EntityStates.BrotherMonster.WeaponSlam self)
        {
            Ray aimRay = self.GetAimRay();
            if (self.isAuthority)
            {
                int wavecount = 360 / 180;
                Vector3 point = Vector3.ProjectOnPlane(self.inputBank.aimDirection, Vector3.up);
                Transform transform = self.FindModelChild(EntityStates.BrotherMonster.FistSlam.muzzleString);
                Vector3 position = transform.position;
                for (int i = 0; i < wavecount; i++)
                {
                    Vector3 forward = Quaternion.AngleAxis(wavecount * i, Vector3.up) * point;
                    ProjectileManager.instance.FireProjectile(EntityStates.BrotherMonster.ExitSkyLeap.waveProjectilePrefab, position, RoR2.Util.QuaternionSafeLookRotation(forward), self.gameObject, self.characterBody.damage * 2f, -200f, RoR2.Util.CheckRoll(self.characterBody.crit, self.characterBody.master), DamageColorIndex.Default, null, -1f);
                }
            }
        }
    }
}
