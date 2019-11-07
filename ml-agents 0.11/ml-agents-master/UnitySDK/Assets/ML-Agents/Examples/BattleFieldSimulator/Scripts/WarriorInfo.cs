using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorInfo
{
    public Vector3 startingPosition = Vector3.zero;
    public int intTeamMemberNumber = 0;
    public Action actualAction = null;
   // private Action lastAction = null;
    public Team team = null;
    public float health = 0;
    public float maxHealth = 0;
    public int MaxViewDistance = 0;
    public int viewDistance = 0;
    public bool canTakeDmg = false;
    public Transform transform;
    public float stamina = 0;
    public float maxStamina = 0;
}
