using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerAgent : Agent
{
    Rigidbody rigdbody;
    public Transform Target;
    public Transform Enemy;
    public float speed = 10;
    public bool isEnemy = false;
    private int reward = 1;
    void Start()
    {
        rigdbody = GetComponent<Rigidbody>();
    }

    public override void AgentReset()
    {
        if(transform.position.y<0)
        {//What happends if agent fall down
            this.rigdbody.angularVelocity = Vector3.zero;
            this.rigdbody.velocity = Vector3.zero;
            this.transform.position = new Vector3(0, 0.5f, 0);
        }
        
    }

    public override void CollectObservations() // Collect data about a world <3
    {
        AddVectorObs(Target.position);// Agent knows where his target is.
        AddVectorObs(this.transform.position);// Agent knows where he is.
        AddVectorObs(rigdbody.velocity.x);//Agent knows how fast he is moving on x-axis
        AddVectorObs(rigdbody.velocity.z);//Agent know how fast he is moving on z-axis
        AddVectorObs(Enemy.position);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        if (isEnemy) reward = -1;
        //Action size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = vectorAction[0];
        controlSignal.z = vectorAction[1];

        this.rigdbody.AddForce(controlSignal * speed);

        //Rewards
        float distanceToTarget = Vector3.Distance(this.transform.position, Target.position);
        float enemyDistanceToTarget = Vector3.Distance(this.Enemy.position, Target.position);
        if(distanceToTarget<1.42f)
        {
            SetReward(reward * 1.0f);
            this.Target.position =  new Vector3(Random.value * 8 - 4, 0.5f, Random.value * 8 - 4);
            Done();
        }
        if(this.transform.position.y < 0)
        {
            SetReward(reward * -1.5f);
            Done();
        }
    }

}
