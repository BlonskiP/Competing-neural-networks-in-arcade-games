using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingAcad : Academy
{
    public GameObject[] agents;
    public AgentsArena[] arenas;
    public int ammoPickUpsOnArena = 10;
    public int healthPickUpsOnArena = 10;
    public int walls = 10;
    public override void AcademyReset()
    {
        ClearObjects(GameObject.FindGameObjectsWithTag("Ammo"));
        ClearObjects(GameObject.FindGameObjectsWithTag("HealthPack"));

        agents = GameObject.FindGameObjectsWithTag("Agent");
        arenas = FindObjectsOfType<AgentsArena>();
        foreach (var arena in arenas)
        {
            
            foreach(var wall in arena.walls)
            {
                Destroy(wall);
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
}
