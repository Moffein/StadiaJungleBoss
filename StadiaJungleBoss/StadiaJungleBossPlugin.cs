using BepInEx;
using RoR2;
using R2API;
using UnityEngine;
using R2API.Utils;
using StadiaJungleBoss.Setup;
using UnityEngine.AddressableAssets;
using StadiaJungleBoss.Components;
using System;
using BepInEx.Configuration;

namespace StadiaJungleBoss
{
    [BepInDependency("com.bepis.r2api")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin("com.Moffein.StadiaJungleBoss", "Stadia Jungle Boss", "1.1.5")]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "ContentAddition"
    })]
    public class StadiaJungleBossPlugin : BaseUnityPlugin
    {
        public static PluginInfo pluginInfo;

        internal static EquipmentDef EliteEarthEquipment = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/EliteEarth/EliteEarthEquipment.asset").WaitForCompletion();

        private void Awake()
        {
            pluginInfo = this.Info;
            Assets.PopulateAssets();
            Tokens.LoadLanguage();

            ReadConfig();

            On.RoR2.SceneDirector.Start += StageSetup.SceneDirectorHook;
            RoR2Application.onLoad += CreateSpawnCard;

            MagmaWormChanges.Initialize();
        }

        private void ReadConfig()
        {
            MagmaWormChanges.enabled = base.Config.Bind<bool>(new ConfigDefinition("Magma Worm", "Enable Changes"), true,
                new ConfigDescription("Modify the Mending Magma Worm spawned by this mod.")).Value;

            MagmaWormChanges.useStatItem = base.Config.Bind<bool>(new ConfigDefinition("Magma Worm", "Use Hidden Item"), true,
                new ConfigDescription("Use a hidden item to determine whether to apply Magma Worm changes. If not, changes will be applied using a hackier method. You should REALLY leave this enabled unless you are using a mod with a config based on raw item indexes.")).Value;
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

            if (MagmaWormChanges.enabled && MagmaWormChanges.useStatItem)
            {

                spawnCard.itemsToGrant = new ItemCountPair[]
                {
                    new ItemCountPair()
                    {
                        itemDef = RoR2Content.Items.TeleportWhenOob,
                        count = 1
                    },
                    new ItemCountPair()
                    {
                        itemDef = MagmaWormChanges.StatItem,
                        count = 1
                    },
                };
            }
            else
            {
                spawnCard.itemsToGrant = new ItemCountPair[]
                {
                    new ItemCountPair()
                    {
                        itemDef = RoR2Content.Items.TeleportWhenOob,
                        count = 1
                    }
                };
            }

            spawnCard.equipmentToGrant = new EquipmentDef[]
            {
                EliteEarthEquipment
            };

            spawnCard.forbiddenAsBoss = true;

            EncounterController.bossCard = spawnCard;
        }
    }
}
