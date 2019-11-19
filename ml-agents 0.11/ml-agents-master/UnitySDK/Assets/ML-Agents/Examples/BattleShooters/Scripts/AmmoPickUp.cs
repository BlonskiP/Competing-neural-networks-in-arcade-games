using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : PickUp
{
    public int ammoRestore = 15;
    public override void pickUpEffect(ShootingAgent agent)
    {
        if (agent.ammo < agent.maxAmmo- ammoRestore)
            agent.AddReward(0.2f);
        agent.ammo += ammoRestore;
        if (agent.ammo > agent.maxAmmo)
            agent.ammo = agent.maxAmmo;
        Used();
    }
}
