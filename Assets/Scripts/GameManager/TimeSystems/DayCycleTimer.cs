using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DayCycleTimer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI clock;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ClockTick();
        WorldClock.RolloverClock();
        if(Input.GetKeyUp(KeyCode.V))
        {
            WorldClock.StartNewDay();
        }
    }

    private void ClockTick()
    {
        Vector2 time = WorldClock.StandardTime;
        string hr = time.x < 10 ? $"0{time.x}" : time.x.ToString();
        string min = time.y < 10 ? $"0{time.y}" : time.y.ToString();
        clock.text = $"{hr}:{min}\nDay: {WorldClock.CurrentDay}";
    }
}
