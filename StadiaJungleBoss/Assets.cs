using R2API;
using RoR2;
using StadiaJungleBoss.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace StadiaJungleBoss
{
    public static class Assets
    {
        public static AssetBundle mainAssetBundle;

        public static class Prefabs
        {
            //from direseeker
            public static GameObject Button;
            public static GameObject Encounter;
        }

        public static void PopulateAssets()
        {
            if (Assets.mainAssetBundle) return;

            using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("StadiaJungleBoss.stadiabossbundle"))
            {
                Assets.mainAssetBundle = AssetBundle.LoadFromStream(manifestResourceStream);
            }

            Assets.Prefabs.Button = Assets.mainAssetBundle.LoadAsset<GameObject>("BossButton");
            Shader shader = LegacyResourcesAPI.Load<Shader>("Shaders/Deferred/hgstandard");
            Material material = Assets.Prefabs.Button.GetComponentInChildren<SkinnedMeshRenderer>().material;
            material.shader = shader;
            Assets.Prefabs.Button.AddComponent<BossButtonController>();
            Assets.Prefabs.Button.AddComponent<NetworkIdentity>();
            Assets.Prefabs.Button.RegisterNetworkPrefab();	//Apparently this auto adds it to the contentpack?

            GameObject bossEncounter = Assets.mainAssetBundle.LoadAsset<GameObject>("StadiaJungleEncounter");
            bossEncounter.AddComponent<NetworkIdentity>();
            TeamFilter tf = bossEncounter.AddComponent<TeamFilter>();
            tf.defaultTeam = TeamIndex.Monster;

            BossGroup bg = bossEncounter.AddComponent<BossGroup>();
            bg.dropTable = Addressables.LoadAssetAsync<PickupDropTable>("RoR2/Base/Common/dtTier3Item.asset").WaitForCompletion();
            bg.bossDropChance = 0f;
            bg.scaleRewardsByPlayerCount = true;
            bg.shouldDisplayHealthBarOnHud = true;

            ScriptedCombatEncounter sc = bossEncounter.AddComponent<ScriptedCombatEncounter>();
            sc.grantUniqueBonusScaling = true;
            sc.spawnOnStart = false;

            ScriptedCombatEncounter.SpawnInfo si = new ScriptedCombatEncounter.SpawnInfo
            {
                cullChance = 0f,
                spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/RoboBallBoss/cscSuperRoboBallBoss.asset").WaitForCompletion()
            };

            sc.spawns = new ScriptedCombatEncounter.SpawnInfo[] { si };

            EncounterController ec = bossEncounter.AddComponent<EncounterController>();

            bossEncounter.RegisterNetworkPrefab();	//Apparently this auto adds it to the contentpack?

            Assets.Prefabs.Encounter = bossEncounter;
        }
    }
}
