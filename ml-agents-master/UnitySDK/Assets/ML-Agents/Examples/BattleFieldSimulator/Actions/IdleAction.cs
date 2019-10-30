using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "Agent's Actions/Idle action", order = 4)]
public class IdleAction : Action
{
    public override void Continue(Agent agent, out bool isActionDone, params int[] param)
    {
        isActionDone = true;
    }

    public override void Perform(Agent agent, out bool isActionDone, params int[] param)
    {
        isActionDone = true;
    }
}
