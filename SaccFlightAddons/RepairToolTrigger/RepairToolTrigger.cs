
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;



namespace SaccFlightAndVehicles
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class RepairToolTrigger : UdonSharpBehaviour
    {
        [Tooltip("Object to send event to")]
        public SaccGroundVehicle VehicleCore;
        [Tooltip("Object to send wakeup to")]
        public SGV_EffectsController VehicleEffects;
        [Tooltip("Name of event sent by this trigger")]
        public string EventName = "ReSupply";

        public void OnParticleCollision()
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            SendRepair();
        }

        public void SendRepair()
        {
            VehicleCore.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, EventName);
            VehicleEffects.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "WakeUp");
        }


    }
}

