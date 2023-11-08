using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    using System;
    using GameDevTV.Inventories;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Quest", menuName = "RPG/New Quest", order = 0)]
    public class Quest : ScriptableObject
    {
        [SerializeField] List<Objective> objectives = new List<Objective>();
        [SerializeField] List<Reward> rewards = new List<Reward>();

        /// <summary>
        /// Class to hold the reward information.
        /// </summary>
        [System.Serializable]
        public class Reward
        {
            public InventoryItem item;
            [Min(1)]
            public int amount;
        }

        /// <summary>
        /// Class to hold the objective information.
        /// </summary>
        [System.Serializable]
        public class Objective
        {
            public string reference;
            public string description;
        }

        public string GetTitle()
        {
            return name;
        }

        public int GetObjectiveCount()
        {
            return objectives.Count;
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }

        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }

        public bool HasObjective(string objectiveRef)
        {
            //return objectives.Contains(objective);
            foreach (Objective obj in objectives)
            {
                if (obj.reference == objectiveRef)
                {
                    return true;
                }
            }
            return false;
        }

        public static Quest GetByName(string questName)
        {
            Quest[] quests = Resources.LoadAll<Quest>("");
            foreach (Quest quest in quests)
            {
                if (quest.name == questName)
                {
                    return quest;
                }
            }
            return null;
        }
    }
}
