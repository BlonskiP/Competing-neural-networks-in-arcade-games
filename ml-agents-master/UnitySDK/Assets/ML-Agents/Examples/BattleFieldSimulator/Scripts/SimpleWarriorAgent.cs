using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWarriorAgent : Agent
{
    public enum ActionState { Dead = -1, Idle = 0, Attacking = 1, Blocking = 2, Charge = 3, Walk = 4 };
    public ActionState actualAction;
    Animator anim;
    private Rigidbody rig;
    public SwordAttack weapon;
    public string TeamName;
    private string enemyTeam;
    public float stamina;
    public float health;
    public float viewDistance = 10f;
    private float staminaRegeneration = 1;
    private float blockStaminaCost = 2;
    bool canChangeAction = true;
    public bool isLearning = true;
    int moveSpeed = 100;
    private Vector3 startingPosition;
    string[] detectableObjects = { "Team1", "Team2", "ArenaWall" };
    RayPerception ray;
    float[] rayAngles = { 45f, 90f, 135f, 110f, 70f };
    public override void InitializeAgent()
    {

        anim = GetComponent<Animator>();
        actualAction = ActionState.Idle;
        weapon = GetComponentInChildren<SwordAttack>();
        weapon.TeamName = this.TeamName;
        rig = GetComponent<Rigidbody>();
        ray = GetComponent<RayPerception>();
        gameObject.tag = TeamName;
        if (TeamName == detectableObjects[0])
        {
            AcademyBattleField.Team1.Add(this);
            AcademyBattleField.team1Alive++;
            enemyTeam = detectableObjects[1];
        }
        else if (TeamName == detectableObjects[1])
        {
            AcademyBattleField.Team2.Add(this);
            AcademyBattleField.team2Alive++;
            enemyTeam = detectableObjects[0];
        }
        if (startingPosition == Vector3.zero)
            startingPosition = transform.position;
        else
            transform.position = startingPosition;
    }

    public override void CollectObservations()
    {

        int teamNumber = 1;
        int enemyNumber = 2;
        if (TeamName == "Team2") { teamNumber = 2; enemyNumber = 1; }
        AddVectorObs(teamNumber);
        AddVectorObs(enemyNumber);
        AddVectorObs(health);
        AddVectorObs(stamina);
        AddVectorObs(viewDistance);
        AddVectorObs(transform.position);
        AddVectorObs(transform.rotation);
        AddVectorObs((int)actualAction);
        AddVectorObs(Convert.ToInt32(canChangeAction));
        AddVectorObs(ray.Perceive(viewDistance, rayAngles, detectableObjects, 0f, 0f)); //Sight
        AddVectorObs(AcademyBattleField.getTeamCount(TeamName)); //Number of enemies alive
        AddVectorObs(AcademyBattleField.getTeamCount(enemyTeam)); //Number of ally alive
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Discrete Vector
        //1 move if can 0-don't move, 1-forward, 2-backward, 
        //2 rotate 1 right 2 left
        //3 Change action
        //   SetReward(-0.005f);
        int moveForward = Mathf.FloorToInt(vectorAction[0]);
        int rotation = Mathf.FloorToInt(vectorAction[1]);
        int newAction = Mathf.FloorToInt(vectorAction[2]);
        checkIfCanSeeEnemy(); //reward if can see enemy
        if (!isLearning)
            Debug.Log(vectorAction[0] + " " + vectorAction[1] + ' ' + vectorAction[2]);

        int rotateDir = 0;
        if (canChangeAction && actualAction != ActionState.Dead)
            switch (newAction)
            {
                case 0: { //Start idle
                        actualAction = ActionState.Idle;
                        break; }
                case 1: {//move
                        actualAction = ActionState.Walk;
                        move(moveForward);
                        break; }
                case 2: { //Attack
                        actualAction = ActionState.Attacking;
                        Attack();
                        break; }
                case 3: { //Block incoming attacks
                        Block();
                        break; }
                case 4: {//Charge!!
                        Charge();
                        break; }
            }
        else
        {
            switch ((int)actualAction)
            {
                case 3: { Charge(); break; }
            }
        }
        if (actualAction != ActionState.Charge && actualAction != ActionState.Attacking && actualAction != ActionState.Dead)
        {
            if (rotation == 1)
            {
                rotateDir = 1;
            }
            else if (rotation == 2)
            {
                rotateDir = -1;
            }
            Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, rotateDir * 100, 0) * Time.deltaTime);
            rig.MoveRotation(rig.rotation * deltaRotation);
        }



    }

    private void Charge()
    {
        if (canChangeAction) {
            actualAction = ActionState.Charge;
            canChangeAction = false;
        }

        Vector3 dir = transform.forward;
        rig.AddForce(dir * moveSpeed * 3);

    }
    public void resetActions()
    {
        canChangeAction = true;
    }
    public override void AgentReset()
    {

    }

    public override void AgentOnDone()
    {

    }
    private void Update()
    {
        if (actualAction != ActionState.Dead)
        {
            staminaRegen();
            blockCost();
            setAnimation();
            if(transform.position.y<0)
            {
                Vector3 tempPosition = transform.position;
                tempPosition.y = 1;
                transform.position = tempPosition;
            }
        }

    }
    private void FixedUpdate()
    {

    }
    private void setAnimation()
    {
        anim.SetInteger("Action", (int)actualAction);
    }
    private void Attack()
    {
        if (stamina > 10)
        {
            actualAction = ActionState.Attacking;
            stamina -= 5;
        }

    }
    private void move(int actionDir)
    {
        Vector3 dirToGo = Vector3.zero;

        int dir = 0;
        if (actualAction == ActionState.Walk)
        {
            if (actionDir == 1)
            {
                dir = 1;
            }
            else if (actionDir == 2)
            {
                dir = -1;
            }
            dirToGo = transform.forward * dir;
        }
        if (actualAction == ActionState.Walk && dirToGo == Vector3.zero)
        {
            actualAction = ActionState.Idle;
        }
        rig.AddForce(dirToGo * moveSpeed);
    }
    private void blockCost()
    {
        if (actualAction == ActionState.Blocking)
            stamina -= blockStaminaCost * Time.deltaTime;
    }
    private void staminaRegen()
    {
        if (actualAction == ActionState.Idle || actualAction == ActionState.Walk)
        {
            if (stamina < 100)
            {
                stamina += Mathf.Min(this.staminaRegeneration * Time.deltaTime, 100);
            }


        }
    }
    private void Block()
    {
        if (stamina > 0)
        {
            stamina -= 10;
            actualAction = ActionState.Blocking;
        }
    }
    public void EnableAttack()
    {
        canChangeAction = false;
        weapon.EnableCollision();
    }

    public void DisableAttack()
    {
        weapon.DisableCollision();
        canChangeAction = true;
    }


    public void GetDmg(float dmg)
    {
        if (actualAction != ActionState.Blocking && health > 0)
        {
            health -= dmg;
            SetReward(-0.1f);
            if (health <= 0)
            {
                SetReward(-0.3f);
                Death();
            }
        }
        else
        {
            SetReward(0.3f);
        }
    }

    public void Death()
    {
        if (!isLearning)
        {
            actualAction = ActionState.Dead;
            health = 0;
            canChangeAction = false;
            Done();
            setAnimation();
            rig.isKinematic = true;
            GetComponent<CapsuleCollider>().enabled = false;
            this.enabled = false;
            AcademyBattleField.SendDeathMessage(TeamName);
        }
        else
        {
            health = 100;
            stamina = 100;
            transform.position = startingPosition;
        }
    }

    public void checkIfCanSeeEnemy()
    {
        string[] enemiesArr = { enemyTeam };
        List<float> enemiesProperties = ray.Perceive(viewDistance, rayAngles, enemiesArr, 0, 0);
        int enemiesSeen = 0;
        for(int i=0;i<rayAngles.Length*3;i++)
        {
            if(enemiesProperties[enemiesArr.Length+1]>0)
            {
                if(enemiesProperties[enemiesArr.Length + 1]*viewDistance<2.1)
                {
                    SetReward(0.1f*health/100); //reward for having enemy in melee range
                }
                SetReward(0.1f); //reward for having enemy in sight
                enemiesSeen++;
            }
        }
    }
}
