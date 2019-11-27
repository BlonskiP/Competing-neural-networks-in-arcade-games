using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickUp : PickUp
{
    public float healthRestore = 25;
    public override void pickUpEffect(ShootingAgent agent)
    {
        if (agent.health < agent.maxHealth-healthRestore)
            agent.AddReward(0.01f);
        agent.health += 25;
        if (agent.health > agent.maxHealth)
            agent.health = agent.maxHealth;
        Used();
    }
}
