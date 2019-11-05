﻿using Assets.ML_Agents.Examples.BattleFieldSimulator.Scripts;
using MLAgents;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WarriorRayPerception : RayPerception3D
{
    RaycastHit hit;
    private Observation obs;
    private List<Observation> warriorPerceptionBuffer = new List<Observation>();

    public List<Observation> PerceiveWarriors(float rayDistance,
            float[] rayAngles, string[] detectableObjects)
    {
        warriorPerceptionBuffer.Clear();
        warriorPerceptionBuffer.Capacity = rayAngles.Length;

        foreach (var angle in rayAngles)
        {
            warriorPerceptionBuffer.Add(getWarriorInfo(angle, rayDistance, detectableObjects));
        }
        return warriorPerceptionBuffer;
    }

    Observation getWarriorInfo(float angle, float rayDistance, string[] detectableObjects)
    {
        obs = new Observation();
        endPosition = transform.TransformDirection(
                PolarToCartesian(rayDistance, angle));
        endPosition.y = 0;
        if (Application.isEditor)
        {
            Debug.DrawRay(transform.position + new Vector3(0f, 0, 0f),
                    endPosition, Color.red, 0.01f, true);
        }
        if (Physics.SphereCast(transform.position +
                                   new Vector3(0f, 0, 0f), 0.5f,
                endPosition, out hit, rayDistance)) //if hit enything
        { 
                obs.gameObject = hit.collider.gameObject;
                obs.exist = true;
                obs.type = hit.collider.gameObject.tag;
                obs.distance = hit.distance;
        }
        return obs;
    }
}
