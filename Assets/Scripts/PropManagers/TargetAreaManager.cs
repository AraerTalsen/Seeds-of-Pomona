using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TargetAreaManager : MonoBehaviour
{
    public List<GameObject> Targets { get; } = new();

    public async Task<List<GameObject>> RetrieveValidTargets()
    {
        float elapsed = 0;
        while(elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            await Task.Yield();
        }
        
        return Targets;
    }
}
