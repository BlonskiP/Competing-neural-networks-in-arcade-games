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
    private float decreaseHealthRate = 2;
    bool isReloading = false;
    float realoadTime=0;
    float LaserDisplayTime = 0;
    float maxRealodTIme = 2;
    private LineRenderer lineRender;
    public Vector3 startingPosition;
    public bool wasShoot = false;
    public bool destroy = false;
    public int maxAmmo = 10;
    public int ammo = 0;
    bool isShooting = false;
    public bool isTreningMode = false;
    public override void AgentAction(float[] vectorAction, string textAction)
    {

        base.AgentAction(vectorAction, textAction);
        lineRender.SetPosition(0, transform.position);
        if (transform.position.y < arena.transform.position.y - 1)
        {
            Done();
        }
        if (Time.time >= LaserDisplayTime + 2)
        {
            lineRender.enabled = false;
            lineRender.SetPosition(1, transform.position);
        }

        if (wasShoot)
        {
            if(!isTreningMode)
                health -= 50f;
            wasShoot = false;
        }
        if (health <= 0) {
            Done();
        }
        
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

        if (rig.velocity.sqrMagnitude > 10f) // slow it down
        {
            rig.velocity *= 0.95f;
        }
        var laserDirection = transform.position + Vector3.Normalize(transform.forward) * rayDistance;
        RaycastHit hit;
        bool wasHit = Physics.Raycast(transform.position, laserDirection, out hit, rayDistance);
        bool wasThatAgent = false;
        if (wasHit)
        {
            var distance = Vector3.Distance(transform.position, hit.transform.position);
            wasThatAgent = hit.collider.gameObject.CompareTag("Ragent") || hit.collider.gameObject.CompareTag("Agent");
            if (wasThatAgent && canShoot() && distance > 2)
            {
                
                AddReward(0.005f); //reward for Aim
            }
            else if (hit.collider.gameObject.CompareTag("Wall"))
            {
                if ( distance < 1)
                {
                    AddReward(-0.001f);
                }
                
            }

        }
        if (canShoot() && isShooting)
        {
            ammo -= 1;  
            if (wasHit)
            {
                Debug.DrawRay(transform.position, hit.point, Color.red, 1f, true);

                if (wasThatAgent)
                {
                    AddReward(1f);
                    var agent = hit.collider.gameObject.GetComponent<ShootingAgent>();
                    if (agent != null)
                    {
                        agent.wasShoot = true;
                        Debug.Log(agent.gameObject.name + "got hit by:" + this.gameObject.name);
                        if (agent.health <= 50)
                        {
                            
                            Debug.Log(agent.gameObject.name + "Will be killed by:" + this.gameObject.name);
                        }
                    }  
                }
                lineRender.SetPosition(1, hit.point);
                lineRender.enabled = true;
            }
            else
            {
                lineRender.enabled = true;
                lineRender.SetPosition(1, laserDirection);
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
        LaserDisplayTime = Time.time;
        this.gameObject.tag = "Ragent";
    }
    private void SetResetParameters()
    {
        isShooting = false;
        wasShoot = false;
        health = 100;
        ammo = 0;
        lineRender.enabled = false;
        lineRender.SetPosition(0, transform.position);
        lineRender.SetPosition(1, transform.position);
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
        this.health -= decreaseHealthRate * Time.deltaTime;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position,Vector3.Normalize(transform.forward)*rayDistance);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, rayDistance);
    }
}
