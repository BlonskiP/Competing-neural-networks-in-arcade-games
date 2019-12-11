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
            var newPos = new Vector3(Random.Range(-myArea.range, myArea.range),
                3f,
                Random.Range(-myArea.range, myArea.range)) + myArea.transform.position;

            while (Physics.CheckSphere(newPos, 1))
            {
                newPos = new Vector3(Random.Range(-myArea.range, myArea.range),
                3f,
                Random.Range(-myArea.range, myArea.range)) + myArea.transform.position;
            }
            transform.position = newPos;
            var rig = GetComponent<Rigidbody>();
            if(rig!=null)
            {
                rig.velocity = Vector3.zero;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
