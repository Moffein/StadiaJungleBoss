using BepInEx;
using RoR2;
using R2API;
using UnityEngine;
using R2API.Utils;
using StadiaJungleBoss.Setup;
using UnityEngine.AddressableAssets;
using StadiaJungleBoss.Components;
using System;

namespace StadiaJungleBoss
{
    [BepInDependency("com.bepis.r2api")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin("com.Moffein.StadiaJungleBoss", "Stadia Jungle Boss", "1.0.0")]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "ContentAddition"
    })]
    public class StadiaJungleBossPlugin : BaseUnityPlugin
    {
        public static PluginInfo pluginInfo;
        private void Awake()
        {
            pluginInfo = this.Info;
            Assets.PopulateAssets();
            Tokens.LoadLanguage();
            On.RoR2.SceneDirector.Start += StageSetup.SceneDirectorHook;
            RoR2Application.onLoad += CreateSpawnCard;
        }

        private void CreateSpawnCard()
        {
            CharacterSpawnCard spawnCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            spawnCard.directorCreditCost = 800;
            spawnCard.eliteRules = SpawnCard.EliteRules.ArtifactOnly;
            spawnCard.noElites = false;
            spawnCard.prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaWormMaster.prefab").WaitForCompletion();
            spawnCard.sendOverNetwork = true;
            spawnCard.requiredFlags = RoR2.Navigation.NodeFlags.None;
            spawnCard.forbiddenFlags = RoR2.Navigation.NodeFlags.TeleporterOK;
            spawnCard.occupyPosition = false;
            spawnCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            spawnCard.hullSize = HullClassification.Golem;

            spawnCard.itemsToGrant = new ItemCountPair[]
            {
                new ItemCountPair()
                {
                    itemDef = RoR2Content.Items.TeleportWhenOob,
                    count = 1
                }
            };
            spawnCard.equipmentToGrant = new EquipmentDef[]
            {
                Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/EliteEarth/EliteEarthEquipment.asset").WaitForCompletion()
            };

            spawnCard.forbiddenAsBoss = true;

            EncounterController.bossCard = spawnCard;
        }
    }
}
