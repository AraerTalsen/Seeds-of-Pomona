using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionTracker : MonoBehaviour
{
    [SerializeField] private List<EvolutionEntry> evolutions;
    public EvolutionContext Context;
    [SerializeField] private int currentEvolution = 0;

    private void Start()
    {
        StageDifficulty.Instance.Subscribe(this);
    }

    public void NextEvolution(int difficultyLvl)
    {
        if(TryChangeEvolution(difficultyLvl, forward: true, out int stage))
            ApplyEvolution(stage);
    }

    public void PrevEvolution(int difficultyLvl)
    {
        if(TryChangeEvolution(difficultyLvl, forward: false, out int stage))
            RevertEvolution(stage);
    }

    private bool TryChangeEvolution(int difficultyLvl, bool forward, out int stage)
    {
        stage = -1;
        int nextEvo = currentEvolution;
        int index = forward ? 0 : evolutions.Count - 1;

        while (index >= 0 && index < evolutions.Count)
        {
            int threshold = evolutions[index].threshold;

            if (forward)
            {
                if (threshold <= difficultyLvl && nextEvo < threshold)
                    nextEvo = threshold;
                else if(threshold > difficultyLvl)
                    break;
            }
            else
            {
                if (threshold > difficultyLvl)
                    nextEvo = threshold;
                else if(threshold <= difficultyLvl)
                {
                    nextEvo = threshold;
                    break;   
                }
            }

            index += forward ? 1 : -1;
        }
        
        if(difficultyLvl == 0)
            nextEvo = 0;

        if(nextEvo == currentEvolution)
        {
            currentEvolution = nextEvo;
            return false;
        }

        stage = index + (forward ? -1 : 1);
        currentEvolution = nextEvo;
        return true;
    }


    private void ApplyEvolution(int stage)
    {
        EvolutionEntry entry = evolutions[stage];
        entry.effect.Apply(Context, entry.payload);
    }

    private void RevertEvolution(int stage)
    {
        EvolutionEntry entry = evolutions[stage];
        entry.effect.Revert(Context, entry.payload);
    }

    private void OnDisable()
    {
        StageDifficulty.Instance.Unsubscribe(this);
    }
}
