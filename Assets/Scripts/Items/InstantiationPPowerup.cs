using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Instantitation Persistent Powerup")]
public class InstantiationPPowerup : Tool
{
    [SerializeField] private int lifespan;
    [SerializeField] private GameObject node;
    public int Lifespan => lifespan;
    public GameObject Node => node;
    public override void UseAbility(GameObject user)
    {
        GameObject g = Instantiate(Node, user.transform.position, Quaternion.identity);
        g.GetComponent<EnvironmentalEffect>().StartDecay(Lifespan); 
    }
}
