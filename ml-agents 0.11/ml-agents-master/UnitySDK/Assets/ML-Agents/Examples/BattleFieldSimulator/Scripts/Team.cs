﻿using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team
{
    public List<Agent> TeamMembers;
    public enum TeamTagEnum { Team1 = 0, Team2 = 1}
    public TeamTagEnum TeamTag;
    public string TeamName;
    public string EnemyTeamName;

    public Team(TeamTagEnum tag)
    {
        TeamTag = tag;
        TeamMembers = new List<Agent>();
        if((int)TeamTag==0)
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
    public int registerNewMember(Agent agent)
    {
        TeamMembers.Add(agent);
        return TeamMembers.Count;
    }
    public bool hasAliveMembers()
    {
        if (TeamMembers.Count > 0)
            return true;
        return false;
    }
}