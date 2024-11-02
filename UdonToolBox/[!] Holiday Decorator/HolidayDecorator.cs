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
//     /_/ /_/ \____//_//_/ \__,_/ \__,_/ \__, /  /_____/ \___/ \___/ \____//_/    \__,_/ \__/ \____//_/     v2!
//                                       /____/                                                              
//
//    Made by Lusif#4807 for VRChat Udon     
//    Part of https://github.com/LusifVR/UdonToolBox  - Feel free to edit - please do not sell my code.  
//
// ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬
// Version 2.0!
// Changes:

// + Added the ability to have an object in multiple holidays.
// + Added custom animators and external script output.
// + Optimized the update with a customizable rate.
// + Added the option to enable/disable playerjoin updates
// + Added Independence Day.
// + Added a custom range for custom holidays.

[UdonBehaviourSyncMode(BehaviourSyncMode.None)] // Added no sync mode < not needed to sync as time is local.
public class HolidayDecorator : UdonSharpBehaviour
{
    #region Configuration

    #region Settings
    [Header("〚 Settings 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [Tooltip("Should the updates happen in Realtime?\nNOTE: This may cause lag if you have lots of complicated decorations.(Like over 100)\nOnly use this if you aren't going over the top. Otherwise,\nThe script updates only on world refresh or player joins.")]
    public bool RealtimeDecorations;
    [Tooltip("How often (In Seconds) should the script update? Default - 300 (5 minutes)")]
    [SerializeField] private float RealtimeUpdateRate = 300f;

    [Tooltip("Enabling this will make HolidayDecorator check the date whenever a new player joins.")]
    [SerializeField] private bool ChangeOnJoin = false;
    #endregion

    #region Holidays
    [Header("〚 Holidays 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [Tooltip("Objects for Newyears!\n(December 30th to January 2nd)")]
    [SerializeField] private GameObject[] NewYears;
    [Tooltip("Objects for Valentines Day!\n(February 10th to 17th)")]
    [SerializeField] private GameObject[] Valentines;
    [Tooltip("Objects for St. Patrick's Day!\n(March 15th to 18th)")]
    [SerializeField] private GameObject[] StPatrick;
    [Tooltip("Objects for Easter!\n(April 6th to 11th)")]
    [SerializeField] private GameObject[] Easter;
    [Tooltip("Objects for fourth of July!\n(July 1st to 6th)")]
    [SerializeField] private GameObject[] IndependenceDay;
    [Tooltip("Objects for Halloween!\n(Oct 20th to November 2nd)")]
    [SerializeField] private GameObject[] Halloween;
    [Tooltip("Objects for XMas!\n(Dec 20th to 29th)")]
    [SerializeField] private GameObject[] XMas;
    #endregion

    #region Custom Holiday
    [Header("〚 Custom Holiday 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [Tooltip("Objects for a custom holiday!\n(Maybe a non-western one or an anniversary?)\nThis will spawn these objects for 2 days before and 2 days after.")]
    [SerializeField] private GameObject[] CustomHoliday;
    [Tooltip("Day of the week.\nNumber of the day. (1-31)")]
    [SerializeField] private int CustomDay;
    [Tooltip("Month of the year.\nNumber of the month. (1-12)")]
    [SerializeField] private int CustomMonth;
    [Tooltip("What is your custom Holiday?\n(Displays on the ui for that timeframe)\nLeave blank if you don't want this.")]
    [SerializeField] private string CustomName;
    [Tooltip("How many days after the starting should your custom holiday persist?\nLeave zero if you want a single-day holiday.")]
    [Range(0,10)]
    [SerializeField] private int CustomDayRange;
    #endregion

    #region Seasonal
    [Header("〚 Seasonal 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [Tooltip("Anything that should be active during Springtime\n(March, April, May)")]
    [SerializeField] private GameObject[] Spring;
    [Tooltip("Anything that should be active during Summertime\n(June, July, August)")]
    [SerializeField] private GameObject[] Summer;
    [Tooltip("Anything that should be active during Autumntime\n(September, October, November)")]
    [SerializeField] private GameObject[] Autumn;
    [Tooltip("Anything that should be active during Wintertime\n(December, January, February)")]
    [SerializeField] private GameObject[] Winter;
    #endregion

    #region Displays
    [Header("〚 Displays 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [Tooltip("Enable or Disable the feature below")]
    [SerializeField] private bool UseTextDisplays;
    [SerializeField] private TextMeshPro TodaysDate;
    #endregion

    #region External
    [Header("〚 External 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [Tooltip("Animators that get the date index ints updated at the same rate.\nUse this if you want to change materials, fade ins, etc.")]
    [SerializeField] private Animator[] ExternalAnimators; // int - CurrentMonth | int - CurrentDay | int - CurrentHoliday | int - CurrentSeason
    [Tooltip("Other scripts that you may want to send events to based on the season or holiday.\nThe event name is the holiday with prefix.\nEx: New Years is 'HD_Holiday_0()' | Winter is 'HD_Season_3()'")]
    [SerializeField] private UdonBehaviour[] ExternalScripts;
    #endregion

    #region Debug
    [Space(2)]
    [Header("〚 Debug/Test 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    //For Debug and Testing Purposes 
    [Tooltip("Should the script follow this date instead? Test your decor?\nMAKE SURE TO TURN THIS OFF BEFORE UPLOADING.")]
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
    // These are private variables used internally.
    // Internal usage for language change or if you'd like to have fantasy months
    // Only mess with these if you know what you are doing.
    private string[] monthNames =
    {
        "January",
        "February",
        "March",
        "April",
        "May",
        "June",
        "July",
        "August",
        "September",
        "October",
        "November",
        "December"
    };
    
    private int CurrentDay;
    private int CurrentMonth;
    private string MonthName = "";
    private string HolidayName = "";
    private int CurrentSeason = 0; // 0 Spring | 1 Summer | 2 Autumn | 3 Winter
    private int CurrentHoliday = -1; // Start as negative 1 (invalid) so we dont have holidays starting on

    // 0 New Years
    // 1 Valentines
    // 2 StPatrick
    // 3 Easter
    // 4 Independence Day
    // 5 Halloween
    // 6 XMas
    // 7 CustomHoliday
    #endregion

    #endregion

    #region Functions

    #region Initial Setup
    public void Start()
    {

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
        SetDecor();
    }

    public void SetDecor()
    {
        #region Get month for seasons and holidays
        switch (CurrentMonth) // Make any adjustmets to values based on month
        {
            case 1:
                MonthName = monthNames[0]; // January
                if (CurrentDay <= 2) { CurrentHoliday = 0; } // Set for new years
                CurrentSeason = 3;
                break;
            case 2:
                MonthName = monthNames[1]; // February
                if (CurrentDay >= 10 && CurrentDay <= 17) { CurrentHoliday = 1; } // Set for Valentines
                CurrentSeason = 3;
                break;
            case 3:
                MonthName = monthNames[2]; // March
                if (CurrentDay >= 15 && CurrentDay <= 18) { CurrentHoliday = 2; } // Set for St. Patricks day
                CurrentSeason = 0;
                break;
            case 4:
                MonthName = monthNames[3]; // April
                if (CurrentDay >= 1 && CurrentDay <= 20) { CurrentHoliday = 3; } // Set for Easter
                CurrentSeason = 0;
                break;
            case 5:
                MonthName = monthNames[4]; // May
                CurrentSeason = 0;
                break;
            case 6:
                MonthName = monthNames[5]; // June
                CurrentSeason = 1;
                break;
            case 7:
                MonthName = monthNames[6]; // July
                if (CurrentDay >= 1 && CurrentDay <= 6) { CurrentHoliday = 4; } // Set for Independence Day
                CurrentSeason = 1;
                break;
            case 8:
                MonthName = monthNames[7]; // August
                CurrentSeason = 1;
                break;
            case 9:
                MonthName = monthNames[8]; // September
                CurrentSeason = 2;
                break;
            case 10:
                MonthName = monthNames[9]; // October
                if (CurrentDay >= 20 && CurrentDay <= 31) { CurrentHoliday = 5; } // Set for Halloween
                CurrentSeason = 2;
                break;
            case 11:
                MonthName = monthNames[10]; // November
                CurrentSeason = 2;
                break;
            case 12:
                MonthName = monthNames[11]; // December
                if (CurrentDay >= 20 && CurrentDay <= 29) { CurrentHoliday = 6; } // Set for Christmas
                if (CurrentDay >= 30 && CurrentDay <= 31) { CurrentHoliday = 0; } // Set for New Years
                CurrentSeason = 3;
                break;
            default:
                MonthName = "?"; // Month was not through 1-12
                break;
        }
        #endregion

        // Check for custom Holiday
        if (CustomMonth == CurrentMonth)
        {
            Debug.Log("CustomMonthFound");
            // Due to the nature of CustomDayRange, it may cause the overlap to bleed into the
            // next month. Thanks to Stray for pointing out this oversight. To fix this issue
            // we'll cache how many days are in the month and then calculate the endday


            int daysInCurrentMonth = DateTime.DaysInMonth(DateTime.Now.Year, CurrentMonth);
            int endDay = CustomDay + CustomDayRange;

            if (endDay <= daysInCurrentMonth)
            {
                // Holiday resides within this month.
                if (CurrentDay >= CustomDay && CustomDay < endDay)
                {
                    CheckCustomHoliday();
                }
            }
            else
            {
                // The holiday bleeds into next month
                int overflowDays = endDay - daysInCurrentMonth;

                if (CurrentDay >= CustomDay || CurrentDay <= overflowDays)
                {
                    CheckCustomHoliday();
                }       
            }
        }


        // We want to apply seasonal decorations after the holiday decorations
        // as seasons change less often than holidays or months do

        SetHoliday();
        SetSeasonal();
        SetDisplays();

        if (RealtimeDecorations) // If you have enabled realtime update, this rate applies changes every "RealtimeUpdateRate" seconds.
        {
            SendCustomEventDelayedSeconds(nameof(SetDecor), RealtimeUpdateRate);
        }
    }

    private void SetHoliday()
    {
        // Now enable objects per season
        switch (CurrentHoliday)
        {
            case 0:
                for (int i = 0; i < NewYears.Length; i++)
                {
                    if (NewYears[i])
                    { // Check if each object is valid, if not- skip.
                        NewYears[i].SetActive(true);
                    }
                }
                break;
            case 1:
                for (int i = 0; i < Valentines.Length; i++)
                {
                    if (Valentines[i])
                    { // Check if each object is valid, if not- skip.
                        Valentines[i].SetActive(true);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < StPatrick.Length; i++)
                {
                    if (StPatrick[i])
                    { // Check if each object is valid, if not- skip.
                        StPatrick[i].SetActive(true);
                    }
                }
                break;
            case 3:
                for (int i = 0; i < Easter.Length; i++)
                {
                    if (Easter[i])
                    { // Check if each object is valid, if not- skip.
                        Easter[i].SetActive(true);
                    }
                }
                break;
            case 4:
                for (int i = 0; i < IndependenceDay.Length; i++)
                {
                    if (IndependenceDay[i])
                    { // Check if each object is valid, if not- skip.
                        IndependenceDay[i].SetActive(true);
                    }
                }
                break;
            case 5:
                for (int i = 0; i < Halloween.Length; i++)
                {
                    if (Halloween[i])
                    { // Check if each object is valid, if not- skip.
                        Halloween[i].SetActive(true);
                    }
                }
                break;
            case 6:
                for (int i = 0; i < XMas.Length; i++)
                {
                    if (XMas[i])
                    { // Check if each object is valid, if not- skip.
                        XMas[i].SetActive(true);
                    }
                }
                break;
                // moved custom holiday to its own function
        }
    }
    
    private void CheckCustomHoliday()
    {
        for (int i = 0; i < CustomHoliday.Length; i++)
        {
            if (CustomHoliday[i])
            { // Check if each object is valid, if not- skip.
                CustomHoliday[i].SetActive(true);
            }
        }
    }

    private void SetSeasonal()
    {
        // Get the range of months and apply seasonal items on.
        // First, disable all objects on a change.

        

        // Now enable objects per the season we are in.
        switch (CurrentSeason)
        {
            case 0:
                for (int i = 0; i < Spring.Length; i++) // SPRING <<<
                {
                    if (Spring[i]) // Check if each object is valid, if not- skip.
                    {
                        Spring[i].SetActive(true);
                    }
                }
                break;
            case 1:
                for (int i = 0; i < Summer.Length; i++) // SUMMER <<<
                {
                    if (Summer[i]) // Check if each object is valid, if not- skip.
                    {
                        Summer[i].SetActive(true);
                    }
                }
                break;
            case 2:
                for (int i = 0; i < Autumn.Length; i++) // AUTUMN <<<
                {
                    if (Autumn[i]) // Check if each object is valid, if not- skip.
                    {
                        Autumn[i].SetActive(true);
                    }
                }
                break;
            case 3:
                for (int i = 0; i < Winter.Length; i++) // WINTER <<<
                {
                    if (Winter[i]) // Check if each object is valid, if not- skip.
                    {
                        Winter[i].SetActive(true);
                    }
                }
                break;
            default:
                Debug.Log("[Holiday Decorator] The Season was invalid.");
                break;
        }
    }

    private void SetExternal()
    {
        for (int i = 0; i < ExternalAnimators.Length; i++)
        {
            if (ExternalAnimators[i])
            {
                ExternalAnimators[i].SetInteger("CurrentDay", CurrentDay);
                ExternalAnimators[i].SetInteger("CurrentMonth", CurrentMonth);
                ExternalAnimators[i].SetInteger("CurrentHoliday", CurrentHoliday);
                ExternalAnimators[i].SetInteger("CurrentSeason", CurrentSeason);
            }
        }
        for (int i = 0; i < ExternalScripts.Length; i++)
        {
            if (ExternalScripts[i])
            {
                ExternalScripts[i].SendCustomEvent("HD_Holiday_" + CurrentHoliday);
                ExternalScripts[i].SendCustomEvent("HD_Season_" + CurrentSeason);
            }
        }
    }

    public void SetDisplays()
    {
        TodaysDate.text = ("Today's Date:\n" + MonthName + " " + CurrentDay + "\n" + HolidayName);
    }
    #endregion
}


