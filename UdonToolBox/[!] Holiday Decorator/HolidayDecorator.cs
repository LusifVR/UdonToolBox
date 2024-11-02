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

[UdonBehaviourSyncMode(BehaviourSyncMode.None)] // No sync mode since time is local.
public class HolidayDecorator : UdonSharpBehaviour
{
    #region Configuration

    #region Settings
    [Header("〚 Settings 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    public bool RealtimeDecorations;
    [SerializeField] private float RealtimeUpdateRate = 300f;
    [SerializeField] private bool ChangeOnJoin = false;
    #endregion

    #region Holidays
    [Header("〚 Holidays 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [SerializeField] private GameObject[] NewYears;
    [SerializeField] private GameObject[] Valentines;
    [SerializeField] private GameObject[] StPatrick;
    [SerializeField] private GameObject[] Easter;
    [SerializeField] private GameObject[] IndependenceDay;
    [SerializeField] private GameObject[] Halloween;
    [SerializeField] private GameObject[] XMas;
    #endregion

    #region Custom Holiday
    [Header("〚 Custom Holiday 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [SerializeField] private GameObject[] CustomHoliday;
    [SerializeField] private int CustomDay;
    [SerializeField] private int CustomMonth;
    [SerializeField] private string CustomName;
    [Range(0, 10)]
    [SerializeField] private int CustomDayRange;
    #endregion

    #region Seasonal
    [Header("〚 Seasonal 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [SerializeField] private GameObject[] Spring;
    [SerializeField] private GameObject[] Summer;
    [SerializeField] private GameObject[] Autumn;
    [SerializeField] private GameObject[] Winter;
    #endregion

    #region Displays
    [Header("〚 Displays 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [SerializeField] private bool UseTextDisplays;
    [SerializeField] private TextMeshPro TodaysDate;
    #endregion

    #region External
    [Header("〚 External 〛")]
    [SerializeField] private Animator[] ExternalAnimators;
    [SerializeField] private UdonBehaviour[] ExternalScripts;
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
        SetDecor();
    }

    public void SetDecor()
    {
        SetMonthAndSeason();
        CheckForCustomHoliday();
        SetHoliday();
        SetSeasonal();
        SetDisplays();

        if (RealtimeDecorations)
        {
            SendCustomEventDelayedSeconds(nameof(SetDecor), RealtimeUpdateRate);
        }
    }

    private void SetMonthAndSeason()
    {
        MonthName = monthNames[CurrentMonth - 1];

        switch (CurrentMonth)
        {
            case 1:
                if (CurrentDay <= 2) { CurrentHoliday = 0; }
                CurrentSeason = 3;
                break;
            case 2:
                if (CurrentDay >= 10 && CurrentDay <= 17) { CurrentHoliday = 1; }
                CurrentSeason = 3;
                break;
            case 3:
                if (CurrentDay >= 15 && CurrentDay <= 18) { CurrentHoliday = 2; }
                CurrentSeason = 0;
                break;
            case 4:
                if (CurrentDay >= 1 && CurrentDay <= 20) { CurrentHoliday = 3; }
                CurrentSeason = 0;
                break;
            case 5:
                CurrentSeason = 0;
                break;
            case 6:
                CurrentSeason = 1;
                break;
            case 7:
                if (CurrentDay >= 1 && CurrentDay <= 6) { CurrentHoliday = 4; }
                CurrentSeason = 1;
                break;
            case 8:
                CurrentSeason = 1;
                break;
            case 9:
                CurrentSeason = 2;
                break;
            case 10:
                if (CurrentDay >= 20 && CurrentDay <= 31) { CurrentHoliday = 5; }
                CurrentSeason = 2;
                break;
            case 11:
                CurrentSeason = 2;
                break;
            case 12:
                if (CurrentDay >= 20 && CurrentDay <= 29) { CurrentHoliday = 6; }
                if (CurrentDay >= 30) { CurrentHoliday = 0; }
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