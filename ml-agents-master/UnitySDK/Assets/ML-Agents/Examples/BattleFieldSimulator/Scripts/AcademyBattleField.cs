using MLAgents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AcademyBattleField : Academy
{
    public static string[] detectableObjects = { "Team1", "Team2", "ArenaWall" };
    public static float[] rayAngles = { -90f, -45f, -135f, 0f, 45f, 90f, 135f, 180f, 110f, 70f };
    public Team team1 = new Team(Team.TeamTagEnum.Team1);
    public Team team2 = new Team(Team.TeamTagEnum.Team2);
    public TeamSpawnPoint[] teamSpawnPoint;
    public override void InitializeAcademy()
    {
        teamSpawnPoint = FindObjectsOfType<TeamSpawnPoint>();
        InitializeSpawners();

    }
    public void ResetTeams()
    {
        team1 = new Team(Team.TeamTagEnum.Team1);
        team2 = new Team(Team.TeamTagEnum.Team2);
}
    public void InitializeSpawners()
    {
        ResetTeams();
        foreach (var spawnPoint in teamSpawnPoint)
        {
            if (spawnPoint.tag == team1.TeamName)
                spawnPoint.SpawnTeam(team1);
            else if (spawnPoint.tag == team2.TeamName)
                spawnPoint.SpawnTeam(team2);
        }
    }
    public override void AcademyReset()
    {
        Agent[] agents = FindObjectsOfType<Agent>();
        foreach(var agent in agents)
        {
            Destroy(agent.gameObject);
        }
        InitializeSpawners();
    }

    public override void AcademyStep()
    {


    }
}
