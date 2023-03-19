﻿namespace WellRoundedBalance.Enemies
{
    internal class Geep : EnemyBase
    {
        public override string Name => ":: Enemies :: Geep";

        [ConfigField("Base Max Health", "Disabled if playing Inferno.", 250f)]
        public static float baseMaxHealth;

        public override void Hooks()
        {
            var geep = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Gup/GeepBody.prefab").WaitForCompletion();

            var geepBody = geep.GetComponent<CharacterBody>();
            geepBody.baseMaxHealth = baseMaxHealth;
            geepBody.levelMaxHealth = baseMaxHealth * 0.3f;
        }
    }
}