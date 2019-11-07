using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventListener : MonoBehaviour
{
    ActionWarriorAgent agent;
    private void Start()
    {
        agent = GetComponentInParent<ActionWarriorAgent>();
    }
    void DisableAttack()
    {
        agent.DisableAttack();
    }
    void EnableAttack()
    {
        agent.EnableAttack();
    }
}
