using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.ML_Agents.Examples.BattleFieldSimulator.Scripts
{
    public class Observation
    {
        public GameObject gameObject;
        public string type;
        public float distance = 0;
        public bool exist = false;
    }
}
