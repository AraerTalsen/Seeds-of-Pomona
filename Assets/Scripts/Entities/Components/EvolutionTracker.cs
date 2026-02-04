using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionTracker : MonoBehaviour
{
    [SerializeField] private List<EvolutionEntry> evolutions;
    public EvolutionContext Context;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            foreach(EvolutionEntry e in evolutions)
            {
                e.effect.Apply(Context, e.payload);
            }
        }
    }
}
