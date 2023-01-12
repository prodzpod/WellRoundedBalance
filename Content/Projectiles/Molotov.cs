﻿namespace WellRoundedBalance.Projectiles
{
    public static class Molotov
    {
        public static GameObject prefab;

        public static void Create()
        {
            var ghostPrefab = Utils.Paths.GameObject.FireballGhost.Load<GameObject>();
            prefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MolotovClusterProjectile.Load<GameObject>(), "BlazingEliteClusterProjectile");
            var projectileController = prefab.GetComponent<ProjectileController>();
            var projectileImpactExplosion = prefab.GetComponent<ProjectileImpactExplosion>();

            projectileController.ghostPrefab = ghostPrefab;
            projectileController.startSound = "Play_fireballsOnHit_shoot";
            //prefab = Utils.Paths.GameObject.MolotovProjectileDotZone.Load<GameObject>();

            projectileImpactExplosion.blastDamageCoefficient = 0f;
            projectileImpactExplosion.blastProcCoefficient = 0f;
            projectileImpactExplosion.dotIndex = DotController.DotIndex.None;
            projectileImpactExplosion.childrenCount = 3;
            projectileImpactExplosion.destroyOnEnemy = false;

            var molotovChild = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MolotovSingleProjectile.Load<GameObject>(), "BlazingEliteSingleProjectile");
            var projectileControllerChild = molotovChild.GetComponent<ProjectileController>();

            projectileControllerChild.ghostPrefab = ghostPrefab;
            projectileControllerChild.startSound = "Play_fireballsOnHit_shoot";

            var projectileImpactExplosionChild = molotovChild.GetComponent<ProjectileImpactExplosion>();

            projectileImpactExplosionChild.blastDamageCoefficient = 0f;
            projectileImpactExplosionChild.blastProcCoefficient = 0f;
            projectileImpactExplosionChild.dotIndex = DotController.DotIndex.None;
            projectileImpactExplosionChild.destroyOnEnemy = false;

            var firePool = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MolotovProjectileDotZone.Load<GameObject>(), "BlazingElitePoolProjectile");
            var projectileDotZonePool = firePool.GetComponent<ProjectileDotZone>();
            projectileDotZonePool.damageCoefficient = 0.12f;
            projectileDotZonePool.overlapProcCoefficient = 0f;
            projectileDotZonePool.lifetime = 8f;
            projectileDotZonePool.resetFrequency = 12f;

            projectileImpactExplosionChild.childrenProjectilePrefab = firePool;

            projectileImpactExplosion.childrenProjectilePrefab = molotovChild;

            PrefabAPI.RegisterNetworkPrefab(firePool);
            PrefabAPI.RegisterNetworkPrefab(molotovChild);
            PrefabAPI.RegisterNetworkPrefab(prefab);
        }
    }
}