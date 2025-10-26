using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldClock : MonoBehaviour
{
    public static int lengthOfDayInSec = 60;
    private static float startTime, currentTime;
    
    public static int TimeInSeconds
    {
        get
        {
            currentTime = Time.time;
            float timeInSeconds = (currentTime - startTime) % lengthOfDayInSec;
            return (int)timeInSeconds;
        }
    }

    public static float TimeInMinutes
    {
        get
        {
            return TimeInSeconds / 60;
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
    
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        currentTime = startTime;
    }    
}
