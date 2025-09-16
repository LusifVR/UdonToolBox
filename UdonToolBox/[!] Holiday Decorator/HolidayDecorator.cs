using System;
using System.Collections;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

// ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬ 
//         __  __       __  _       __                ____                                    __              
//        / / / / ____ / / (_)____ / / ____ _ ____   / __ \ ___   _____ ____   _____ ____  _ / /_ ___   _____
//       / /_/ // __ \ / // // __  // __ `// / / /  / / / // _ \ / ___// __ \ / ___// __ `// __// __ \ / ___/
//      / __  // /_/ // // // /_/ // /_/ // /_/ /  / /_/ //  __// /__ / /_/ // /   / /_/ // /_ / /_/ // /    
//     /_/ /_/ \____//_//_/ \__,_/ \__,_/ \__, /  /_____/ \___/ \___/ \____//_/    \__,_/ \__/ \____//_/     v2.1!
//                                       /____/                                                              
//
//    Made by Lusif#4807 for VRChat Udon     
//    Part of https://github.com/LusifVR/UdonToolBox  - Feel free to edit - please do not sell my code.  
//
// ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬
// Version 2.1!
// Changes:

// + Added tooltips :)
// + Added some more code-comments to make editing a bit easier
// + Added the ability to set custom holiday ranges
// + Added external compatibility (it wasnt there? . _. )
// + Added internal 'CurrentYear' variable

[UdonBehaviourSyncMode(BehaviourSyncMode.None)] // No sync mode since time is local.
public class HolidayDecorator : UdonSharpBehaviour
{
    #region Configuration

