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
    public BoxCollider swordEdge;
    private RayPerception3D rayPerc;
    private AcademyBattleField academy;
    private Team team;
    [HideInInspector]
    public bool isActionDone = true;
    int animationID = 0;
    public int viewDistance = 20;
    public override void AgentAction(float[] vectorAction, string textAction)
    {
        
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
                    AddReward(-0.1f);//punish for choosing not existing action
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
        foreach (var edge in GetComponentsInChildren<BoxCollider>())
        {
            if (edge.tag == "SwordCollider")
            {
                swordEdge = edge;
                edge.enabled = false;
            }
        }
    }
    public override void CollectObservations()
    {
        base.CollectObservations();
        AddVectorObs(actualAction.ActionKey);
        AddVectorObs(isActionDone);
        AddVectorObs(rayPerc.Perceive(viewDistance, AcademyBattleField.rayAngles, AcademyBattleField.detectableObjects, 0, 0));
        AddVectorObs((int)team.TeamTag);
    }
    public override void AgentReset()
    {
        base.AgentReset();
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
        setAnimation();
    }

    public void EnableAttack()
    {

    }

    public void DisableAttack()
    {
        this.isActionDone = true;
        swordEdge.enabled = false;
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject!=this.gameObject)
        {
            if (AcademyBattleField.detectableObjects.Contains(other.tag))
            {
                AddReward(1.0f);
                Done();
            }
        }
    }
}
