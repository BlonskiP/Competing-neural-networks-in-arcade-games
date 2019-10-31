using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    BoxCollider swordBoxCollider;
    public ActionWarriorAgent agent;
    int hitCounter = 0;
    private void Awake()
    {
        swordBoxCollider = GetComponent<BoxCollider>();
        GetComponent<Collider>().enabled = false;
        agent = GetComponentInParent<ActionWarriorAgent>();
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
        if(collider.tag==agent.team.EnemyTeamName)
        {
            agent.AddReward(1);
            //Debug.Log(Vector3.Distance(agent.transform.position, collider.transform.position).ToString());
            ActionWarriorAgent target = collider.gameObject.GetComponent<ActionWarriorAgent>();
            if (target != null)
            {
                    if (target.takeDmg(25))
                    {
                        agent.AddReward(1f);
                        if (target.health < 0)
                            agent.AddReward(1f);
                    }
            }
        }
    }

}
