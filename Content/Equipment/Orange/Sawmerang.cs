﻿using WellRoundedBalance.Items;
using WellRoundedBalance.Misc;

namespace WellRoundedBalance.Equipment.Orange
{
    public class Sawmerang : EquipmentBase
    {
        public override string Name => ":: Equipment :: Sawmerang";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.Saw;

        public override string PickupText => "Throw a fan of buzzing saws that come back to you.";

        public override string DescText => "Throw <style=cIsDamage>three large saw blades</style> that slice through enemies for <style=cIsDamage>3x" + d(frontSawDamage) + "</style> damage. Also deals an additional <style=cIsDamage>3x" + d(returningSawDamage) + " damage per second</style> while <style=cIsDamage>bleeding</style> enemies. Can <style=cIsDamage>strike</style> enemies again on the way back.";

        [ConfigField("Cooldown", "", 45f)]
        public static float cooldown;

        [ConfigField("Front Saw Damage", "Decimal.", 4f)]
        public static float frontSawDamage;

        [ConfigField("Front Saw Proc Coefficient", "", 1f)]
        public static float frontSawProcCoefficient;

        [ConfigField("Returning Saw Damage", "Decimal.", 1.4f)]
        public static float returningSawDamage;

        [ConfigField("Returning Saw Proc Coefficient", "", 0.2f)]
        public static float returningSawProcCoefficient;

        [ConfigField("Distance Multiplier", "", 0.6f)]
        public static float distanceMultiplier;

        [ConfigField("Speed", "", 60f)]
        public static float speed;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();

            var Saw = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Saw/Saw.asset").WaitForCompletion();
            Saw.cooldown = cooldown;
        }

        private void Changes()
        {
            var saw = Utils.Paths.GameObject.Sawmerang.Load<GameObject>();
            var destroyStuckObject = saw.AddComponent<DestroyStuckObject>();
            destroyStuckObject.initialDelay = 1.3f;
            var projectileOverlapAttack = saw.GetComponent<ProjectileOverlapAttack>();
            var boomerangProjectile = saw.GetComponent<BoomerangProjectile>();
            var projectileDotZone = saw.GetComponent<ProjectileDotZone>();

            boomerangProjectile.travelSpeed = speed;
            boomerangProjectile.distanceMultiplier = distanceMultiplier;

            projectileOverlapAttack.damageCoefficient = frontSawDamage;
            projectileOverlapAttack.overlapProcCoefficient = frontSawProcCoefficient;

            projectileDotZone.damageCoefficient = returningSawDamage * (1f / 7f);
            projectileDotZone.overlapProcCoefficient = returningSawProcCoefficient;
        }
    }
}