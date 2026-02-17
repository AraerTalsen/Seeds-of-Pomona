using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Smoke Screen")]
public class SmokeScreen : Tool
{
    [SerializeField] private int lifespan;
    [SerializeField] private GameObject smokeCloudAOE;

    public override void UseAbility(GameObject user)
    {
        /*GameObject g = Instantiate(smokeCloudAOE, user.transform.position, Quaternion.identity);
        EnvironmentalEffect effect = g.GetComponent<EnvironmentalEffect>();
        effect.StartDecay(lifespan);*/
        Debug.Log("Smoke Screen");
    }
}
