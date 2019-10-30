using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Agent's Actions/Move Action", order = 1)]
public class MoveAction : Action
{
    public int direction = 1;
    public int moveSpeed = 2;
    public override void Perform(Agent agent, out bool isActionDone, params int[] param)
    {
        Rigidbody rig = agent.gameObject.GetComponent<Rigidbody>();
        Vector3 dirToGo = Vector3.zero;
        dirToGo = agent.transform.forward * direction;
        rig.AddForce(dirToGo * moveSpeed, ForceMode.VelocityChange);
        isActionDone = true;
    }

    public override void Continue(Agent agent, out bool isActionDone, params int[] param)
    {
        agent.AddReward(-0.1f); //punish for choosing this action. Should't be posible to get here.
        isActionDone = true;
    }
}
