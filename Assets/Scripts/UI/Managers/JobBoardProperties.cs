using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobBoardProperties
{
    public List<int> FulfilledRequests { get; set; }
    public Dictionary<int, int> CurrentPlayerInv { get; set; } = new(); 
    
}
