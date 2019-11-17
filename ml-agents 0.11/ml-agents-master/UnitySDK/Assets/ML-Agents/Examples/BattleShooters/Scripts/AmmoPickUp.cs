using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickUp : PickUp
{
    public override void pickUpEffect(ShootingAgent agent)
    {
        agent.ammo += 5;
        Used();
    }
}
