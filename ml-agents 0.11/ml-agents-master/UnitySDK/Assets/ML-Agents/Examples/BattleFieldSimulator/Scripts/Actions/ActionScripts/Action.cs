using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
abstract public class Action : ScriptableObject
{
    public int ActionKey;
    public float succesReward = 0;
    public string AnimationBoolName;
    public abstract void Perform(Agent agent, out bool isActionDone, params int[] param);

    public abstract void Continue(Agent agent, out bool isActionDone, params int[] param);
}
