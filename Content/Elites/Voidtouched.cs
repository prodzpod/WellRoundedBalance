﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Gamemodes.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Voidtouched : EliteBase<Voidtouched>
    {
        public static BuffDef useless;
        public static BuffDef hiddenCooldown;
        public static GameObject spike; // cache this once so dont have to reload each time (i forgor if addressables already does this but just in case)

        public override string Name => ":: Elites :::::: Voidtouched";

        [ConfigField("Spike Count", "", 5)]
        public static int spikeCount;

        [ConfigField("Spike Count Eclipse 3+", "Only applies if you have Eclipse Changes enabled.", 8)]
        public static int spikeCountE3;

        [ConfigField("Spike Cooldown", "", 2f)]
        public static float spikeCooldown;

        [ConfigField("Spike Damage", "Decimal.", 2f)]
        public static float spikeDamage;

        [ConfigField("Permanent Damage Percent", "Eclipse 8 is 40", 40f)]
        public static float permanentDamagePercent;

        public override void Init()
        {
            useless = ScriptableObject.CreateInstance<BuffDef>();
            useless.name = "Voidtouched Deletion";
            useless.isHidden = true;

            hiddenCooldown = ScriptableObject.CreateInstance<BuffDef>();
            hiddenCooldown.name = "Voidtouched Spike Cooldown";
            hiddenCooldown.isHidden = true;

            ContentAddition.AddBuffDef(useless);
            ContentAddition.AddBuffDef(hiddenCooldown);

            spike = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.ImpVoidspikeProjectile.Load<GameObject>(), "Voidtouched Spike");

            var projectileController = spike.GetComponent<ProjectileController>();

            var spikeGhost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.ImpVoidspikeProjectileGhost.Load<GameObject>(), "Voidtouched Spike Ghost", false);
            spikeGhost.transform.localScale = new Vector3(3f, 3f, 3f);

            projectileController.ghostPrefab = spikeGhost;

            var projectileImpactExplosion = spike.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.blastRadius = 6f;

            spike.transform.localScale = new Vector3(3f, 3f, 3f);

            var proximityTrigger = spike.transform.GetChild(0).GetChild(5);
            var sphereCollider = proximityTrigger.GetComponent<SphereCollider>();
            sphereCollider.radius = 6f;

            var projectileStickOnImpact = spike.GetComponent<ProjectileStickOnImpact>();
            // remove projectileimpactexplosion setexplosionradius event
            // add projectileimpactexplosion setexplosionradius event to make spike have a 7.5m explosion radius

            PrefabAPI.RegisterNetworkPrefab(spike);

            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            IL.RoR2.AffixVoidBehavior.FixedUpdate += AffixVoidBehavior_FixedUpdate;
            On.RoR2.CharacterBody.OnSkillActivated += OnSkillActivated;
            On.RoR2.HealthComponent.Awake += HealthComponent_Awake;
        }

        private void HealthComponent_Awake(On.RoR2.HealthComponent.orig_Awake orig, HealthComponent self)
        {
            self.gameObject.AddComponent<VoidtouchedPermanentDamage>();
            orig(self);
        }

        private void OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody body, GenericSkill slot)
        {
            orig(body, slot);
            if (!NetworkServer.active || body.HasBuff(hiddenCooldown) || !body.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                return;
            }

            Vector3 originalPosition = body.corePosition;
            Vector3 aimDirection = body.inputBank.aimDirection;

            for (int i = 0; i < (Eclipse3.CheckEclipse() ? spikeCountE3 : spikeCount); i++)
            {
                Vector3 position = originalPosition + (aimDirection * (i * 10));
                position.y = 20;
                if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 1000, ~0))
                {
                    if (Util.HasEffectiveAuthority(body.gameObject))
                    {
                        FireProjectileInfo info = new()
                        {
                            damage = body.damage * spikeDamage,
                            damageTypeOverride = DamageType.Nullify,
                            crit = false,
                            position = position,
                            rotation = Quaternion.LookRotation(Vector3.down),
                            projectilePrefab = spike,
                            owner = body.gameObject,
                            speedOverride = 50
                        };
                        ProjectileManager.instance.FireProjectile(info);
                    }
                }
            }
            body.AddTimedBuff(hiddenCooldown, spikeCooldown);
        }

        private void AffixVoidBehavior_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld("RoR2.DLC1Content/Buffs", "BearVoidReady"),
                x => x.MatchCallOrCallvirt<CharacterBody>("AddBuff")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.voidtouchedSaferSpaces));
            }
            else
            {
                Logger.LogError("Failed to apply Voidtouched Elite Safer Spaces hook");
            }
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(DLC1Content.Buffs), "EliteVoid")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessBuff));
            }
            else
            {
                Logger.LogError("Failed to apply Voidtouched Elite Needletick hook");
            }
        }
    }

    public class VoidtouchedPermanentDamage : MonoBehaviour, IOnTakeDamageServerReceiver
    {
        public HealthComponent hc;
        public CharacterBody body;

        public void Start()
        {
            hc = GetComponent<HealthComponent>();
            if (!hc)
            {
                Destroy(this);
                return;
            }
            body = hc.body;
        }

        public void OnTakeDamageServer(DamageReport damageReport)
        {
            if (body && damageReport.attacker && damageReport.attackerBody && damageReport.attackerBody.HasBuff(DLC1Content.Buffs.EliteVoid))
            {
                switch (body.teamComponent.teamIndex)
                {
                    case TeamIndex.Player:
                        {
                            float takenDamagePercent = damageReport.damageDealt / hc.fullCombinedHealth * 100f;
                            int permanentDamage = Mathf.FloorToInt((takenDamagePercent * Voidtouched.permanentDamagePercent / 100f) * damageReport.damageInfo.procCoefficient);
                            for (int l = 0; l < permanentDamage; l++)
                            {
                                body.AddBuff(RoR2Content.Buffs.PermanentCurse);
                            }
                        }
                        break;
                }
            }
        }
    }
}