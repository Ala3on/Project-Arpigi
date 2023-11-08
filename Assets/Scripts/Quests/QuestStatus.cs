using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestStatus
    {
        Quest quest;
        List<string> completedObjectives = new List<string>();

        /// <summary>
        /// This constructor is used to create a new quest status.
        /// </summary>
        /// <param name="quest"></param>
        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }


        /// <summary>
        /// This constructor is used to load the quest status from the save file.
        /// </summary>
        /// <param name="objectState"></param>
        public QuestStatus(JToken objectState)
        {
            if (objectState is JObject state)
            {
                IDictionary<string, JToken> stateDict = state;
                quest = Quest.GetByName(stateDict["questName"].ToObject<string>());
                completedObjectives.Clear();
                if (stateDict["completedObjectives"] is JArray completedState)
                {
                    IList<JToken> completedStateArray = completedState;
                    foreach (JToken objective in completedStateArray)
                    {
                        completedObjectives.Add(objective.ToObject<string>());
                    }
                }
            }
        }

        public Quest GetQuest()
        {
            return quest;
        }

        public int GetCompletedCounts()
        {
            return completedObjectives.Count;
        }

        public bool IsObjectiveComplete(string objective)
        {
            return completedObjectives.Contains(objective);
        }

        public bool IsQuestComplete()
        {
            foreach (Quest.Objective objective in quest.GetObjectives())
            {
                if (!IsObjectiveComplete(objective.reference))
                {
                    return false;
                }
            }
            return true;
        }

        public void CompleteObjective(string objective)
        {
            if (quest.HasObjective(objective))
            {

                completedObjectives.Add(objective);
            }
        }



        public JToken CaptureState()
        {
            JObject state = new JObject();
            IDictionary<string, JToken> stateDict = state;
            stateDict["questName"] = quest.name;
            JArray completedState = new JArray();
            IList<JToken> completedStateArray = completedState;
            foreach (string objective in completedObjectives)
            {
                completedStateArray.Add(JToken.FromObject(objective));
            }
            stateDict["completedObjectives"] = completedState;
            return state;
        }

    }

}