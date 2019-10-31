using MLAgents;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionWarriorAgent : Agent
{
    Animator anim;
    Vector3 startingPosition;
    public List<Action> actions;
    public Action actualAction;
    private Action lastAction;
    private Rigidbody rig;
    private RayPerception3D rayPerc;
    private AcademyBattleField academy;
    public Team team;
    [HideInInspector]
    public bool isActionDone = true;
    int hitCounter = 0;
    public float health = 100;
    public float maxHealth = 100;
    public int viewDistance = 30;
    public bool canTakeDmg = true;
    private SwordAttack sword;
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        AddReward(-1/5000f);
        int[] actionParams = { };
        if(isActionDone)
        {//can change action
            isActionDone = false;
            actualAction = actions.Where(a => a.ActionKey==(int)vectorAction[0]).FirstOrDefault();
                if (actualAction != null)
                {
                    actualAction.Perform(this, out isActionDone, actionParams);
                }
                else
                { 
                    AddReward(-0.3f);//punish for choosing not existing action
                    isActionDone = true;
                }
        }
        else
        {//can't change action
            if (actualAction != null)
                actualAction.Continue(this,out isActionDone, actionParams);
            else
            {
             AddReward(-0.1f); //punish for continuing not existing action
            }
        }
    }
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        academy = FindObjectOfType<AcademyBattleField>();
        startingPosition = transform.position;
        if (gameObject.tag == "Team1")
        {
            team = academy.team1;
        }
        else if (gameObject.tag == "Team2")
        {
            team = academy.team2;
        }
        team.registerNewMember(this);
        anim = GetComponent<Animator>();
        rayPerc = GetComponent<RayPerception3D>();
        rig = GetComponent<Rigidbody>();
        sword = GetComponentInChildren<SwordAttack>();
        sword.DisableCollision();
        health = maxHealth;
    }
    public override void CollectObservations()
    {
        base.CollectObservations();
        if (actualAction != null)
            AddVectorObs(actualAction.ActionKey);
        else
            AddVectorObs(0);
        AddVectorObs(isActionDone);
        AddVectorObs(rayPerc.Perceive(viewDistance, AcademyBattleField.rayAngles, AcademyBattleField.detectableObjects, 0, 0));
        AddVectorObs((int)team.TeamTag);
        AddVectorObs(rig.rotation);
        Vector3 localVelocity = transform.InverseTransformDirection(rig.velocity);
        AddVectorObs(localVelocity.x);
        AddVectorObs(localVelocity.z);
        AddVectorObs(health / maxHealth);
    }
    public override void AgentReset()
    {
        base.AgentReset();
        hitCounter = 0;
        health = maxHealth;
        transform.position = startingPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void setAnimation()
    {
        if (actualAction != null)
            anim.SetInteger("Action", actualAction.ActionKey);
        else
            anim.SetInteger("Action", 0);
    }
    // Update is called once per frame
    void Update()
    {
        //Should not have logic here
    }
    private void FixedUpdate()
    {
        setAnimation();
    }
    public void EnableAttack()
    {

    }
    public bool takeDmg(float dmg)
    {
        if (canTakeDmg) {
            if (health > 0)
            {
                AddReward(-0.2f);//Panish for taking dmg
                health -= dmg;
                if (health < 0)
                {//Death
                    AddReward(-1f);
                    Death();
                }
               
            }
            return true;
        }
        else
        {
            AddReward(0.2f);//Reward for defending
            return false;
        }
        
    }
    public void Death()
    {
        Done();
    }
    public void DisableAttack()
    {
        this.isActionDone = true;
        sword.DisableCollision();
    }
    private void OnTriggerExit(Collider other)
    {
        //Collider[] list = GetComponentsInChildren<Collider>();
        //if (!list.Contains(other)&&other.enabled) { 
        //    ActionWarriorAgent warrior = other.GetComponentInParent<ActionWarriorAgent>();
        //if(warrior!=null && warrior.team.TeamName != team.TeamName)
        //{
        //    bool attackSucess = warrior.takeDmg(25);
        //    if (attackSucess) AddReward(1);
        //}
        //}
    }
}
