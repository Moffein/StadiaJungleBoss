using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine;
using StadiaJungleBoss.Components;

namespace StadiaJungleBoss.Setup
{
    internal static class StageSetup
    {
        internal static void SceneDirectorHook(On.RoR2.SceneDirector.orig_Start orig, RoR2.SceneDirector self)
        {
            if (SceneManager.GetActiveScene().name == "rootjungle")
            {
                BossButtonController.buttonsPressed = 0;
                BossButtonController.totalButtons = 0;

                GameObject randomHolder = GameObject.Find("HOLDER: Randomization");
                if (!randomHolder)
                {
                    Debug.LogError("StadiaJungleBoss: Could not find Randomization");
                    return;
                }

                Transform largeChestTransform = randomHolder.transform.Find("GROUP: Large Treasure Chests");
                if (!largeChestTransform)
                {
                    Debug.LogError("StadiaJungleBoss: Could not find Large Treasure Chests");
                    return;
                }

                for (int i = 0; i < largeChestTransform.childCount; i++)
                {
                    Transform currentChild = largeChestTransform.GetChild(i);
                    if (!currentChild) continue;

                    currentChild.gameObject.SetActive(true);
                    Transform chest = currentChild.Find("GoldChest");
                    if (chest)
                    {
                        if (NetworkServer.active)
                        {
                            Debug.Log("StadiaJungleBoss: Placing button.");
                            GameObject button = UnityEngine.Object.Instantiate<GameObject>(Assets.Prefabs.Button);
                            button.transform.position = chest.position - 1.2f * Vector3.up;
                            button.transform.rotation = chest.rotation;
                            NetworkServer.Spawn(button);
                        }

                        Debug.Log("StadiaJungleBoss: Destroying potential gold chest.");
                        UnityEngine.Object.Destroy(chest.gameObject);
                    }
                    else
                    {
                        Debug.Log("StadiaJungleBoss: No chest found in category " + i);
                    }
                }


                GameObject encounter = UnityEngine.Object.Instantiate<GameObject>(Assets.Prefabs.Encounter);
                NetworkServer.Spawn(encounter);
            }

            orig(self);
        }
    }
}
