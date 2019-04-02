using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostControl : MonoBehaviour
{
    public GameObject target;
    UnityEngine.AI.NavMeshAgent agent;


    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Pacman");
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = target.transform.position;
        
    }
}
