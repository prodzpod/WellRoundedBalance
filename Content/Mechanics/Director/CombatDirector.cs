﻿namespace WellRoundedBalance.Mechanic.Director
{
    internal class CombatDirector : MechanicBase
    {
        public override string Name => ":: Mechanics ::::: Combat Director";

        public override void Hooks()
        {
            On.RoR2.CombatDirector.OnEnable += CombatDirector_OnEnable;
            On.RoR2.CombatDirector.OnDisable += CombatDirector_OnDisable;
            On.RoR2.CombatDirector.Spawn += CombatDirector_Spawn;
        }

        private bool CombatDirector_Spawn(On.RoR2.CombatDirector.orig_Spawn orig, RoR2.CombatDirector self, SpawnCard spawnCard, EliteDef eliteDef, Transform spawnTarget, DirectorCore.MonsterSpawnDistance spawnDistance, bool preventOverhead, float valueMultiplier, DirectorPlacementRule.PlacementMode placementMode)
        {
            self.monsterCredit += 0.3f * Mathf.Sqrt(Run.instance.difficultyCoefficient + spawnCard.directorCreditCost);
            return orig(self, spawnCard, eliteDef, spawnTarget, spawnDistance, preventOverhead, valueMultiplier, placementMode);
        }

        private void CombatDirector_OnDisable(On.RoR2.CombatDirector.orig_OnDisable orig, RoR2.CombatDirector self)
        {
            self.minRerollSpawnInterval *= 1.35f;
            self.maxRerollSpawnInterval *= 1.35f;
            orig(self);
        }

        private void CombatDirector_OnEnable(On.RoR2.CombatDirector.orig_OnEnable orig, RoR2.CombatDirector self)
        {
            self.minRerollSpawnInterval /= 1.35f;
            self.maxRerollSpawnInterval /= 1.35f;
            self.creditMultiplier += 0.25f;
            self.eliteBias *= 0.9f;
            orig(self);
        }
    }
}