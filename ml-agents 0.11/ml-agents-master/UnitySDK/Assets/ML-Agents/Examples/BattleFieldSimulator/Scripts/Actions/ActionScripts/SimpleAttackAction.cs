using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "Agent's Actions/Simple Attack Action", order = 3)]
public class SimpleAttackAction : Action
{
    public override void Continue(Agent agent, out bool isActionDone, params int[] param)
    {
        isActionDone = false;
    }

    public override void Perform(Agent agent, out bool isActionDone, params int[] param)
    {
        SwordAttack attack = agent.GetComponentInChildren<SwordAttack>();
        if(attack!=null)
        {
            attack.EnableCollision();
        }
        isActionDone = false;
    }
}
