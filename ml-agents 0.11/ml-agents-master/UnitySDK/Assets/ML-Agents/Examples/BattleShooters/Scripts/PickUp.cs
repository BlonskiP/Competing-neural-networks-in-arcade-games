using System.Collections;
using System.Collections.Generic;
using UnityEngine;
abstract public class PickUp : MonoBehaviour
{
    public AgentsArena myArea;
    public bool respawn = true;
    abstract public void pickUpEffect(ShootingAgent agent);
    public void Used()
    {
        if (respawn)
        {
            transform.position = new Vector3(Random.Range(-myArea.range, myArea.range),
                3f,
                Random.Range(-myArea.range, myArea.range)) + myArea.transform.position;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
