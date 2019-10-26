using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWarriorAgent : Agent
{
    public enum ActionState { Dead = -1, Idle = 0, Attacking = 1, Blocking = 2, Charge = 3, Walk = 4};
    public ActionState actualAction;
    Animator anim;
    public SwordAttack weapon;
    public string TeamName;
    public float stamina;
    public float health;
    private float staminaRegeneration = 1;
    private float blockStaminaCost = 2;
    public override void InitializeAgent()
    {
        anim = GetComponent<Animator>();
        actualAction = ActionState.Attacking;
        weapon = GetComponentInChildren<SwordAttack>();
        weapon.TeamName = this.TeamName;
    }

    public override void CollectObservations()
    {
        AddVectorObs(health);
        AddVectorObs(stamina);
        AddVectorObs(transform.position);
        AddVectorObs(transform.rotation);
        AddVectorObs((int)actualAction);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
      
    }

    public override void AgentReset()
    {

    }

    public override void AgentOnDone()
    {

    }
    private void Update()
    {
        staminaRegen();
        blockCost();
        setAnimation();
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
        if(stamina > 10)
        {
            actualAction = ActionState.Attacking;
            stamina -= 5;
        }
        
    }
    private void blockCost()
    {
        if(actualAction == ActionState.Blocking)
        stamina -= blockStaminaCost * Time.deltaTime;
    }
    private void staminaRegen()
    {
        if(actualAction == ActionState.Idle || actualAction == ActionState.Walk)
        {
            if(stamina < 100)
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
        weapon.EnableCollision();
    }

    public void DisableAttack()
    {
        weapon.DisableCollision();
    }

    public void GetDmg(float dmg)
    {
        if(actualAction!=ActionState.Blocking && health > 0)
        {
            health -= dmg;
            if(health <= 0)
            {
                actualAction = ActionState.Dead;
            }
        }
    }
}
