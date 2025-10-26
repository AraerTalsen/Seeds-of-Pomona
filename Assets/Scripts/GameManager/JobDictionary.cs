using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class JobDictionary
{
    public static Dictionary<int, JobRequest> jobs = new Dictionary<int, JobRequest>()
    {
        {0, Resources.Load("ScriptableObjects/Jobs/Seeds") as JobRequest},
        {1, Resources.Load("ScriptableObjects/Jobs/Flower") as JobRequest},
        {2, Resources.Load("ScriptableObjects/Jobs/Bio Mass") as JobRequest}
    };
}
