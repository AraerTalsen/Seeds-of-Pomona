using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldClock : MonoBehaviour
{
    private static readonly int lengthOfMinInSec = 1;
    private static readonly int lengthOfHrInSec = 10;
    public static readonly int lengthOfDayInSec = 10;//1200
    private static readonly int wakeUpTimeInSec = 360;//360
    private static readonly int sleepTimeInSec = 960;//960
    private static float startTime, lastRollover = 1;
    private static float cummTimeMod = 0;

    private static int currentDay = 0;
    public static int CurrentDay {get => currentDay; set => currentDay = value;}
    
    public static int TimeInSeconds
    {
        get
        {
            float currentTime = Time.time + cummTimeMod;
            float timeInSeconds = (currentTime - startTime) % lengthOfDayInSec;
            return (int)timeInSeconds;
        }
    }

    public static Vector2 StandardTime
    {
        get
        {
            int timePassed = TimeInSeconds;
            int hour = timePassed / lengthOfHrInSec;
            int min = timePassed % lengthOfHrInSec / lengthOfMinInSec;
            return new Vector2(hour, min);
        }
    }

    public static float TimeInMinutes
    {
        get
        {
            return TimeInSeconds / 60;
        }
    }

    public static float TimeInHours
    {
        get
        {
            return TimeInMinutes / 60;
        }
    }

    public static int WorldTimeInSeconds
    {
        get
        {
            return (int)Time.time;
        }
    }

    public static float WorldTimeInMinutes
    {
        get
        {
            return (int)(Time.time / 60);
        }
    }

    public static int WorldTimeSince(float savedTime)
    {
        float currentTime = Time.time;
        return (int)(currentTime - savedTime);
    }

    public static void StartNewDay()
    {
        if(TimeInSeconds > wakeUpTimeInSec)
        {
            CurrentDay++;
        }
        int ffTimeBySec = lengthOfDayInSec - TimeInSeconds + wakeUpTimeInSec;
        cummTimeMod += ffTimeBySec;
    }

    public static void RolloverClock()
    {
        if(WorldTimeSince(lastRollover) >= 1 && ((int)Time.time + cummTimeMod) % lengthOfDayInSec == 0)
        {
            CurrentDay++;
            lastRollover = Time.time;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }
}
