using System;
using System.Collections;
using UdonSharp;
using UnityEditor;
//using UnityEditor.UI; << Removed unneeded library (Thank you Stray for pointing this out!)
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

//
//         __  __       __  _       __                ____                                    __              
//        / / / / ____ / / (_)____ / / ____ _ ____   / __ \ ___   _____ ____   _____ ____  _ / /_ ___   _____
//       / /_/ // __ \ / // // __  // __ `// / / /  / / / // _ \ / ___// __ \ / ___// __ `// __// __ \ / ___/
//      / __  // /_/ // // // /_/ // /_/ // /_/ /  / /_/ //  __// /__ / /_/ // /   / /_/ // /_ / /_/ // /    
//     /_/ /_/ \____//_//_/ \__,_/ \__,_/ \__, /  /_____/ \___/ \___/ \____//_/    \__,_/ \__/ \____//_/     
//                                       /____/                                                              
//
//    Made by Lusif#4807 for VRChat Udon
//      
//    Part of the UdonSim/UdonToolBox Public scripts Repo
//
//    Feel free to edit - please do not sell my code.
//
//


public class HolidayDecorator : UdonSharpBehaviour
{
    [Header("〚 Settings 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]

    [Tooltip("Should the updates happen in Realtime?\nNOTE: This may cause lag if you have lots of complicated decorations. (Like over 100)\nOnly use this if you aren't going over the top. Otherwise,\nThe script updates only on world refresh or player joins.")]
    public bool RealtimeDecorations;


    [Header("〚 Holidays 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [Tooltip("Objects for Newyears!\n(December 30th to January 2nd)")]
    public GameObject[] NewYears;
    [Tooltip("Objects for Valentines Day!\n(February 10th to 17th)")]
    public GameObject[] Valentines;
    [Tooltip("Objects for St. Patrick's Day!\n(March 15th to 18th)")]
    public GameObject[] StPatrick;
    [Tooltip("Objects for Easter!\n(April 6th to 11th)")]
    public GameObject[] Easter;
    [Tooltip("Objects for Halloween!\n(Oct 25th to November 2nd)")]
    public GameObject[] Halloween;
    [Tooltip("Objects for XMas!\n(Dec 20th to 30th)")]
    public GameObject[] XMas;

    [Header("〚 Custom Holiday 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [Tooltip("Objects for a custom holiday!\n(Maybe a non-western one or an anniversary?)\nThis will spawn these objects for 2 days before and 2 days after.")]
    public GameObject[] CustomHoliday;
    [Tooltip("Day of the week.\nNumber of the day. (1-31)")]
    public int CustomDay;
    [Tooltip("Month of the year.\nNumber of the month. (1-12)")]
    public int CustomMonth;
    [Tooltip("What is your custom Holiday?\n(Displays on the ui for that timeframe)\nLeave blank if you don't want this.")]
    public string CustomName;

    // If you would want to add more than 1 custom holiday, or simply replace the current ones
    // All you would have to do is edit the responding names for those dates and change the dates they appear on.
    // Otherwise, these are pretty standard/western holidays.


    [Header("〚 Seasonal 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [Tooltip("Anything that should be active during Springtime\n(March, April, May)")]
    public GameObject[] Spring;
    [Tooltip("Anything that should be active during Summertime\n(June, July, August)")]
    public GameObject[] Summer;
    [Tooltip("Anything that should be active during Autumntime\n(September, October, November)")]
    public GameObject[] Autumn;
    [Tooltip("Anything that should be active during Wintertime\n(December, January, February)")]
    public GameObject[] Winter;

    [Header("〚 Displays 〛▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬")]
    [SerializeField] public TextMeshPro TodaysDate;


    // These are private variables used internally.
    private int CurrentDay;
    private int CurrentMonth;
    private string MonthName = "";
    private string HolidayName = "";

    [Space(12)]

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

    public void OnPlayerJoined()
    {
        if (!isDebugMode)
        {
            SetDate();
        }

    }


    private void Update()
    {
        if (RealtimeDecorations && !isDebugMode)
        {
            SetDate();
        }
    }

    public void SetDate()
    {
        CurrentDay = DateTime.Now.Day;
        CurrentMonth = DateTime.Now.Month;
        SetDecor();
    }


    public void SetDecor()
    {


        if (CurrentMonth == 1)
        {
            // Jan
            for (int i = 0; i < Winter.Length; i++)
            {
                Winter[i].SetActive(true);
            }
            for (int i = 0; i < Spring.Length; i++)
            {
                Spring[i].SetActive(false);
            }
            for (int i = 0; i < Summer.Length; i++)
            {
                Summer[i].SetActive(false);
            }
            for (int i = 0; i < Autumn.Length; i++)
            {
                Autumn[i].SetActive(false);
            }

            //Set the MonthName
            MonthName = "January";

            // Check for the holiday thats in this month. (New Years)
            if (CurrentDay < 3 && CurrentDay > 0) //1st to the 2nd - Yeah I know.
            {
                HolidayName = "Happy New Years!";
                for (int i = 0; i < NewYears.Length; i++)
                {
                    NewYears[i].SetActive(true);

                }
            }
            else
            {
                for (int i = 0; i < NewYears.Length; i++)
                {
                    NewYears[i].SetActive(false);
                }
            }
            // End of check for this month's holiday. (Wupee)

            // Check for custom holiday. Yay. More damn if statements....
            if (CurrentMonth == CustomMonth)
            {
                if (CurrentDay > CustomDay - 3 && CurrentDay < CustomDay + 3)
                {
                    HolidayName = (CustomName);
                    // Yay custom holiday woo
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(true);

                    }
                }
                else
                {
                    // Booo not custom holiday
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(false);
                    }
                }
            } 
            // End of Custom Holiday Check
        }
        else if (CurrentMonth == 2)
        {
            // Feb
            for (int i = 0; i < Winter.Length; i++)
            {
                Winter[i].SetActive(true);
            }
            for (int i = 0; i < Spring.Length; i++)
            {
                Spring[i].SetActive(false);
            }
            for (int i = 0; i < Summer.Length; i++)
            {
                Summer[i].SetActive(false);
            }
            for (int i = 0; i < Autumn.Length; i++)
            {
                Autumn[i].SetActive(false);
            }

            //Set the MonthName
            MonthName = "February";

            // Check for the holiday thats in this month. (VALENTINES DAY)
            if (CurrentDay < 18 && CurrentDay > 9) //10th to the 17th - Yeah I know.
            {
                HolidayName = "Happy Valentines Day!";
                for (int i = 0; i < Valentines.Length; i++)
                {
                    Valentines[i].SetActive(true);

                }
            }
            else
            {
                for (int i = 0; i < Valentines.Length; i++)
                {
                    Valentines[i].SetActive(false);
                }
            }
            // End of check for this month's holiday. (Wupee)

            // Check for custom holiday. Yay. More damn if statements....
            if (CurrentMonth == CustomMonth)
            {
                HolidayName = (CustomName);
                if (CurrentDay > CustomDay - 3 && CurrentDay < CustomDay + 3)
                {
                    // Yay custom holiday woo
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(true);

                    }
                }
                else
                {
                    // Booo not custom holiday
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(false);
                    }
                }
            }
            // End of Custom Holiday Check
        }
        else if (CurrentMonth == 3)
        {
            // Mar
            for (int i = 0; i < Spring.Length; i++)
            {
                Spring[i].SetActive(true);
            }
            for (int i = 0; i < Summer.Length; i++)
            {
                Summer[i].SetActive(false);
            }
            for (int i = 0; i < Autumn.Length; i++)
            {
                Autumn[i].SetActive(false);
            }
            for (int i = 0; i < Winter.Length; i++)
            {
                Winter[i].SetActive(false);
            }

            //Set the MonthName
            MonthName = "March";

            // Check for the holiday thats in this month. (STPATRICKS DAY)
            if (CurrentDay < 19 && CurrentDay > 14) //15th to the 18th - Yeah I know.
            {
                HolidayName = "Happy St. Patrick's Day!";
                for (int i = 0; i < StPatrick.Length; i++)
                {
                    StPatrick[i].SetActive(true);

                }
            }
            else
            {
                for (int i = 0; i < StPatrick.Length; i++)
                {
                    StPatrick[i].SetActive(false);
                }
            }
            // End of check for this month's holiday. (Wupee)


            // Check for custom holiday. Yay. More damn if statements....
            if (CurrentMonth == CustomMonth)
            {
                if (CurrentDay > CustomDay - 3 && CurrentDay < CustomDay + 3)
                {
                    HolidayName = (CustomName);
                    // Yay custom holiday woo
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(true);

                    }
                }
                else
                {
                    // Booo not custom holiday
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(false);
                    }
                }
            }
            // End of Custom Holiday Check
        }
        else if (CurrentMonth == 4)
        {
            // Apr
            for (int i = 0; i < Spring.Length; i++)
            {
                Spring[i].SetActive(true);
            }
            for (int i = 0; i < Summer.Length; i++)
            {
                Summer[i].SetActive(false);
            }
            for (int i = 0; i < Autumn.Length; i++)
            {
                Autumn[i].SetActive(false);
            }
            for (int i = 0; i < Winter.Length; i++)
            {
                Winter[i].SetActive(false);
            }

            //Set the MonthName
            MonthName = "April";

            // Check for the holiday thats in this month. (EASTER)
            if (CurrentDay < 12 && CurrentDay > 5) //6th to the 11th - Yeah I know.
            {
                HolidayName = "Happy Easter!";
                for (int i = 0; i < Easter.Length; i++)
                {
                    Easter[i].SetActive(true);

                }
            }
            else
            {
                for (int i = 0; i < Easter.Length; i++)
                {
                    Easter[i].SetActive(false);
                }
            }
            // End of check for this month's holiday. (Wupee)

            // Check for custom holiday. Yay. More damn if statements....
            if (CurrentMonth == CustomMonth)
            {
                if (CurrentDay > CustomDay - 3 && CurrentDay < CustomDay + 3)
                {
                    HolidayName = (CustomName);
                    // Yay custom holiday woo
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(true);

                    }
                }
                else
                {
                    // Booo not custom holiday
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(false);
                    }
                }
            }
            // End of Custom Holiday Check
        }
        else if (CurrentMonth == 5)
        {



            // May
            for (int i = 0; i < Spring.Length; i++)
            {
                Spring[i].SetActive(true);
            }
            for (int i = 0; i < Summer.Length; i++)
            {
                Summer[i].SetActive(false);
            }
            for (int i = 0; i < Autumn.Length; i++)
            {
                Autumn[i].SetActive(false);
            }
            for (int i = 0; i < Winter.Length; i++)
            {
                Winter[i].SetActive(false);
            }

            //Set the MonthName
            MonthName = "May";

            // Not including any custom months for may/june/july/aug/sep
            // so feel free to copy that same crap down here
            // if you want to use these months. :)

            // Check for custom holiday. Yay. More damn if statements....
            if (CurrentMonth == CustomMonth)
            {
                if (CurrentDay > CustomDay - 3 && CurrentDay < CustomDay + 3)
                {
                    HolidayName = (CustomName);
                    // Yay custom holiday woo
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(true);

                    }
                }
                else
                {
                    // Booo not custom holiday
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(false);
                    }
                }
            }
            // End of Custom Holiday Check
        }
        else if (CurrentMonth == 6)
        {
            // Jun
            for (int i = 0; i < Summer.Length; i++)
            {
                Summer[i].SetActive(true);
            }
            for (int i = 0; i < Spring.Length; i++)
            {
                Spring[i].SetActive(false);
            }
            for (int i = 0; i < Autumn.Length; i++)
            {
                Autumn[i].SetActive(false);
            }
            for (int i = 0; i < Winter.Length; i++)
            {
                Winter[i].SetActive(false);
            }

            //Set the MonthName
            MonthName = "June";


            // Check for custom holiday. Yay. More damn if statements....
            if (CurrentMonth == CustomMonth)
            {
                if (CurrentDay > CustomDay - 3 && CurrentDay < CustomDay + 3)
                {
                    HolidayName = (CustomName);
                    // Yay custom holiday woo
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(true);

                    }
                }
                else
                {
                    // Booo not custom holiday
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(false);
                    }
                }
            }
            // End of Custom Holiday Check
        }
        else if (CurrentMonth == 7)
        {
            // Jul
            for (int i = 0; i < Summer.Length; i++)
            {
                Summer[i].SetActive(true);
            }
            for (int i = 0; i < Spring.Length; i++)
            {
                Spring[i].SetActive(false);
            }
            for (int i = 0; i < Autumn.Length; i++)
            {
                Autumn[i].SetActive(false);
            }
            for (int i = 0; i < Winter.Length; i++)
            {
                Winter[i].SetActive(false);
            }

            //Set the MonthName
            MonthName = "July";

            // Check for custom holiday. Yay. More damn if statements....
            if (CurrentMonth == CustomMonth)
            {
                if (CurrentDay > CustomDay - 3 && CurrentDay < CustomDay + 3)
                {
                    HolidayName = (CustomName);
                    // Yay custom holiday woo
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(true);

                    }
                }
                else
                {
                    // Booo not custom holiday
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(false);
                    }
                }
            }
            // End of Custom Holiday Check
        }
        else if (CurrentMonth == 8)
        {
            // Aug
            for (int i = 0; i < Summer.Length; i++)
            {
                Summer[i].SetActive(true);
            }
            for (int i = 0; i < Spring.Length; i++)
            {
                Spring[i].SetActive(false);
            }
            for (int i = 0; i < Autumn.Length; i++)
            {
                Autumn[i].SetActive(false);
            }
            for (int i = 0; i < Winter.Length; i++)
            {
                Winter[i].SetActive(false);
            }

            //Set the MonthName
            MonthName = "August";

            // Check for custom holiday. Yay. More damn if statements....
            if (CurrentMonth == CustomMonth)
            {
                if (CurrentDay > CustomDay - 3 && CurrentDay < CustomDay + 3)
                {
                    HolidayName = (CustomName);
                    // Yay custom holiday woo
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(true);

                    }
                }
                else
                {
                    // Booo not custom holiday
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(false);
                    }
                }
            }
            // End of Custom Holiday Check
        }
        else if (CurrentMonth == 9)
        {
            // Sep
            for (int i = 0; i < Autumn.Length; i++)
            {
                Autumn[i].SetActive(true);
            }
            for (int i = 0; i < Spring.Length; i++)
            {
                Spring[i].SetActive(false);
            }
            for (int i = 0; i < Summer.Length; i++)
            {
                Summer[i].SetActive(false);
            }
            for (int i = 0; i < Winter.Length; i++)
            {
                Winter[i].SetActive(false);
            }

            //Set the MonthName
            MonthName = "September";

            // Check for custom holiday. Yay. More damn if statements....
            if (CurrentMonth == CustomMonth)
            {
                if (CurrentDay > CustomDay - 3 && CurrentDay < CustomDay + 3)
                {
                    HolidayName = (CustomName);
                    // Yay custom holiday woo
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(true);

                    }
                }
                else
                {
                    // Booo not custom holiday
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(false);
                    }
                }
            }
            // End of Custom Holiday Check
        }
        else if (CurrentMonth == 10)
        {
            // Oct
            for (int i = 0; i < Autumn.Length; i++)
            {
                Autumn[i].SetActive(true);
            }
            for (int i = 0; i < Spring.Length; i++)
            {
                Spring[i].SetActive(false);
            }
            for (int i = 0; i < Summer.Length; i++)
            {
                Summer[i].SetActive(false);
            }
            for (int i = 0; i < Winter.Length; i++)
            {
                Winter[i].SetActive(false);
            }

            //Set the MonthName
            MonthName = "October";

            // Check for the holiday thats in this month. (Halloween) SPOOKY MONTH LETS GO!!
            if (CurrentDay < 40 && CurrentDay > 24) //25th to the 2nd of November 
            // So theres not really a way to extend this across multiple (CurrentMonth)
            // without adding ANOTHER if statement so just setting this to 40 works. Nice lmao
            {
                HolidayName = "Happy Halloween!";
                for (int i = 0; i < Halloween.Length; i++)
                {
                    Halloween[i].SetActive(true);

                }
            }
            else
            {
                for (int i = 0; i < Halloween.Length; i++)
                {
                    Halloween[i].SetActive(false);
                }
            }
            // End of check for this month's holiday. (Wupee)

            // Check for custom holiday. Yay. More damn if statements....
            if (CurrentMonth == CustomMonth)
            {
                if (CurrentDay > CustomDay - 3 && CurrentDay < CustomDay + 3)
                {
                    HolidayName = (CustomName);
                    // Yay custom holiday woo
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(true);

                    }
                }
                else
                {
                    // Booo not custom holiday
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(false);
                    }
                }
            }
            // End of Custom Holiday Check
        }
        else if (CurrentMonth == 11)
        {
            // Nov
            for (int i = 0; i < Autumn.Length; i++)
            {
                Autumn[i].SetActive(true);
            }
            for (int i = 0; i < Spring.Length; i++)
            {
                Spring[i].SetActive(false);
            }
            for (int i = 0; i < Summer.Length; i++)
            {
                Summer[i].SetActive(false);
            }
            for (int i = 0; i < Winter.Length; i++)
            {
                Winter[i].SetActive(false);
            }

            //Set the MonthName
            MonthName = "November";

            // Continuing from October for Halloween until the 2nd. Check above for note.
            if (CurrentDay < 3 && CurrentDay > 0) // Using 0 here to include the 1st.
            {
                HolidayName = "Happy Halloween!";
                for (int i = 0; i < Halloween.Length; i++)
                {
                    Halloween[i].SetActive(true);

                }
            }
            else
            {
                for (int i = 0; i < Halloween.Length; i++)
                {
                    Halloween[i].SetActive(false);
                }
            }
            // End of check for this month's holiday. (Wupee)

            // Check for custom holiday. Yay. More damn if statements....
            if (CurrentMonth == CustomMonth)
            {
                if (CurrentDay > CustomDay - 3 && CurrentDay < CustomDay + 3)
                {
                    HolidayName = (CustomName);
                    // Yay custom holiday woo
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(true);

                    }
                }
                else
                {
                    // Booo not custom holiday
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(false);
                    }
                }
            }
            // End of Custom Holiday Check
        }
        else if (CurrentMonth == 12)
        {
            // Dec
            for (int i = 0; i < Winter.Length; i++)
            {
                Winter[i].SetActive(true);
            }
            for (int i = 0; i < Spring.Length; i++)
            {
                Spring[i].SetActive(false);
            }
            for (int i = 0; i < Summer.Length; i++)
            {
                Summer[i].SetActive(false);
            }
            for (int i = 0; i < Autumn.Length; i++)
            {
                Autumn[i].SetActive(false);
            }

            //Set the MonthName
            MonthName = "December";

            // Check for the holiday thats in this month. (XMas) 
            // ITS THE MOOOSTTT WONDERRRFULLLL TIIIIIMMMMEEE... OFFFF THE YEEEARRR
            if (CurrentDay < 30 && CurrentDay > 19) 
            {
                HolidayName = "Merry Christmas!";
                for (int i = 0; i < XMas.Length; i++)
                {
                    XMas[i].SetActive(true);

                }
            }
            else
            {
                for (int i = 0; i < XMas.Length; i++)
                {
                    XMas[i].SetActive(false);
                }
            }
            // End of check for this month's holiday.... Nope just kidding.
            // Remember we did new years? Yeah. Great, now I gotta add that.

            if (CurrentDay < 32 && CurrentDay > 29) // Check to see if its at least the 30th. Ignore 32. We don't talk about that. It works ok? Ok.
            {
                HolidayName = "Happy New Years Eve!";
                for (int i = 0; i < NewYears.Length; i++)
                {
                    NewYears[i].SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < NewYears.Length; i++)
                {
                    NewYears[i].SetActive(false);
                }
            }


            // Check for custom holiday. Yay. More damn if statements....
            if (CurrentMonth == CustomMonth)
            {
                if (CurrentDay > CustomDay - 3 && CurrentDay < CustomDay + 3)
                {
                    HolidayName = (CustomName);
                    // Yay custom holiday woo
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(true);
                    }
                }
                else
                {
                    // Booo not custom holiday
                    for (int i = 0; i < CustomHoliday.Length; i++)
                    {
                        CustomHoliday[i].SetActive(false);
                    }
                }
            }
            // End of Custom Holiday Check

        }
        else
        {
            // null
            // IM DONE...? HOLY CRAP IM DONE.

            // nope. forgot the displays.. heh..
        }

        //string getMonth = CurrentMonth.ToString();
        //string getDay = CurrentMonth.ToString();
        //string result = "Today's Date: " + getMonth + getDay;

        SetDisplays();

    }

    public void SetDisplays()
    {
        TodaysDate.text = ("Today's Date:\n" + MonthName + " " + CurrentDay + "\n" + HolidayName);
    }
    



}
