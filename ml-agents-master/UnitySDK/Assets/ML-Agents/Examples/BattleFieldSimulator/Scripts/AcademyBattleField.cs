using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcademyBattleField : Academy
{
    public static string[] detectableObjects = { "Team1", "Team2", "ArenaWall" };
    public static float[] rayAngles = { 45f, 90f, 135f, 110f, 70f };
    public Team team1 = new Team(Team.TeamTagEnum.Team1);
    public Team team2 = new Team(Team.TeamTagEnum.Team2);
    public override void InitializeAcademy()
    {
       
    }

    public override void AcademyReset()
    {


    }

    public override void AcademyStep()
    {


    }
}
