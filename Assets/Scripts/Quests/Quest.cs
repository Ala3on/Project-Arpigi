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
        [SerializeField] List<string> objectives = new List<string>();

        internal string GetTitle()
        {
            return name;
        }

        public int GetObjectiveCount()
        {
            return objectives.Count;
        }

        internal IEnumerable<string> GetObjectives()
        {
            return objectives;
        }

        internal bool HasObjective(string objective)
        {
            return objectives.Contains(objective);
        }
    }
}
