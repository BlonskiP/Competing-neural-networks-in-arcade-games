﻿using MLAgents;
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
    public int setpsToReset = 30000;
    public override void AcademyReset()
    {
        
        agents = FindObjectsOfType<ShootingAgent>();
        arenas = FindObjectsOfType<AgentsArena>();
        foreach (var arena in arenas)
        {
            arena.ResetArena(agents);
        }
        
    }
    
    public override void AcademyStep()
    {
        base.AcademyStep();
    }
}
