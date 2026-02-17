using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalEffect : MonoBehaviour
{
    public void StartDecay(int lifespan)
    {
        StartCoroutine(nameof(Decay), lifespan);
    }
    
    private IEnumerator Decay(int lifespan)
    {
        yield return new WaitForSeconds(lifespan);
        Destroy(this);
    }
}
