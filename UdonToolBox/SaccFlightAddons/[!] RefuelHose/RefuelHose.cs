using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RefuelHose : UdonSharpBehaviour
{
    [HideInInspector] public bool hasProbe = false;
    [Tooltip("This is the trigger attached to this object.")]
    public Collider DrogueTrigger;
    [Tooltip("The layer of refueling probes. This should be the same as your vehicle (OnboardVehicleLayer 31)")]
    public LayerMask triggerLayer;
    [Tooltip("The maximum distance the plane can be from the refueler before the basket/drogue disconnects.")]
    public float MaxDistance = 5.0f;
    [Tooltip("How long the script should wait before accepting another detach? (Seconds)")]
    public float RefreshDuration = 1f;

    [Tooltip("The parent bone/object of the hose, this is what the Maxdistance checks.")]
    public Transform HoseParent; // Parent to revert to
    private Transform CurrentVehicle; // Current Vehicle
    private Vector3 OriginalLocation; // The saved location of the basket
        // This is here to prevent the basket from getting stuck to the plane
        // Not the most realistic solution but yea

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & triggerLayer) != 0)
        {
            if (other.gameObject.name == "Probe")
            {
                // Set the original position of the basket.
                OriginalLocation = this.transform.localPosition;
                Transform Vehreference = other.gameObject.transform;
                CurrentVehicle = Vehreference;
                this.transform.SetParent(CurrentVehicle);
                this.transform.localPosition = Vector3.zero;
                hasProbe = true;
            }
            else if (other.gameObject.transform.GetChild(0).name=="AttachLocation") // If there is no probe, check for a reparent spot
            {
                // Set the original position of the basket.
                OriginalLocation = this.transform.localPosition;
                Transform Vehreference = other.gameObject.transform;
                CurrentVehicle = Vehreference;
                this.transform.SetParent(CurrentVehicle);
                this.transform.localPosition = other.gameObject.transform.GetChild(0).position;
                this.transform.localRotation = other.gameObject.transform.GetChild(0).rotation;
                hasProbe = true;
            }
        }
    }

    private void Update()
    {
        if (hasProbe && HoseParent != null && CurrentVehicle != null)
        {
            float distance = Vector3.Distance(HoseParent.position, CurrentVehicle.position);
            //Debug.Log($"[Refueling] Distance: {distance}");

            if (distance > MaxDistance)
            {
                //Debug.Log("[Refueling] Distance exceeded! Forcing detach.");
                ForceDetach();
            }
        }
    }

    public void ForceDetach()
    {
        hasProbe = false;
        this.transform.SetParent(HoseParent);
        this.transform.localPosition = OriginalLocation;
        DrogueTrigger.enabled = false;
        SendCustomEventDelayedSeconds(nameof(ReadyDrogue), RefreshDuration);
    }

    public void ReadyDrogue()
    {
        DrogueTrigger.enabled = true;
        //Debug.Log("[Refueling] Ready for next refuel");
    }
}