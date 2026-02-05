using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDifficulty : MonoBehaviour
{
    public static StageDifficulty Instance {get; private set;}
    private List<EvolutionTracker> evoSubs = new();
    private List<DifficultyScaler> diffSubs = new();
    private int currentDifficulty;
    public int CurrentDifficulty
    {
        get => currentDifficulty;
        set
        {
            bool increasd = currentDifficulty < value;
            currentDifficulty = value;
        
            foreach(EvolutionTracker e in evoSubs)
            {
                if(increasd)
                {
                    e.NextEvolution(currentDifficulty);
                }
                else
                {
                    e.PrevEvolution(currentDifficulty);
                }
            }
        }
    }

    private void Awake()
    {
        TrySetInstance();
    }

    private void TrySetInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void Subscribe(EvolutionTracker e)
    {
        evoSubs.Add(e);
    }

    public void Subscribe(DifficultyScaler d)
    {
        diffSubs.Add(d);
    }

    public void Unsubscribe(EvolutionTracker e)
    {
        evoSubs.Remove(e);
    }

    public void Unsubscribe(DifficultyScaler d)
    {
        diffSubs.Remove(d);
    }

    public void NoticeOfDiffChange(int difficultyLvl)
    {
        if(difficultyLvl != CurrentDifficulty)
        {
            CurrentDifficulty = difficultyLvl;
        }
    }
}
