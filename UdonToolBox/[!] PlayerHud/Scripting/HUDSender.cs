
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HUDSender : UdonSharpBehaviour
{
    [Tooltip("The HUD Controller Script.")]
    public HUD HudController;

    [Tooltip("Should the trigger disable after use?\n> Use this to prevent the text from appearing again.")]
    public bool disableAfterUse = false;

    [Tooltip("Is this global?\nTRUE - All players see the HUD Update\nFALSE - Only the player who triggers this sees the HUD Update.")]
    public bool isGlobal = false;

    [Tooltip("The text to send to the HUD Controller.")]
    [TextArea]
    public string Text = "";

    [Tooltip("How many seconds inbetween each letter.\nEX: You can use 0.01 for 1/100th of a second.")]
    public float TickerSpeed = 0.01f;

    [Tooltip("Should the text play sound when displayed?")]
    public bool AudioPlays = true;

    public void OnPlayerTriggerEnter()
    {
            if (isGlobal)
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "SendToController");
            }
            else
            {
                SendToController();
            }



    }

    public void SendToController()
    {
        HudController.showingText = false;
        HudController.ShowText(Text, TickerSpeed, AudioPlays);
        if (disableAfterUse)
        {
            this.gameObject.SetActive(false);
        }
    }




}
