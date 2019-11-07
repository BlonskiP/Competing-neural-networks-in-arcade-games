using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour
{
    BoxCollider collider;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider>();
    }
    private void OnTriggerExit(Collider other)
    {
        Agent agent = other.gameObject.GetComponent<Agent>();
        if(agent !=null)
        {
            agent.AddReward(-0.5f);
            agent.AgentReset();       
        }
    }
}
