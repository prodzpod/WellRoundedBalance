﻿using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateCustomRun.Stages
{
    public static class SunderedGrove
    {
        public static void AddGrovetender()
        {
            var tendies = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscGravekeeper"),
                selectionWeight = 2,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = "",
                forbiddenUnlockable = "",
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };

            var groovesalad = new DirectorAPI.DirectorCardHolder
            {
                Card = tendies,
                MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                InteractableCategory = DirectorAPI.InteractableCategory.None
            };

            DirectorAPI.MonsterActions += (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage) =>
            {
                if (stage.stage == DirectorAPI.Stage.SunderedGrove)
                {
                    if (!list.Contains(groovesalad))
                    {
                        list.Add(groovesalad);
                    }
                }
            };
        }

        public static void RemoveClayDunestrider()
        {
            var glue = new DirectorCard
            {
                spawnCard = LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscClayBoss"),
                selectionWeight = 1,
                preventOverhead = false,
                minimumStageCompletions = 0,
                requiredUnlockable = "",
                forbiddenUnlockable = "",
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
            };

            var zamn = new DirectorAPI.DirectorCardHolder
            {
                Card = glue,
                MonsterCategory = DirectorAPI.MonsterCategory.Champions,
                InteractableCategory = DirectorAPI.InteractableCategory.None
            };

            DirectorAPI.MonsterActions += (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage) =>
            {
                if (stage.stage == DirectorAPI.Stage.SunderedGrove)
                {
                    if (list.Contains(zamn))
                    {
                        list.Remove(zamn);
                    }
                }
            };
        }
    }
}