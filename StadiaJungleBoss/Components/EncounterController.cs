using UnityEngine.Networking;
using UnityEngine;
using RoR2;
using static StadiaJungleBoss.Components.BossButtonController;
using System;

namespace StadiaJungleBoss.Components
{
    public class EncounterController : MonoBehaviour
    {
        public float spawnDelay = 3f;

        public static CharacterSpawnCard bossCard;

        private ScriptedCombatEncounter scriptedCombatEncounter;

        private bool showedWarning = false;
        private bool spawnedBoss = false;
        private float spawnDelayCountdown = 0f;


        private void Awake()
        {
            scriptedCombatEncounter = base.GetComponent<ScriptedCombatEncounter>();

            /*GameObject spawnPosition = new GameObject();
            spawnPosition.layer = LayerIndex.noCollision.intVal;
            spawnPosition.transform.position = new Vector3(-156.2064f, 74.37479f, -173.8296f) + Vector3.up * 20f;
            UnityEngine.Object.Instantiate(spawnPosition);*/

            for (int i = 0; i < scriptedCombatEncounter.spawns.Length; i++)
            {
                //scriptedCombatEncounter.spawns[i].explicitSpawnPosition = spawnPosition.transform;
                scriptedCombatEncounter.spawns[i].spawnCard = bossCard;
            }

            BossButtonController.OnButtonPressedActions += CheckButtons;

            BossGroup bg = base.GetComponent<BossGroup>();
            GameObject dropPosition = new GameObject();
            dropPosition.layer = LayerIndex.noCollision.intVal;
            dropPosition.transform.position = new Vector3(-156f, 100f, -152.5f);
            UnityEngine.Object.Instantiate(dropPosition);
            bg.dropPosition = dropPosition.transform;
        }

        private void OnDestroy()
        {
            BossButtonController.OnButtonPressedActions -= CheckButtons;
        }

        private void CheckButtons()
        {
            if (!showedWarning && BossButtonController.buttonsPressed == BossButtonController.totalButtons - 1)
            {
                showedWarning = true;
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = "STADIAJUNGLEBOSS_SPAWN_WARNING"
                });
            }

            if (!spawnedBoss && BossButtonController.buttonsPressed >= BossButtonController.totalButtons)
            {
                spawnedBoss = true;
                spawnDelayCountdown = spawnDelay;
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = "STADIAJUNGLEBOSS_SPAWN_BEGIN"
                });
            }
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active) return;

            if (spawnDelayCountdown > 0f)
            {
                spawnDelayCountdown -= Time.fixedDeltaTime;

                if (spawnDelayCountdown <= 0f) DoSpawn();
            }
        }

        private void DoSpawn()
        {
            this.scriptedCombatEncounter.BeginEncounter();
        }
    }
}
