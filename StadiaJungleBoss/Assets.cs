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
        }
    }
}
