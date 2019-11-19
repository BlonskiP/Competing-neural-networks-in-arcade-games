using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingAcad : Academy
{
    public ShootingAgent[] agents;
    public AgentsArena[] arenas;
    public GameObject[] targets;
    public int ammoPickUpsOnArena = 10;
    public int healthPickUpsOnArena = 10;
    public int walls = 10;
    public int traingTargets = 3;
    private int counter = 0;
    public int setpsToReset = 30000;
    public override void AcademyReset()
    {
        counter = 0;
        ClearObjects(GameObject.FindGameObjectsWithTag("Ammo"));
        ClearObjects(GameObject.FindGameObjectsWithTag("HealthPack"));
        agents = FindObjectsOfType<ShootingAgent>();
        arenas = FindObjectsOfType<AgentsArena>();
        foreach (var arena in arenas)
        {
            foreach(var wall in arena.walls)
            {
                Destroy(wall);
            }
            foreach (var target in arena.targets)
            {
                Destroy(target);
            }
            arena.wallCount = walls;
            arena.ResetArena(agents);
        }
        
    }
    void ClearObjects(GameObject[] objects)
    {
        foreach (var pickUps in objects)
        {
            Destroy(pickUps);
        }
    }

    public override void AcademyStep()
    {
        base.AcademyStep();
        counter++;
        if(counter>setpsToReset)
        {
            AcademyReset();
        }
    }
}
