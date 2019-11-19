using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentsArena : Area
{
    public GameObject healthPickUp;
    public GameObject ammoPickUp;
    public GameObject smallWall;
    public List<GameObject> walls;
    public int wallCount = 10;
    public bool pickUpsRespawn = true;
    public float range;
    public AgentsArena()
    {
    }

    void InitPickUp(int num, GameObject type)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject f = Instantiate(type, new Vector3(Random.Range(-range, range), 1f,
                Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            f.GetComponent<PickUp>().respawn = pickUpsRespawn;
            f.GetComponent<PickUp>().myArea = this;
        }
    }
    public void ResetArena(ShootingAgent[] agents)
    {
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
        for(int i=0; i< wallCount; i++)
        {
            GameObject f = Instantiate(smallWall, new Vector3(Random.Range(-range, range), 1f,
                Random.Range(-range, range)) + transform.position,
                Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            walls.Add(f);
        }
        var acad = FindObjectOfType<ShootingAcad>();
        if(acad!=null)
        {   
            InitPickUp(acad.ammoPickUpsOnArena, ammoPickUp);
            InitPickUp(acad.healthPickUpsOnArena, healthPickUp);
        }
       
    }

}
