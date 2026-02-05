using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyScaler : MonoBehaviour
{
    [SerializeField]
    private Slider difficultyScale;
    [SerializeField]
    private int maxTimeDifficulty;
    [SerializeField]
    private int maxDistDifficulty;
    [SerializeField]
    private int baseAggroCoolDown;
    public int TimesSpotted {
    get => timesSpotted; 
    set
        {
            timesSpotted = value;
            if(isAggroed)
            {
                StopCoroutine(nameof(AggroTimer));
            }
            StartCoroutine(nameof(AggroTimer));
        }
    }
    public Vector2 startPos;
    private float startTime;
    private int timesSpotted;
    private bool isAggroed = false;
    private StageDifficulty stageDifficulty;
    private int currThreshold = 0;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        startTime = Time.time;
        stageDifficulty = StageDifficulty.Instance;
        stageDifficulty.Subscribe(this);
    }

    // Update is called once per frame
    void Update()
    {
        float difficulty = CalculateDifficultyScale();
        GetCurrentDiffThreshold(difficulty);
        difficultyScale.value = difficulty / 3;
    }

    private float DistanceFromBase()
    {
        return Vector2.Distance(transform.position, startPos);
    }

    private float TimeInWilderness()
    {
        return WorldClock.WorldTimeSince(startTime);
    }

    private float CalculateDifficultyScale()
    {
        float timeDifficulty = TimeInWilderness() / maxTimeDifficulty;
        float distDifficulty = DistanceFromBase() / maxDistDifficulty;
        float aggroDifficulty = TimesSpotted * 0.5f;
        
        return Mathf.Min(timeDifficulty + distDifficulty + aggroDifficulty, 3);
    }

    private IEnumerator AggroTimer()
    {
        isAggroed = true;
        yield return new WaitForSeconds(baseAggroCoolDown);
        timesSpotted--;
        if(TimesSpotted > 0)
        {
            StartCoroutine(nameof(AggroTimer));
        }
        else
        {
            isAggroed = false;
        }
    }

    private void GetCurrentDiffThreshold(float difficulty)
    {
        int diffValue = Mathf.FloorToInt(difficulty);
        
        if(diffValue != currThreshold)
        {
            currThreshold = diffValue;
            stageDifficulty.NoticeOfDiffChange(currThreshold);
        }
    }

    private void OnDisable()
    {
        stageDifficulty.Unsubscribe(this);
    }
}
