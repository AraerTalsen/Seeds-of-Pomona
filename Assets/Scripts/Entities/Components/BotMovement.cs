using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotMovement : MonoBehaviour
{
    [SerializeField] private Vector2 destination;

    private Vector2 cachedDestination;
    private NavMeshAgent agent;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(cachedDestination, destination) > 0.1f)
        {
            agent.SetDestination(destination);
            cachedDestination = destination;
        }  
    }
}