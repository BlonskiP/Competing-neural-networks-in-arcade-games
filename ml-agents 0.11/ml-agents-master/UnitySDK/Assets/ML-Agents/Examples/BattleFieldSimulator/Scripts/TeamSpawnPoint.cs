using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSpawnPoint : MonoBehaviour
{
    public GameObject team1Member;
    public GameObject team2Member;
    public int howManyWarriors = 0;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (tag == "Team2")
            Gizmos.color = Color.blue;
        Vector3 startPos = transform.position;
        startPos.y = 1;
        Gizmos.DrawWireCube(startPos, new Vector3(howManyWarriors/4+1,1,howManyWarriors/4+1));
    }

    public void SpawnTeam(Team team)
    {
        Vector3 startPos = transform.position;
        startPos.y = 0;
        startPos.x -= 1;
        startPos.z -= 1;
        GameObject toInit;
        if(team.TeamName==team1Member.tag)
        {
            toInit = team1Member;
        }
        else
            toInit = team2Member;

        for (int i = 1; i <= howManyWarriors; i++)
        {
            Instantiate(toInit,startPos,Quaternion.Euler(0,90,0));
            startPos.z++;
            if(i%4==0)
            {
                startPos.z -= 4;
                startPos.x++;
            }
        }
    }
}
