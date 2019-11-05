using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    BoxCollider swordBoxCollider;
    public ActionWarriorAgent agent;
    private void Awake()
    {
        swordBoxCollider = GetComponent<BoxCollider>();
        GetComponent<Collider>().enabled = false;
        agent = GetComponentInParent<ActionWarriorAgent>();
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
        if (collider.tag == agent.WarriorStats.team.EnemyTeamName)
        {
            agent.AddReward(1);
            //Debug.Log(Vector3.Distance(agent.transform.position, collider.transform.position).ToString());
            ActionWarriorAgent target = collider.gameObject.GetComponent<ActionWarriorAgent>();
            if (target != null)
            {
                if (target.takeDmg(100))
                {
                    agent.AddReward(1f);
                    if (target.WarriorStats.health < 0)
                    {
                        agent.AddReward(1f);
                    }

                }
            }
        }
        else
            if (collider.tag == agent.WarriorStats.team.TeamName || collider.tag == "ArenaWall")
        {
            agent.AddReward(-0.1f);
        }
    }

}
