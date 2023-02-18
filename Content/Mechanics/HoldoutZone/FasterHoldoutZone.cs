﻿namespace WellRoundedBalance.Mechanics.HoldoutZone
{
    internal class FasterHoldoutZone : MechanicBase
    {
        public override string Name => ":: Mechanics :::::: Faster Holdout Zone";

        public override void Hooks()
        {
            On.RoR2.HoldoutZoneController.Awake += HoldoutZoneController_Awake;
        }

        private void HoldoutZoneController_Awake(On.RoR2.HoldoutZoneController.orig_Awake orig, HoldoutZoneController self)
        {
            orig(self);
            self.calcChargeRate += Self_calcChargeRate;
        }

        private void Self_calcChargeRate(ref float rate)
        {
            rate *= 1.6f;
        }
    }
}