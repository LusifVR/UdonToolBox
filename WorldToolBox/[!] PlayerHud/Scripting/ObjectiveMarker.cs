
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using TMPro;
using System;
using VRC.Udon;

public class ObjectiveMarker : UdonSharpBehaviour
{
    [Header("Objective Markers")]

    [Tooltip("The posisitions of all your Objectives.")]
    public Transform[] Objectives;
    [Tooltip("Icons to set for Each Objective\nThis should be the same amount as Objectives.")]
    public Transform[] UiIcons;
    [Tooltip("Text Displays for Each Icon for distance.\nThis should be the same amount as Objectives.")]
    public TextMeshProUGUI[] TextDisplays;



    // Private Variables used by the code.
    private int dist;
    private float offset;
    private Vector3 CurPlayPos;

    void Update()
    {
        if (UiIcons.Length == Objectives.Length && Objectives.Length == TextDisplays.Length) 
            // Check to see if the arrays are the same length
        {
            for (int i = 0; i < UiIcons.Length; i++) 
                // Do this function for every icon that you set
            {
                UiIcons[i].position = Objectives[i].position; 
                // Set Ui Icons to Objective Transform posistions

                CurPlayPos = Networking.LocalPlayer.GetPosition(); 
                // Get the local player's position

                offset = Vector3.Distance(CurPlayPos, UiIcons[i].position); 
                // Get the distance between the player and each objective

                dist = (int)Math.Round(offset); 
                // Round to the nearest integer (whole number)

                TextDisplays[i].text = (dist.ToString() + "m"); 
                // Set the text of each icon to the distance to the player (plus add a m for meters)

                UiIcons[i].localScale = new Vector3(offset * 5, offset * 5, UiIcons[i].localScale.z); 
                // Set the scale of each icon based on the distance to the player

            }
        }
        else
        {
            Debug.LogWarning("Your Objective Markers and Icons do not match!\nBe sure all of your Arrays are the same length.");
            // This prevents the code from crashing and doesn't update the icons if it fails to find the Icons.
        }

    }
}
