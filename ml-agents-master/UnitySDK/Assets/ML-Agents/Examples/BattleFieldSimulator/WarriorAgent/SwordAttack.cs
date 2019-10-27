using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    BoxCollider swordBoxCollider;
    public string TeamName;
    public SimpleWarriorAgent agent;
    int hitCounter = 0;
    private void Awake()
    {
        swordBoxCollider = GetComponent<BoxCollider>();
        GetComponent<Collider>().enabled = false;
        agent = GetComponentInParent<SimpleWarriorAgent>();
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
        //SimpleWarriorAgent target = collider.gameObject.GetComponent<SimpleWarriorAgent>();
        string enemyTag = "Team2";
        if (agent.tag == enemyTag)
            enemyTag = "Team1";
        if(collider.tag==enemyTag)
        {
            agent.SetReward(1);
            hitCounter++;

            //Debug.Log(Vector3.Distance(agent.transform.position, collider.transform.position).ToString());
            SimpleWarriorAgent target = collider.gameObject.GetComponent<SimpleWarriorAgent>();
            if (target != null)
            {
                if (target.TeamName != this.TeamName)
                    target.GetDmg(25);
                if (target.health < 0)
                {
                    agent.SetReward(2f);
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
