﻿namespace WellRoundedBalance.Projectiles
{
    public static class EarthQuakeWave
    {
        public static GameObject prefab;

        public static void Init()
        {
            prefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.BrotherSunderWave.Load<GameObject>(), "EarthquakeWave", false);
            var projectileDamage = prefab.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Generic;

            // var characterController = prefab.GetComponent<CharacterController>();
            // characterController.slopeLimit = 70f;

            var projectileCharacterController = prefab.GetComponent<ProjectileCharacterController>();
            projectileCharacterController.velocity = 35f;
            // projectileCharacterController.lifetime = 2f;

            var projectileOverlapAttack = prefab.GetComponent<ProjectileOverlapAttack>();
            projectileOverlapAttack.forceVector = new Vector3(0f, 0f, 0f);

            var newGhost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.BrotherSunderWaveGhost.Load<GameObject>(), "EarthquakeWaveGhost", false);
            var @base = newGhost.transform.GetChild(0);
            var infection = @base.GetChild(0).GetComponent<ParticleSystemRenderer>();
            infection.gameObject.transform.localPosition = new Vector3(0.5f, -0.456f, 0.5f);
            infection.material.SetTexture("_EmTex", Main.wellroundedbalance.LoadAsset<Texture2D>("Assets/WellRoundedBalance/texRampWave.png"));
            infection.material.SetTexture("_MainTex", Main.wellroundedbalance.LoadAsset<Texture2D>("Assets/WellRoundedBalance/texRampWave.png"));

            var particleSystem = infection.gameObject.GetComponent<ParticleSystem>();

            var main = particleSystem.main;
            main.maxParticles = 500;
            var emission = particleSystem.emission;
            emission.rateOverTime = 80;

            var sizeOverLifetime = particleSystem.sizeOverLifetime;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, new AnimationCurve(new Keyframe(0f, 0.66f), new Keyframe(0.5f, 0.9f), new Keyframe(1f, 0.66f)));

            var water = @base.GetChild(3);
            water.gameObject.SetActive(false);

            var hitboxReal = prefab.transform.GetChild(0);
            hitboxReal.transform.localScale = new Vector3(30f, 1.33f, 1.1f);

            var hitboxFake = newGhost.transform.GetChild(1);
            hitboxFake.localScale = new Vector3(30f, 1.33f, 1.1f);

            var mr = hitboxFake.GetComponent<MeshRenderer>();
            mr.material.SetTexture("_RemapTex", Main.wellroundedbalance.LoadAsset<Texture2D>("Assets/WellRoundedBalance/texRampInspire.png"));

            var projectileController = prefab.GetComponent<ProjectileController>();
            projectileController.flightSoundLoop = null;
            projectileController.ghostPrefab = newGhost;

            PrefabAPI.RegisterNetworkPrefab(prefab);
        }
    }
}