using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    BoxCollider swordBoxCollider;
    public WarriorAgent agent;
    int hitCounter = 0;
    private void Awake()
    {
        swordBoxCollider = GetComponent<BoxCollider>();
        GetComponent<Collider>().enabled = false;
        agent = GetComponentInParent<WarriorAgent>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableCollision() // Animation Event
    {
        GetComponent<Collider>().enabled = true;
    }
    public void DisableCollision() // Animation Event
    {
        GetComponent<Collider>().enabled = false;
    }
    private void OnTriggerEnter(Collider collider)
    {
        if(collider.tag==agent.agentTeam.EnemyTeamName)
        {
            agent.AddReward(1);
            hitCounter++;

            //Debug.Log(Vector3.Distance(agent.transform.position, collider.transform.position).ToString());
            WarriorAgent target = collider.gameObject.GetComponent<WarriorAgent>();
            if (target != null)
            {
                if (target.agentTeam.EnemyTeamName != agent.agentTeam.TeamName)
                    target.GetDmg(25);
                if (target.health < 0)
                {
                    agent.AddReward(2f);
                }
            }
            if(agent.isLearning)
            {
                if (hitCounter > 4)
                {
                    agent.Done();
                    hitCounter = 0;
                }
                
            }
            
        }
      //  Debug.Log("Attack check");
    }

}
