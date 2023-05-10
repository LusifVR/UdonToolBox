using System.Collections;
using System.Collections.Generic;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

public class HUD : UdonSharpBehaviour
{
    [Header("HUD CONTROLLER")]


    [Tooltip("The TextMeshProGUI to set text.\nThis is what the player sees.")]
    public TextMeshProUGUI HUDText;

    [Tooltip("The scale of your UI.\nYou should typically keep this at 1,\nbut you can mess around with it.")]
    public float HUDScale;

    [Header("Sounds")]
    [Tooltip("The sound to play when a new letter appears.")]
    public AudioClip TextSound;
    [Tooltip("The sound to play when the text clears.")]
    public AudioClip ClearSound;
    [Tooltip("The actual audio source that these sounds come from.")]
    public AudioSource Speaker;


    // these are all private variables used by the script
    // the ones labeled 'hideInInspector' but are also
    // public, are the ones used by the Sender and Triggers.
    [HideInInspector] public float timer = 0.0f;
    private bool cleared = false;
    int i = 0;
    [HideInInspector] public string SHOWtext;
    [HideInInspector] public float SHOWfloat;
    [HideInInspector] public bool SHOWbool;
    [HideInInspector] public bool showingText = false;

    private void Start()
    {
        HUDText.text = ""; // clear the text on start
    }


    public void ShowText(string display, float ticker, bool plays)
    { // Get info from Sender(s)
        HUDText.text = "";
        SHOWtext = display;
        SHOWfloat = ticker;
        SHOWbool = plays;
        cleared = false;
        showingText = true;
    }




    void Update()
    {
        if (showingText)
        {
            if (i < SHOWtext.Length)
            {
                timer += Time.deltaTime;
                if (timer > SHOWfloat)
                {
                    char c = SHOWtext[i];
                    HUDText.text = HUDText.text + c.ToString(); // For every letter, play sound and add to string of text
                    if (SHOWbool)
                    {
                        Speaker.PlayOneShot(TextSound);
                    }
                    i++;
                    timer = 0.0f;
                }
            }
            else
            {
                timer += Time.deltaTime;
                if (timer > 5 && !cleared) // if 5 seconds have passed and all text is displayed, clear
                {
                    if (SHOWbool)
                    {
                        Speaker.PlayOneShot(ClearSound);
                    }
                    cleared = true;
                    HUDText.text = "";
                }
            }
        }

        if (cleared)
        {
            showingText = false;
            timer = 0.0f;
            i = 0;
        }

        this.transform.position = (Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position); // Track hud to player
        this.transform.rotation = (Networking.LocalPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation);
        this.transform.localScale = new Vector3(HUDScale, HUDScale, HUDScale);

    }
}
