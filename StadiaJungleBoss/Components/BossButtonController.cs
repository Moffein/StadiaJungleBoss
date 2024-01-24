using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace StadiaJungleBoss.Components
{
    public class BossButtonController : NetworkBehaviour
    {
        public delegate void OnButtonPressed();
        public static OnButtonPressed OnButtonPressedActions;

        public static int totalButtons = 0;
        public static int buttonsPressed = 0;

        [SyncVar]
        public bool isPressedServer = false;

        public bool isPressedLocal = false;

        private void Awake()
        {
            this.animator = base.GetComponent<Animator>();
            if (!animator) Debug.LogError("StadiaJungleBoss: Could not find button animator");

            if (NetworkServer.active) this.isPressedServer = false;
            this.isPressedLocal = false;

            this.overlapSphereRadius = 1.5f;
            this.overlapSphereFrequency = 5f;
            this.enableOverlapSphere = true;

            if (BossButtonController.totalButtons < 4) BossButtonController.totalButtons++;
        }

        //Client handles press visual
        private void FixedUpdate()
        {
            if (NetworkServer.active) FixedUpdateServer();

            if (!isPressedLocal)
            {
                if (isPressedServer)
                {
                    Pressed();
                }
            }
        }

        //Server handles press logic
        private void FixedUpdateServer()
        {
            if (!isPressedServer && this.enableOverlapSphere)
            {
                float overlapTick = 1f / this.overlapSphereFrequency;
                this.overlapSphereStopwatch += Time.fixedDeltaTime;
                if (this.overlapSphereStopwatch >= overlapTick)
                {
                    this.overlapSphereStopwatch -= overlapTick;
                    if (Physics.OverlapSphere(base.transform.position, this.overlapSphereRadius, LayerIndex.defaultLayer.mask | LayerIndex.fakeActor.mask, QueryTriggerInteraction.UseGlobal).Length != 0)
                    {
                        this.isPressedServer = true;
                        BossButtonController.buttonsPressed++;
                        OnButtonPressedActions?.Invoke();
                    }
                }
            }
        }

        private void Pressed()
        {
            isPressedLocal = true;
            this.enableOverlapSphere = false;
            bool flag2 = this.animator;
            if (flag2)
            {
                this.animator.SetBool("pressed", true);
            }
            Util.PlaySound("Play_item_proc_bandolierSpawn", base.gameObject);
        }

        private bool enableOverlapSphere;
        private float overlapSphereRadius;
        private float overlapSphereStopwatch;
        private float overlapSphereFrequency;
        private Animator animator;
    }
}
