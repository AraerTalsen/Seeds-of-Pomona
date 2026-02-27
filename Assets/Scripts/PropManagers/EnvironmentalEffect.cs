using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalEffect : MonoBehaviour
{
    public void StartDecay(float lifespan)
    {
        StartCoroutine(nameof(Decay), lifespan);
    }
    
    private IEnumerator Decay(float lifespan)
    {
        yield return new WaitForSeconds(lifespan);
        Destroy(gameObject);
    }
}
