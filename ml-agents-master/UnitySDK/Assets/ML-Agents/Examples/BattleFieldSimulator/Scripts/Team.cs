using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team
{
    public List<Agent> TeamMembers;
    public int aliveMembers = 0;
    public enum TeamTagEnum { Team1 = 1, Team2 = 2}
    public TeamTagEnum TeamTag;
    public string TeamName;
    public string EnemyTeamName;

    public Team(TeamTagEnum tag)
    {
        TeamTag = tag;
        TeamMembers = new List<Agent>();
        if((int)TeamTag==1)
        {
            TeamName = "Team1";
            EnemyTeamName = "Team2";
        }
        else
        {
            TeamName = "Team2";
            EnemyTeamName = "Team1";
        }
    }
    public void registerNewMember(Agent agent)
    {
        TeamMembers.Add(agent);
        aliveMembers++;
    }
}
