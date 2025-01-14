﻿using System;

namespace WellRoundedBalance.Interactables
{
    internal class Chests : InteractableBase<Chests>
    {
        public override string Name => ":: Interactables :: Chests";

        [ConfigField("Equipment Trishop Max Spawns Per Stage", "", 2)]
        public static int equipmentTrishopMaxSpawnsPerStage;

        [ConfigField("Legendary Chest Cost", "", 250)]
        public static int legendaryChestCost;

        [ConfigField("Small Category Chest Cost", "", 25)]
        public static int smallCategoryChestCost;

        [ConfigField("Large Category Chest Cost", "", 50)]
        public static int largeCategoryChestCost;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var legendaryChest = Utils.Paths.GameObject.GoldChest.Load<GameObject>();
            var legendaryChesturchaseInteraction = legendaryChest.GetComponent<PurchaseInteraction>();
            legendaryChesturchaseInteraction.cost = legendaryChestCost;
            On.RoR2.Run.GetDifficultyScaledCost_int_float += Run_GetDifficultyScaledCost_int_float;

            var stealthedChest = Utils.Paths.InteractableSpawnCard.iscChest1Stealthed.Load<InteractableSpawnCard>();
            stealthedChest.maxSpawnsPerStage = 2;
            stealthedChest.directorCreditCost = 1;

            var smallDamage = Utils.Paths.GameObject.CategoryChestDamage.Load<GameObject>().GetComponent<PurchaseInteraction>();
            smallDamage.cost = smallCategoryChestCost;

            var smallHealing = Utils.Paths.GameObject.CategoryChestHealing.Load<GameObject>().GetComponent<PurchaseInteraction>();
            smallHealing.cost = smallCategoryChestCost;

            var smallUtility = Utils.Paths.GameObject.CategoryChestUtility.Load<GameObject>().GetComponent<PurchaseInteraction>();
            smallUtility.cost = smallCategoryChestCost;

            var largeDamage = Utils.Paths.GameObject.CategoryChest2DamageVariant.Load<GameObject>().GetComponent<PurchaseInteraction>();
            largeDamage.cost = largeCategoryChestCost;

            var largeHealing = Utils.Paths.GameObject.CategoryChest2HealingVariant.Load<GameObject>().GetComponent<PurchaseInteraction>();
            largeHealing.cost = largeCategoryChestCost;

            var largeUtility = Utils.Paths.GameObject.CategoryChest2UtilityVariant.Load<GameObject>().GetComponent<PurchaseInteraction>();
            largeUtility.cost = largeCategoryChestCost;

            var equipTrishop = Utils.Paths.InteractableSpawnCard.iscTripleShopEquipment.Load<InteractableSpawnCard>();
            equipTrishop.maxSpawnsPerStage = equipmentTrishopMaxSpawnsPerStage;
        }

        private int Run_GetDifficultyScaledCost_int_float(On.RoR2.Run.orig_GetDifficultyScaledCost_int_float orig, Run self, int baseCost, float difficultyCoefficient)
        {
            if (baseCost == 400)
            {
                baseCost = legendaryChestCost;
            }
            return orig(self, baseCost, difficultyCoefficient);
        }
    }
}