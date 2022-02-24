﻿using RoR2;
using RoR2.CharacterAI;
using System.Linq;
using UnityEngine;

namespace UltimateCustomRun.Enemies
{
    public class Beetle : EnemyBase
    {
        public static float Duration;
        public static float SpawnSpeed;
        public static bool CanBeHitStunned;
        public static bool Tweaks;
        public override string Name => ":::: Enemies :: Beetle";

        public override void Init()
        {
            Duration = ConfigOption(1.5f, "Headbutt Duration", "Vanilla is 1.5.\nRecommended Value with Dash enabled: 3");
            SpawnSpeed = ConfigOption(5f, "Spawn Duration", "Vanilla is 5.\nRecommended Value: 2.5");
            CanBeHitStunned = ConfigOption(true, "Can be Hit Stunned?", "Vanilla is true.\nRecommended Value: false");
            Tweaks = ConfigOption(false, "Enable Dash and AI Tweaks?", "Vanilla is false.\nRecommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Buff();
        }

        public static void Buff()
        {
            var m = Resources.Load<CharacterMaster>("prefabs/charactermasters/BeetleMaster");
            var b = Resources.Load<CharacterBody>("prefabs/characterbodies/BeetleBody");
            if (Tweaks)
            {
                On.EntityStates.BeetleMonster.HeadbuttState.FixedUpdate += (orig, self) =>
                {
                    EntityStates.BeetleMonster.HeadbuttState.baseDuration = Duration;
                    if (self.modelAnimator && self.modelAnimator.GetFloat("Headbutt.hitBoxActive") > 0.5f)
                    {
                        Vector3 direction = self.GetAimRay().direction;
                        Vector3 a = direction.normalized * 2f * self.moveSpeedStat;
                        Vector3 b = Vector3.up * 5f;
                        Vector3 b2 = new Vector3(direction.x, 0f, direction.z).normalized * 4.5f;
                        self.characterMotor.Motor.ForceUnground();
                        self.characterMotor.velocity = a + b + b2;
                    }
                    if (self.fixedAge > 0.5f)
                    {
                        self.attack.Fire(null);
                    }
                    orig(self);
                };

                m.GetComponent<BaseAI>().fullVision = true;

                AISkillDriver ai = (from x in m.GetComponents<AISkillDriver>()
                                    where x.customName == "HeadbuttOffNodegraph"
                                    select x).First();
                ai.maxDistance = 25f;
                ai.selectionRequiresOnGround = true;
                ai.activationRequiresAimTargetLoS = true;

                b.baseMoveSpeed = 12f;
            }

            b.GetComponent<SetStateOnHurt>().canBeHitStunned = CanBeHitStunned;

            On.EntityStates.GenericCharacterSpawnState.OnEnter += (orig, self) =>
            {
                if (self is EntityStates.BeetleMonster.SpawnState)
                {
                    self.duration = SpawnSpeed;
                }
                orig(self);
            };
        }
    }
}