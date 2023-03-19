﻿namespace WellRoundedBalance.Enemies
{
    internal class SolusProbe : EnemyBase
    {
        public override string Name => ":: Enemies ::::::: Solus Probe";

        [ConfigField("Base Damage", "Disabled if playing Inferno.", 12f)]
        public static float baseDamage;

        public override void Hooks()
        {
            var solus = Utils.Paths.GameObject.RoboBallMiniBody14.Load<GameObject>();
            var solusBody = solus.GetComponent<CharacterBody>();
            solusBody.baseDamage = baseDamage;
            solusBody.levelDamage = baseDamage * 0.2f;
        }
    }
}