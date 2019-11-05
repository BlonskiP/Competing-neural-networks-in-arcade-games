using Assets.ML_Agents.Examples.BattleFieldSimulator.Scripts;
using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionWarriorAgent : Agent
{
    Animator anim;
    public float maxHealth = 100;
    public List<Action> actions;
    private Rigidbody rig;
    private RayPerception3D rayPerc;
    private WarriorRayPerception sight;
    private AcademyBattleField academy;
    [HideInInspector]
    public bool isActionDone = true;
    public WarriorInfo WarriorStats;
    private SwordAttack sword;
    public float staminaRegenRate = 1;
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-0.05f);
        int[] actionParams = { };
        if (isActionDone)
        {//can change action
            ResetTemporaryParameters();
            isActionDone = false;
            WarriorStats.actualAction = actions.Where(a => a.ActionKey == (int)vectorAction[0]).FirstOrDefault();
            if (WarriorStats.actualAction != null)
            {
                WarriorStats.actualAction.Perform(this, out isActionDone, actionParams);
            }
            else
            {
                AddReward(-0.3f);//punish for choosing not existing action
                isActionDone = true;
            }
        }
        else
        {//can't change action
            if (WarriorStats.actualAction != null)
                WarriorStats.actualAction.Continue(this, out isActionDone, actionParams);
            else
            {
                AddReward(-0.1f); //punish for continuing not existing action
            }
        }
    }

    private void ResetTemporaryParameters()
    {
        WarriorStats.viewDistance = WarriorStats.MaxViewDistance;
        WarriorStats.canTakeDmg = true;
    }

    public override void InitializeAgent()
    {
        base.InitializeAgent();

        academy = FindObjectOfType<AcademyBattleField>();
        WarriorStats = new WarriorInfo();
        sight = GetComponent<WarriorRayPerception>();
        if (gameObject.tag == "Team1")
        {
            WarriorStats.team = academy.team1;
        }
        else if (gameObject.tag == "Team2")
        {
            WarriorStats.team = academy.team2;
        }

        anim = GetComponent<Animator>();
        rayPerc = GetComponent<RayPerception3D>();
        rig = GetComponent<Rigidbody>();
        sword = GetComponentInChildren<SwordAttack>();
        sword.DisableCollision();
        WarriorStats.startingPosition = transform.position;
        WarriorStats.maxHealth = maxHealth;
        WarriorStats.maxStamina = maxHealth;
        WarriorStats.team.registerNewMember(this);
        WarriorStats.transform = transform;

        SetPrimalParameters();
    }
    public override void CollectObservations()
    {
        base.CollectObservations();
        if (WarriorStats.actualAction != null)
            AddVectorObs(WarriorStats.actualAction.ActionKey);
        else
            AddVectorObs(0);
        AddVectorObs(isActionDone);
      //  AddVectorObs(rayPerc.Perceive(WarriorStats.viewDistance, AcademyBattleField.rayAngles, AcademyBattleField.detectableObjects, 0, 0));
        AddVectorObs((int)WarriorStats.team.TeamTag);
        AddVectorObs(rig.rotation);
        Vector3 localVelocity = transform.InverseTransformDirection(rig.velocity);
        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
        AddVectorObs(transform.forward);
        AddVectorObs(WarriorStats.health / WarriorStats.maxHealth);
        AddVectorObs(WarriorStats.canTakeDmg);
        AddVectorObs(WarriorStats.stamina);
        AddVectorObs(sight.PerceiveWarriors(WarriorStats.viewDistance, AcademyBattleField.rayAngles, AcademyBattleField.detectableObjects));
        AddVectorObs((float)WarriorStats.intTeamMemberNumber/WarriorStats.team.TeamMembers.Count);
       
    }
    public override void AgentOnDone()
    {
        base.AgentOnDone();
        WarriorStats.actualAction = null;
    }
    public override void AgentReset()
    {
        base.AgentReset();
        SetPrimalParameters();
    }
    public void SetPrimalParameters()
    {
        WarriorStats.health = WarriorStats.maxHealth;
        WarriorStats.stamina = WarriorStats.maxStamina;
        WarriorStats.actualAction = null;
        this.isActionDone = true;
        rig.isKinematic = false;
        transform.position = WarriorStats.startingPosition;
    }

    private void setAnimation()
    {
        if (WarriorStats.health > 0)
        {
            if (WarriorStats.actualAction != null)
                anim.SetInteger("Action", WarriorStats.actualAction.ActionKey);
            else
                anim.SetInteger("Action", 0);
        }
        else
        {
            anim.SetInteger("Action",-10);//death animation
        }
       
    }
    private void FixedUpdate()
    {
        Debug.Log(WarriorStats.stamina);
        if(WarriorStats.stamina<WarriorStats.maxStamina)
        {
            WarriorStats.stamina += staminaRegenRate * Time.deltaTime;
        }
        setAnimation();
    }
    public void EnableAttack()
    {

    }
    public bool takeDmg(float dmg)
    {
        if (WarriorStats.canTakeDmg) {

            AddReward(-0.2f);//Panish for taking dmg
            WarriorStats.health -= dmg;
            if (WarriorStats.health <= 0)
            {
                Death();
            }
            return true;
        }
        else
        {
            if (WarriorStats.health > 0 && WarriorStats.actualAction != null) {
            float reward = ((maxHealth - WarriorStats.health) / maxHealth + WarriorStats.actualAction.succesReward)/2;
            AddReward(reward);//Reward for defending
            return false;
            }
        }
        return false;

    }
    public void Death()
    {
        WarriorStats.canTakeDmg = false;
        isActionDone = true;
        rig.isKinematic = true;
        Done();
    }
    public void DisableAttack()
    {
        this.isActionDone = true;
        sword.DisableCollision();
    }
    private void AddVectorObs(List<Observation> ObsVector)
    {
        foreach (var observation in ObsVector)
        {
            AddVectorObs(!observation.exist);
            if (observation.gameObject != null)
            {
                if(observation.type.Contains("Team"))//If warrior
                {
                    ActionWarriorAgent agent = observation.gameObject.GetComponent<ActionWarriorAgent>();
                    if(agent!=null)
                    {
                        AddVectorObs(0.1f);
                        AddVectorObs(agent.WarriorStats.health/agent.WarriorStats.maxHealth);
                        AddVectorObs((int)agent.WarriorStats.team.TeamTag);
                        if (agent.WarriorStats.actualAction != null)
                        {
                            AddVectorObs(agent.WarriorStats.actualAction.ActionKey / 10);
                        }
                        else AddVectorObs(0);
                        
                        AddVectorObs(agent.WarriorStats.canTakeDmg);
                        AddVectorObs(WarriorStats.stamina);
                    }
                }
                else if(observation.type.Contains("Wall"))
                {
                    AddVectorObs(0.2f);
                    for (int i = 0; i < 5; i++)
                        AddVectorObs(0);
                }
                else
                {
                    AddVectorObs(0.3f);
                    for (int i = 0; i < 5; i++)
                        AddVectorObs(0);
                }
                AddVectorObs(observation.distance / this.WarriorStats.viewDistance);
            }
            else //nothing found
            {
                for (int i = 0; i < 7; i++)
                    AddVectorObs(0);
            }
        }
    }
}
