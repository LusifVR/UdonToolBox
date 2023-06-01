
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VehicleStorage : UdonSharpBehaviour
{
    [Header("Edit the box collider to resize. DO NOT SCALE THIS OBJECT!")]
    [Space(20)]
    [Tooltip("Transform outside this vehicle. An empty will do fine.\nThis should be outside the vehicle completely\nfor example, your VRCWorld Object.")]
    public Transform External;
    [Tooltip("The currently packed item.")]
    [HideInInspector] public GameObject Store;    

    public void OnTriggerEnter(Collider other)
    {
        Store = other.gameObject;
        if (Store.layer == 13)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "PackItem");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Store = other.gameObject;
        if (Store.layer == 13)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "UnpackItem");
        }
    }

    public void PackItem()
    {
        Store.transform.parent = transform;
    }

    public void UnpackItem()
    {
        Store.transform.parent = External;
    }


}
