using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : PickUp
{
    public override void pickUpEffect(ShootingAgent agent)
    {
        agent.health += 50;
        if (agent.health > 200)
            agent.health = 200;
        Used();
    }
}
