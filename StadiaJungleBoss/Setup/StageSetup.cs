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
            Debug.Log("Number of stages found in config: " + StadiaJungleBossPlugin.LevelNames.Length);
            foreach (string level in StadiaJungleBossPlugin.LevelNames)
            {
                Debug.Log("Level = " + level);
            }
            bool attemptPatch = false;
            if (StadiaJungleBossPlugin.LevelNames.Contains(SceneManager.GetActiveScene().name))
            {
                Debug.Log("Current stage " + SceneManager.GetActiveScene().name + " found in config");
                attemptPatch = true;
            }
            else
            {
                Debug.Log("Current stage " + SceneManager.GetActiveScene().name + " not found in config, so the no changes will apply");
                attemptPatch = false;
            }
            if (attemptPatch)
            {
                BossButtonController.buttonsPressed = 0;
                BossButtonController.totalButtons = 0;
                GameObject randomHolder = GameObject.Find("HOLDER: Randomization");
                if (randomHolder)
                {
                    Transform largeChestTransform = randomHolder.transform.Find("GROUP: Large Treasure Chests");
                    if (largeChestTransform)
                    {
                        PlaceButtons(largeChestTransform);
                        GameObject encounter = UnityEngine.Object.Instantiate<GameObject>(Assets.Prefabs.Encounter);
                        NetworkServer.Spawn(encounter);
                    }
                }
                else
                {
                    return; //WIP code, planned support for Sunset Tropics and other stage 4s
                    //Gonna need to locate the large chests without the Randomization Holder, so check for sunset tropic's own chest placement method, or use another
                    GameObject[] chestHolders = {
                    GameObject.Find("Gold Chests/Under Planks"),
                    GameObject.Find("Gold Chests/Beach Nautilus"),
                    GameObject.Find("Gold Chests/Ruins Staircase"),
                    GameObject.Find("Gold Chests/Cliff Hole"),
                    GameObject.Find("Gold Chests/Trench"),
                    };
                    if (chestHolders.Length > 0)
                    {
                        for (int i = 0; i < chestHolders.Length; i++)
                        {
                            Transform currentChild = chestHolders[i].transform;
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
                                if (StadiaJungleBossPlugin.RemoveChestsConfig)
                                {
                                    Debug.Log("StadiaJungleBoss: Destroying potential gold chest.");
                                    UnityEngine.Object.Destroy(chest.gameObject);
                                }
                            }
                            else
                            {
                                Debug.Log("StadiaJungleBoss: No chest found in category " + i);
                            }
                        }
                    }
                    else
                    { // Not sunset tropics, so just place buttons at random spots?

                    }
                }
            }
            orig(self);
        }
        internal static void PlaceButtons(Transform LargeChestTransform)
        {
            for (int i = 0; i < LargeChestTransform.childCount; i++)
            {
                Transform currentChild = LargeChestTransform.GetChild(i);
                if (!currentChild) continue;
                bool originalChest = false;
                if (currentChild.gameObject.activeSelf)
                {
                    originalChest = true;
                }
                if (originalChest && !StadiaJungleBossPlugin.RemoveChestsConfig)
                { //If its the original chest and we are not deleting Legendary Chests, don't bother placing a button
                    Debug.Log("StadiaJungleBoss: Skipping button placement because this is where the Legendary Chest goes");
                }
                else
                {
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
                        if (StadiaJungleBossPlugin.RemoveChestsConfig)
                        {
                            Debug.Log("StadiaJungleBoss: Destroying potential gold chest.");
                            UnityEngine.Object.Destroy(chest.gameObject);
                        }
                        else if (!originalChest)
                        {
                            currentChild.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        Debug.Log("StadiaJungleBoss: No chest found in category " + i);
                    }
                }
            }
        }
    }
}
