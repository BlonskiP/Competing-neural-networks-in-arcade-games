using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAgent : Agent
{
    #region stats
    public float stamina;
    public float health;
    public float viewDistance = 10f;
    public float staminaRegeneration = 1;
    public float blockStaminaCost = 2;
    public bool canChangeAction = true;
    public bool isLearning = true;
    public int moveSpeed = 100;
    public Team agentTeam;
    #endregion
    #region Components
    Animator anim;
    public Rigidbody rig;
    public SwordAttack weapon;
    RayPerception ray;
    AcademyBattleField academy;
    #endregion
    public enum ActionState { Dead = -1, Idle = 0, Attacking = 1, Blocking = 2, Charge = 3, Walk = 4 };
    public ActionState actualAction;
    List<float> observation;
    private Vector3 startingPosition;
    #region ML_AGENTS METHODS
    public override void InitializeAgent()
    {
        getComponents();
        actualAction = ActionState.Idle;
    }
    public override void AgentReset()
    {
        base.AgentReset();
        transform.position = startingPosition;
        health = 100;
        stamina = 100;

    }

    public override void CollectObservations()
    {
        List<float> obs = ray.Perceive(viewDistance, AcademyBattleField.rayAngles, AcademyBattleField.detectableObjects, 0, 0);
        AddVectorObs(canChangeAction);
        AddVectorObs(health);
        AddVectorObs(stamina);
        AddVectorObs((int)agentTeam.TeamTag);
        AddVectorObs(obs);
        AddVectorObs((int)actualAction);

    }
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        int moveForward = Mathf.FloorToInt(vectorAction[0]);
        int rotation = Mathf.FloorToInt(vectorAction[1]);
        int newAction = Mathf.FloorToInt(vectorAction[2]);
        checkIfCanSeeEnemy(); //reward if can see enemy
        int rotateDir = 0;
        if (canChangeAction && actualAction != ActionState.Dead)
            switch (newAction)
            {
                case 0:
                    { //Start idle
                        actualAction = ActionState.Idle;
                        break;
                    }
                case 1:
                    {//move
                        actualAction = ActionState.Walk;
                        Move(moveForward);
                        break;
                    }
                case 2:
                    { //Attack
                        actualAction = ActionState.Attacking;
                        Attack();
                        break;
                    }
                case 3:
                    { //Block incoming attacks
                        Block();
                        break;
                    }
                case 4:
                    {//Charge!!
                        Charge();
                        break;
                    }
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
    #endregion
    #region Monobehaviour methods
    private void Update()
    {
        if (actualAction != ActionState.Dead)
        {
            staminaRegen();
            blockCost();
            setAnimation();
        }

    }
    #endregion
    #region Private methods
   
    private void getComponents()
    {
        academy = FindObjectOfType<AcademyBattleField>();
        anim = GetComponent<Animator>();
        weapon = GetComponentInChildren<SwordAttack>();
        rig = GetComponent<Rigidbody>();
        ray = GetComponent<RayPerception>();
        if (gameObject.tag == "Team1")
        {
            agentTeam = academy.team1;
        }
        else if (gameObject.tag == "Team2")
        {
            agentTeam = academy.team2;
        }
        agentTeam.registerNewMember(this);
        startingPosition = transform.position;
    }

    private void setAnimation()
    {
        anim.SetInteger("Action", (int)actualAction);
    }
    private void checkIfCanSeeEnemy()
    {
        string[] enemiesArr = { agentTeam.EnemyTeamName };
        List<float> enemiesProperties = ray.Perceive(viewDistance, AcademyBattleField.rayAngles, enemiesArr, 0, 0);
        for (int i = 0; i < AcademyBattleField.rayAngles.Length * 3; i++)
        {
            if (enemiesProperties[enemiesArr.Length + 1] > 0)
            {
                float distanceToEnemy = enemiesProperties[enemiesArr.Length + 1] * viewDistance;
                Gizmos.DrawWireSphere(transform.position, distanceToEnemy);
                if (distanceToEnemy < 2.1 || distanceToEnemy > 1)
                {
                    AddReward(0.1f); //reward for having enemy in melee range
                }
                AddReward(0.1f); //reward for having enemy in sight
            }
        }
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
    #endregion
    #region Actions
    private void Move(int actionDir)
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
    private void Charge()
    {
        if (canChangeAction)
        {
            actualAction = ActionState.Charge;
            canChangeAction = false;
        }
        if (actualAction == ActionState.Charge)
        {
            Vector3 dir = transform.forward;
            rig.AddForce(dir * moveSpeed * 3);
            RaycastHit raycastHit;
            if (Physics.SphereCast(transform.position, 3, transform.forward, out raycastHit, 5)) {
                WarriorAgent target = raycastHit.collider.GetComponent<WarriorAgent>();
                if(target!=null)
                {
                    target.GetDmg(5);
                    AddReward(0.1f);
                }
            };
           

        }
    }
    private void Attack()
    {
        if (stamina > 10)
        {
            actualAction = ActionState.Attacking;
            stamina -= 5;
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
    public void Death()
    {
        
        if(isLearning)
        {
            AgentReset();
        }
        else
        {
            agentTeam.aliveMembers--;
            actualAction = ActionState.Dead;
            health = 0;
            canChangeAction = false;
            Done();
            setAnimation();
            rig.isKinematic = true;
            GetComponent<CapsuleCollider>().enabled = false;
            this.enabled = false;
        }
    }
    #endregion
    #region PublicMethods And Events
    public void GetDmg(float dmg)
    {
        if (actualAction != ActionState.Blocking && health > 0)
        {
            health -= dmg;
            AddReward(-0.1f); //Panish for not blocking
            if (health <= 0)
            {
                AddReward(-0.6f); //Panish for dying 
                Death();
            }
        }
        else
        {
            AddReward(0.6f); //Reward for blocking
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
    public void resetActions()
    {
        canChangeAction = true;
    }
    #endregion






}
