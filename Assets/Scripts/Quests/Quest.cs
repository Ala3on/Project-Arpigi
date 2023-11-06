using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    using System;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Quest", menuName = "RPG/New Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] string[] objective;

        internal string GetTitle()
        {
            return name;
        }

        public int GetObjectiveCount()
        {
            return objective.Length;
        }

        internal IEnumerable<string> GetObjectives()
        {
            return objective;
        }
    }
}
