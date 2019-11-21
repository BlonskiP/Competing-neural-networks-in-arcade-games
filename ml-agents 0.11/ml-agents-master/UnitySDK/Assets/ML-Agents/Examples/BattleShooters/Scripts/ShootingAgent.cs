using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShootingAgent : Agent
{
    Rigidbody rig;
    AgentsArena arena;
    private RayPerception3D rayPer;
    public float turnSpeed = 300;
    public float moveSpeed = 2;
    const float rayDistance = 25;
    public float maxHealth = 100;
    public float health = 100;
    bool isReloading = false;
    float realoadTime=0;
    float maxRealodTIme = 5;
    private LineRenderer lineRender;
    public Vector3 startingPosition;
    public bool wasShoot = false;
    public bool destroy = false;
    public int maxAmmo = 10;
    public int ammo = 0;
    public int maxlineCounter = 100;
    int lineCounter = 0;
    bool isShooting = false;
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        
        base.AgentAction(vectorAction, textAction);
        if (transform.position.y < arena.transform.position.y-1)
        {
            Done();
        }
       

        if (wasShoot)
        {
            health -= 50f;
            wasShoot = false;
            AddReward(-0.15f); //panish for being shot
        }
        if (health <= 0) {
            AddReward(-0.2f);  //dying punish
            Debug.Log("Dies" + this.gameObject.name);
            Done();
        }
     //   else { AddReward(0.000025f); } //Survival reward
        
        if (Time.time >= realoadTime + maxRealodTIme && isReloading)
        {
            
            isReloading = false;
            this.gameObject.tag = "Agent";
        }
       
        isShooting = false;
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var forwardAxis = (int)vectorAction[0];
        var rightAxis = (int)vectorAction[1];
        var rotateAxis = (int)vectorAction[2];
        var shootAxis = (int)vectorAction[3];
        var shootCommand = false;

        switch (forwardAxis)
        {
            case 1:
                dirToGo = transform.forward;
                break;
            case 2:
                dirToGo = -transform.forward;
                break;
        }

        switch (rightAxis)
        {
            case 1:
                dirToGo = transform.right;
                break;
            case 2:
                dirToGo = -transform.right;
                break;
        }

        switch (rotateAxis)
        {
            case 1:
                rotateDir = -transform.up;
                break;
            case 2:
                rotateDir = transform.up;
                break;
        }
        switch (shootAxis)
        {
            case 1:
                shootCommand = true;
                break;
        }
        if (shootCommand)
        {
            isShooting = true;
            dirToGo *= 0.5f;
            rig.velocity *= 0.75f;
        }
        rig.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
        transform.Rotate(rotateDir, Time.fixedDeltaTime * turnSpeed);

        if (canShoot() && isShooting)
        {
            ammo -= 1;
            var myTransform = transform;
           // var position = myTransform.TransformDirection(RayPerception3D.PolarToCartesian(rayDistance, 90f));
            var position = transform.position + Vector3.Normalize(transform.forward) * rayDistance;
          
            RaycastHit hit;
            bool wasHit = Physics.SphereCast(transform.position, 2f, position, out hit, rayDistance);
            if (wasHit)
            {
                Debug.DrawRay(myTransform.position, hit.point, Color.red, 3f, true);
                var agent = hit.collider.gameObject.GetComponent<ShootingAgent>();
                if (agent != null)
                {
                    AddReward(1f);
                    agent.wasShoot = true;
                }
                else
                {
                    if (hit.collider.gameObject.CompareTag("Ragent") || hit.collider.gameObject.CompareTag("Agent"))
                    {
                        AddReward(1f);
                    }else
                    {
                     //   AddReward(-0.1f);
                    }
                }
                lineRender.SetPosition(1, hit.point);
                lineCounter = 0;
                maxlineCounter = 120;
            }else
            {
                lineRender.SetPosition(1, position);
                lineCounter = 0;
                maxlineCounter = 30;
            }
            needToRealoadLaser();

        }
    }
    public override void AgentOnDone()
    {
        base.AgentOnDone();
        if(destroy)
        {
            DestroyImmediate(this.gameObject);
        }
    }
    public override void CollectObservations()
    {
        base.CollectObservations();
        float[] rayAngles = { 20f, 90f, 160f, 45f, 135f, 70f, 110f };
        string[] detectableObjects = { "Agent" , "Wall" , "Ammo" , "HealthPack", "Ragent" };
        AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
        var localVelocity = transform.InverseTransformDirection(rig.velocity);
        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
        AddVectorObs(health / maxHealth);
        AddVectorObs(ammo / maxAmmo);
        AddVectorObs(canShoot());
        AddVectorObs(wasShoot);
    }
    public bool canShoot()
    {
        if(ammo>0 && !isReloading)
        {
            return true;
        }
        return false;
    }
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        arena = GetComponentInParent<AgentsArena>();
        rig = GetComponent<Rigidbody>();
        rayPer = GetComponent<RayPerception3D>();
        lineRender = GetComponent<LineRenderer>();
        SetResetParameters();
        startingPosition = transform.position;
    }
    public void needToRealoadLaser()
    {
        isReloading = true;
        realoadTime = Time.time;
        this.gameObject.tag = "Ragent";
    }
    private void SetResetParameters()
    {
        isShooting = false;
        wasShoot = false;
        health = 100;
        ammo = 0;
        lineRender.SetPosition(0, transform.position);
        lineRender.SetPosition(1, transform.position);
        maxlineCounter = 0;
        lineCounter = 0;
    }

    public override void AgentReset()
    {
        rig.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
        transform.position = startingPosition;
        SetResetParameters();
    }

    private void OnCollisionEnter(Collision collision)
    {
        PickUp pickUp = collision.gameObject.GetComponent<PickUp>();
        if ((pickUp) != null)
        {
            pickUp.pickUpEffect(this);
        }
    }

    private void FixedUpdate()
    {
        lineRender.SetPosition(0, transform.position);
        lineCounter++;
        if (maxlineCounter < lineCounter)
        {
            lineRender.SetPosition(1, transform.position);
            lineCounter = 0;
            maxlineCounter = 0;
        }
    }
}