    #region Settings
    [Header("〚 Settings 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [Tooltip("Should the decorations be updated in realtime? (Constantly check clock/calendar)")]
    public bool RealtimeDecorations;
    [Tooltip("If the above is checked, how often should we check? (Seconds)")]
    [SerializeField] private float RealtimeUpdateRate = 300f;
    [Tooltip("Check date & time whenever a player joins?")]
    [SerializeField] private bool ChangeOnJoin = false;
    #endregion

    #region Holidays
    [Header("〚 Holidays 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]

    // While these are (mostly) western holidays, you are able to edit all of the variables
    // and or the ranges of the dates here to change when they begin or end, or if you want
    // to replace a western holiday with a non-western one.

    // To change variable names, just Find&Replace any instances of it, shared variables
    // use the same name (NewYears & NewYears_Start) >>> (YourHoliday & YourHoliday_Start)

    // By default, these have set months they are in, see SetMonthAndSeason() function
    // to edit the months your holidays are in. Simply move them to another switch as they
    // are numbered by the month of the year. (1-12)

    [SerializeField] private GameObject[] NewYears;
        [Tooltip("The day of the month the holiday begins.")]
            public int NewYears_Start = 30; // (December 30th)
        [Tooltip("The day of the month the holiday ends.")]
            public int NewYears_End = 3;

    [SerializeField] private GameObject[] Valentines;
        [Tooltip("The day of the month the holiday begins.")]
            public int Valentines_Start = 10;
        [Tooltip("The day of the month the holiday ends.")]
            public int Valentines_End = 17;

    [SerializeField] private GameObject[] StPatrick;
        [Tooltip("The day of the month the holiday begins.")]
            public int StPatrick_Start = 15;
        [Tooltip("The day of the month the holiday ends.")]
            public int StPatrick_End = 18;

    [SerializeField] private GameObject[] Easter;
        [Tooltip("The day of the month the holiday begins.")]
            public int Easter_Start = 1;
        [Tooltip("The day of the month the holiday ends.")]
            public int Easter_End = 20;

    [SerializeField] private GameObject[] IndependenceDay;
        [Tooltip("The day of the month the holiday begins.")]
            public int IndependenceDay_Start = 1;
        [Tooltip("The day of the month the holiday ends.")]
            public int IndependenceDay_End = 5;

    [SerializeField] private GameObject[] Halloween;
        [Tooltip("The day of the month the holiday begins.")]
            public int Halloween_Start = 20;
        [Tooltip("The day of the month the holiday ends.")]
            public int Halloween_End = 31;

    [SerializeField] private GameObject[] XMas;
        [Tooltip("The day of the month the holiday begins.")]
            public int XMas_Start = 20;
        [Tooltip("The day of the month the holiday ends.")]
            public int XMas_End = 29;
    #endregion

    #region Custom Holiday
    [Header("〚 Custom Holiday 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [Tooltip("Any objects you want to be enabled on your custom holiday.")]
    [SerializeField] private GameObject[] CustomHoliday;
    [Tooltip("What day does your custom holiday start?")]
    [SerializeField] private int CustomDay;
    [Tooltip("What month does your custom holiday reside?")]
    [SerializeField] private int CustomMonth;
    [Tooltip("What is the name of your holiday?")]
    [SerializeField] private string CustomName;
    [Tooltip("How long does your holiday last?")]
    [Range(0, 20)]
    [SerializeField] private int CustomDayRange;
    #endregion

    #region Seasonal
    [Header("〚 Seasonal 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [Tooltip("Objects to be enabled in Spring")]
    [SerializeField] private GameObject[] Spring;
    [Tooltip("Objects to be enabled in Summer")]
    [SerializeField] private GameObject[] Summer;
    [Tooltip("Objects to be enabled in Autumn")]
    [SerializeField] private GameObject[] Autumn;
    [Tooltip("Objects to be enabled in Winter")]
    [SerializeField] private GameObject[] Winter;
    #endregion

    #region Displays
    [Header("〚 Displays 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [Tooltip("Toggle the use of Text displays (Calendar Screen?)")]
    [SerializeField] private bool UseTextDisplays;
    [SerializeField] private TextMeshPro TodaysDate;
    #endregion

    #region External
    [Header("〚 External 〛")]
    [Tooltip("Any other animators to get variables of the day.\n[ (int) Year | (int) Month | (int) Day | (int) HolidayID | (int) Season ]")]
    [SerializeField] private Animator[] ExternalAnimators;
    [Tooltip("Any other scripts to get variables of the day and send Holiday_NewHoliday();\n[ (int) Year | (int) Month | (int) Day | (int) HolidayID | (int) Season ]")]
    [SerializeField] private UdonBehaviour[] ExternalScripts;
    [Tooltip("Should the above script(s) get a networked/global message?")]
    public bool SendGlobalEventMessage;
    #endregion

    #region Debug
    [Header("〚 Debug/Test 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    public bool isDebugMode;
    public int DEBUGDAY;
    public int DEBUGMONTH;

    public void SetDebugDate()
    {
        CurrentDay = DEBUGDAY;
        CurrentMonth = DEBUGMONTH;
        SetDecor();
    }
    #endregion

    #region Internal Variables
    private string[] monthNames = {
        "January", "February", "March", "April", "May", "June",
        "July", "August", "September", "October", "November", "December"
    };
    private int CurrentYear;
    private int CurrentDay;
    private int CurrentMonth;
    private string MonthName = "";
    private string HolidayName = "";
    private int CurrentSeason = 0; // 0: Spring, 1: Summer, 2: Autumn, 3: Winter
    private int CurrentHoliday = -1; // -1: No holiday
    #endregion

    #endregion

    #region Functions

    #region Initial Setup
    public void Start()
    {
        DisableAllDecorations();
        // Start by turning all off.


        if (!isDebugMode)
        {
            SetDate();
        }
        else
        {
            SetDebugDate();
        }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if (!isDebugMode && ChangeOnJoin)
        {
            SetDate();
        }
    }
    #endregion

    private void SetDate()
    {
        CurrentDay = DateTime.Now.Day;
        CurrentMonth = DateTime.Now.Month;
        CurrentYear = DateTime.Now.Year; // +

        SetDecor();
    }

    public void SetDecor()
    {
        SetMonthAndSeason();
        CheckForCustomHoliday();
        SetHoliday();
        SetSeasonal();
        SetDisplays();
        SetOtherConnections();

        if (RealtimeDecorations)
        {
            SendCustomEventDelayedSeconds(nameof(SetDecor), RealtimeUpdateRate);
        }
    }

    public void SetOtherConnections()
    {
        for (int i = 0; i < ExternalAnimators.Length; i++)
        {
            if (ExternalAnimators[i] != null)
            {
                ExternalAnimators[i].SetInteger("Day", CurrentDay);
                ExternalAnimators[i].SetInteger("Month", CurrentMonth);
                ExternalAnimators[i].SetInteger("Year", CurrentYear);
                ExternalAnimators[i].SetInteger("HolidayID", CurrentHoliday);
                ExternalAnimators[i].SetInteger("Season", CurrentSeason);
            }
        }

        for (int i = 0; i < ExternalScripts.Length; i++)
        {
            if (ExternalScripts[i] != null)
            {
                if (SendGlobalEventMessage)
                {
                    ExternalScripts[i].SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, "Holiday_NewHoliday");
                }
                else
                {
                    ExternalScripts[i].SendCustomEvent("Holiday_NewHoliday");
                }

                ExternalScripts[i].SetProgramVariable("Day",CurrentDay);
                ExternalScripts[i].SetProgramVariable("Month", CurrentMonth);
                ExternalScripts[i].SetProgramVariable("Year", CurrentYear);
                ExternalScripts[i].SetProgramVariable("HolidayID", CurrentHoliday);
                ExternalScripts[i].SetProgramVariable("Season", CurrentSeason);
            }
        }
    }

    private void SetMonthAndSeason()
    {
        MonthName = monthNames[CurrentMonth - 1];

        switch (CurrentMonth)
        {
            case 1:
                if (CurrentDay <= NewYears_End) { CurrentHoliday = 0; } // New Years
                CurrentSeason = 3;
                break;
            case 2:
                if (CurrentDay >= Valentines_Start && CurrentDay <= Valentines_End) { CurrentHoliday = 1; } // Valentines
                CurrentSeason = 3;
                break;
            case 3:
                if (CurrentDay >= StPatrick_Start && CurrentDay <= StPatrick_End) { CurrentHoliday = 2; } // St. Patricks day
                CurrentSeason = 0;
                break;
            case 4:
                if (CurrentDay >= Easter_Start && CurrentDay <= Easter_End) { CurrentHoliday = 3; } // Easter
                CurrentSeason = 0;
                break;
            case 5:
                CurrentSeason = 0;
                break;
            case 6:
                CurrentSeason = 1;
                break;
            case 7:
                if (CurrentDay >= IndependenceDay_Start && CurrentDay <= IndependenceDay_End) { CurrentHoliday = 4; }
                CurrentSeason = 1;
                break;
            case 8:
                CurrentSeason = 1;
                break;
            case 9:
                CurrentSeason = 2;
                break;
            case 10:
                if (CurrentDay >= Halloween_Start && CurrentDay <= Halloween_End) { CurrentHoliday = 5; }
                CurrentSeason = 2;
                break;
            case 11:
                CurrentSeason = 2;
                break;
            case 12:
                if (CurrentDay >= XMas_Start && CurrentDay <= XMas_End) { CurrentHoliday = 6; }
                if (CurrentDay >= NewYears_Start) { CurrentHoliday = 0; }
                CurrentSeason = 3;
                break;
            default:
                MonthName = "?";
                break;
        }
    }

    private void CheckForCustomHoliday()
    {
        // Check for custom Holiday
        if (CustomMonth == CurrentMonth || (CustomMonth == CurrentMonth - 1 && CustomDay + CustomDayRange > DateTime.DaysInMonth(DateTime.Now.Year, CustomMonth)))
        {
            // Compute days in current and previous month
            int daysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, CurrentMonth);
            int daysInCustomMonth = DateTime.DaysInMonth(DateTime.Now.Year, CustomMonth);

            // Calculate the end day for the holiday, considering overflow
            int endDay = CustomDay + CustomDayRange;
            if (endDay <= daysInCustomMonth)
            {
                // Custom holiday does not overflow into the next month
                if (CurrentDay >= CustomDay && CurrentDay <= endDay)
                {
                    ActivateHolidayObjects(CustomHoliday);
                }
            }
            else
            {
                // Custom holiday overflows into the next month
                int overflowDays = endDay - daysInCustomMonth;

                // Check if we are within the overflow period
                if (CurrentDay >= CustomDay || CurrentDay <= overflowDays)
                {
                    ActivateHolidayObjects(CustomHoliday);
                }
            }
        }
    }

    private void SetHoliday()
    {
        switch (CurrentHoliday)
        {
            case 0: ActivateHolidayObjects(NewYears); break;
            case 1: ActivateHolidayObjects(Valentines); break;
            case 2: ActivateHolidayObjects(StPatrick); break;
            case 3: ActivateHolidayObjects(Easter); break;
            case 4: ActivateHolidayObjects(IndependenceDay); break;
            case 5: ActivateHolidayObjects(Halloween); break;
            case 6: ActivateHolidayObjects(XMas); break;
            default: break;
        }
    }

    private void SetSeasonal()
    {
        GameObject[] seasonObjects = null;
        switch (CurrentSeason)
        {
            case 0: seasonObjects = Spring; break;
            case 1: seasonObjects = Summer; break;
            case 2: seasonObjects = Autumn; break;
            case 3: seasonObjects = Winter; break;
        }
        if (seasonObjects != null) ActivateHolidayObjects(seasonObjects);
    }

    private void ActivateHolidayObjects(GameObject[] objects)
    {
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    private void DisableAllDecorations()
    {
        // Disable all objects for every holiday and season on start
        // This is to ensure any referenced objects arent left on by
        // mistake in case you need to test things.

        foreach (var obj in NewYears) { if (obj) obj.SetActive(false); }
        foreach (var obj in Valentines) { if (obj) obj.SetActive(false); }
        foreach (var obj in StPatrick) { if (obj) obj.SetActive(false); }
        foreach (var obj in Easter) { if (obj) obj.SetActive(false); }
        foreach (var obj in IndependenceDay) { if (obj) obj.SetActive(false); }
        foreach (var obj in Halloween) { if (obj) obj.SetActive(false); }
        foreach (var obj in XMas) { if (obj) obj.SetActive(false); }
        foreach (var obj in CustomHoliday) { if (obj) obj.SetActive(false); }
        foreach (var obj in Spring) { if (obj) obj.SetActive(false); }
        foreach (var obj in Summer) { if (obj) obj.SetActive(false); }
        foreach (var obj in Autumn) { if (obj) obj.SetActive(false); }
        foreach (var obj in Winter) { if (obj) obj.SetActive(false); }
    }



    public void SetDisplays()
    {
        if (TodaysDate != null)
        {
            TodaysDate.text = $"Today's Date:\n{MonthName} {CurrentDay}\n{HolidayName}";
        }
    }

    #endregion
}