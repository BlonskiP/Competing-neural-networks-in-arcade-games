using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShootingAgent : Agent
{
    Rigidbody rig;
    private RayPerception3D rayPer;
    public float turnSpeed = 300;
    public float moveSpeed = 2;
    const float rayDistance = 100;
    public float maxHealth = 100;
    public float health = 100;
    bool isReloading = false;
    float realoadTime=0;
    private LineRenderer lineRender;
    public Vector3 startingPosition;
    public bool wasShoot = false;
    public int maxAmmo = 50;
    public int ammo = 50;
    bool isShooting = false;
    public GameObject laserGameObj;
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        base.AgentAction(vectorAction, textAction);
        if (transform.position.y < -5)
        {
            Done();
        }
        lineRender.SetPosition(0, transform.position);

        if (wasShoot)
        {
            health -= 50f;
            wasShoot = false;
            AddReward(-0.05f); //panish for being shot
        }
        if (health <= 0) {
            AddReward(-0.5f);  //dying punish
            Done(); }
        else { AddReward(0.01f); } //Survival reward
        if (Time.time >= realoadTime + 10f && isReloading)
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

        if (!isReloading && isShooting && ammo>0)
        {
            ammo -= 1;
            var myTransform = transform;
            const int laserLenght = 50;
            laserGameObj.transform.localScale = new Vector3(1f, 1f, laserLenght);
            var position = myTransform.TransformDirection(RayPerception3D.PolarToCartesian(25f, 90f));
            Debug.DrawRay(myTransform.position, position, Color.red, 0f, true);

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, 3f, position, out hit, laserLenght))
            {
                var agent = hit.collider.gameObject.GetComponent<ShootingAgent>();
                if (agent!=null)
                {
                    AddReward(1f);
                    agent.wasShoot = true;
                }
                else
                {
                    AddReward(-0.005f); //punish wasting ammo
                }
                
            }
            lineRender.SetPosition(1, hit.point);
            needToRealoadLaser();

        }
        else
        {
            laserGameObj.transform.localScale = new Vector3(0f, 0f, 0f);
            lineRender.SetPosition(1, transform.position);
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
        AddVectorObs(health / 200);
        AddVectorObs(ammo / 50);
        AddVectorObs(isReloading);
    }
    public override void InitializeAgent()
    {
        base.InitializeAgent();
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
        ammo = 5;
    }

    public override void AgentReset()
    {
        rig.velocity = Vector3.zero;
        laserGameObj.transform.localScale = new Vector3(0f, 0f, 0f);
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
}
