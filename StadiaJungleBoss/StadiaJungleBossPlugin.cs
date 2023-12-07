using BepInEx;
using RoR2;
using R2API;
using UnityEngine;
using R2API.Utils;
using StadiaJungleBoss.Setup;

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
        public void Awake()
        {
            Assets.PopulateAssets();
            On.RoR2.SceneDirector.Start += StageSetup.SceneDirectorHook;
        }
    }
}
