using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    BoxCollider swordBoxCollider;
    public string TeamName;
    private void Awake()
    {
        swordBoxCollider = GetComponent<BoxCollider>();
        GetComponent<Collider>().enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableCollision() // Animation Event
    {
        GetComponent<Collider>().enabled = true;
    }
    public void DisableCollision() // Animation Event
    {
        GetComponent<Collider>().enabled = false;
    }
    private void OnTriggerEnter(Collider collider)
    {
        SimpleWarriorAgent target = collider.gameObject.GetComponent<SimpleWarriorAgent>();
        if (target != null)
        {
            if (target.TeamName != this.TeamName)
                target.GetDmg(25);
        }
        Debug.Log("Attack check");
    }

}
