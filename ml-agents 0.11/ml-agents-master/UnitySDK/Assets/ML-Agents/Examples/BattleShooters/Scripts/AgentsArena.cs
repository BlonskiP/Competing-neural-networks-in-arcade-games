﻿using MLAgents;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AgentsArena : Area
{
    public GameObject healthPickUp;
    public GameObject ammoPickUp;
    public GameObject smallWall;
    public GameObject target;
    public List<GameObject> walls;
    public List<GameObject> targets;
    public List<GameObject> healthPickUps;
    public List<GameObject> ammoPickUps;
    public bool pickUpsRespawn = true;
    public float range;
    public AgentsArena()
    {
    }

  
    public void ResetArena(ShootingAgent[] agents)
    {
        var academy = FindObjectOfType<ShootingAcad>();

        DestoryObjects(walls.ToArray());
        DestoryObjects(targets.ToArray());
        DestoryObjects(healthPickUps.ToArray());
        DestoryObjects(ammoPickUps.ToArray());

        ammoPickUps = new List<GameObject>();
        healthPickUps = new List<GameObject>();
        targets = new List<GameObject>();
        walls = new List<GameObject>();
        foreach (ShootingAgent agent in agents)
        {
            if (agent.transform.parent.parent == gameObject.transform)
            {
                agent.transform.position = new Vector3(Random.Range(-range, range), 2f,
                    Random.Range(-range, range))
                    + transform.position;
                agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
                var shooter = agent.GetComponent<ShootingAgent>();
                if(shooter != null)
                {
                    shooter.startingPosition = agent.transform.position;
                }
            }
        }

        for(int i=0; i< academy.walls; i++)
        {
            GameObject f = Instantiate(smallWall, new Vector3(Random.Range(-range, range), 1f,
                Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            walls.Add(f);
        }
        for (int i = 0; i < academy.traingTargets; i++)
        {
            int tagNum = Random.Range(0, 2);
            GameObject f = Instantiate(target, new Vector3(Random.Range(-range, range), 1f,
                Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            if (tagNum == 1) 
                f.tag = "Ragent";
            else f.tag = "Agent";
            targets.Add(f);
        }
        if(academy!=null)
        {   
            InitPickUp(academy.ammoPickUpsOnArena, ammoPickUp);
            InitPickUp(academy.healthPickUpsOnArena, healthPickUp);
        }
       
    }
    void DestoryObjects(GameObject[] objects)
    {
        foreach(var obj in objects)
        {
            Destroy(obj);
        }

    }
    void InitPickUp(int num, GameObject type)
    {
        for (int i = 0; i < num; i++)
        {
            
            var newPos = new Vector3(Random.Range(-range, range), 1f,
                Random.Range(-range, range)) + transform.position;
            while (Physics.CheckSphere(newPos,1))
            {
                newPos = new Vector3(Random.Range(-range, range), 2f,
                Random.Range(-range, range)) + transform.position;
            }
                GameObject f = Instantiate(type, newPos ,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            f.GetComponent<PickUp>().respawn = pickUpsRespawn;
            f.GetComponent<PickUp>().myArea = this;
        }
    }
}
