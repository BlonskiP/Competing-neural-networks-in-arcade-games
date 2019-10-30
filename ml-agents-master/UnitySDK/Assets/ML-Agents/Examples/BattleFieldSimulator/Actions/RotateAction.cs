using System.Collections;
using System.Collections.Generic;
using MLAgents;
using UnityEngine;
[CreateAssetMenu(fileName = "Data", menuName = "Agent's Actions/Rotate Action", order = 2)]
public class RotateAction : Action
{
    public int dir = 1;
    public override void Continue(Agent agent, out bool isActionDone, params int[] param)
    {
        isActionDone = true;
    }

    public override void Perform(Agent agent, out bool isActionDone, params int[] param)
    {
        isActionDone = true;
        Rigidbody rig = agent.GetComponent<Rigidbody>();
        Quaternion deltaRotation = Quaternion.Euler(new Vector3(0, dir * 100, 0) * Time.deltaTime);
        rig.MoveRotation(rig.rotation * deltaRotation);

    }

}
