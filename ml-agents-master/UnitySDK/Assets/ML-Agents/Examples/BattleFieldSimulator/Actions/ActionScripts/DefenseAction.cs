using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "Agent's Actions/Defense Action", order = 5)]
public class DefenseAction : Action
{
    public override void Continue(Agent agent, out bool isActionDone, params int[] param)
    {
        isActionDone = true;
    }

    public override void Perform(Agent agent, out bool isActionDone, params int[] param)
    {
        ActionWarriorAgent actionAgent = agent.GetComponent<ActionWarriorAgent>();
        if(actionAgent!= null)
        {
            if (actionAgent.WarriorStats.stamina > 10)
            {
                actionAgent.WarriorStats.canTakeDmg = false;
                actionAgent.WarriorStats.stamina -= 5;
            }
        }
        isActionDone = false;
    }
}
