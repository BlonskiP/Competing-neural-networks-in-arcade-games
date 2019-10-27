using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcademyBattleField : Academy
{
    public static List<SimpleWarriorAgent> Team1 = new List<SimpleWarriorAgent>();
    public static List<SimpleWarriorAgent> Team2 = new List<SimpleWarriorAgent>();
    public static int team1Alive = 0;
    public static int team2Alive = 0;
    public override void InitializeAcademy()
    {
        
    }

    public override void AcademyReset()
    {


    }

    public override void AcademyStep()
    {


    }

    internal static int getTeamCount(string team)
    {
       if(team == "Team1")
        {
            return team1Alive;
        }
       if(team == "Team2")
        {
            return team2Alive;
        }
        return 0;
    }

    internal static void SendDeathMessage(string teamName)
    {
        if (teamName == "Team1")
        {
            team1Alive--;
        }
        if (teamName == "Team2")
        {
            team2Alive--;
        }
    }
}
