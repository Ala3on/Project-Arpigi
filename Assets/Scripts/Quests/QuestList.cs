using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using UnityEngine;

namespace RPG.Quests
{

    public class QuestList : MonoBehaviour, ISaveable, IPredicateEvaluator
    {
        List<QuestStatus> statuses = new List<QuestStatus>();

        public event Action OnQuestListUpdated;

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            QuestStatus newStatus = new QuestStatus(quest);
            statuses.Add(newStatus);
            OnQuestListUpdated?.Invoke();

            /* if (OnQuestListUpdated != null)
            {
                OnQuestListUpdated();
            }
 */
        }

        private bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            if (status == null) return;
            if (status.IsObjectiveComplete(objective)) return;
            status.CompleteObjective(objective);
            if (status.IsQuestComplete())
            {
                GiveRewards(quest);
            }
            OnQuestListUpdated?.Invoke();
        }

        /*  private void GiveRewards(Quest quest)
         {
             foreach (Quest.Reward reward in quest.GetRewards())
             {
                 bool success = GetComponent<Inventory>().AddToFirstEmptySlot(reward.item, reward.amount);
                 if (!success)
                 {
                     GetComponent<ItemDropper>().DropItem(reward.item, reward.amount);
                 }
             }
         } */

        private void GiveRewards(Quest quest)
        {
            Inventory inventory = GetComponent<Inventory>();
            ItemDropper dropper = GetComponent<ItemDropper>();
            foreach (Quest.Reward reward in quest.GetRewards())
            {
                // in case the reward is not stackable
                if (!reward.item.IsStackable())
                {
                    int given = 0;

                    // add all possible to empty slots
                    for (int i = 0; i < reward.amount; i++)
                    {
                        bool isGiven = inventory.AddToFirstEmptySlot(reward.item, 1);
                        if (!isGiven) break;
                        given++;
                    }

                    // if entire reward was given, go to the next reward
                    if (given == reward.amount) continue;

                    // if given less than in reward, drop the difference
                    for (int i = given; i < reward.amount; i++)
                    {
                        dropper.DropItem(reward.item, 1);
                    }
                }
                //if stackable, drop/add several units
                else
                {
                    bool isGiven = inventory.AddToFirstEmptySlot(reward.item, reward.amount);
                    if (!isGiven)
                    {
                        for (int i = 0; i < reward.amount; i++)
                        {
                            dropper.DropItem(reward.item, reward.amount);
                        }
                    }
                }
            }
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            foreach (QuestStatus status in statuses)
            {
                if (status.GetQuest() == quest)
                {
                    return status;
                }
            }
            return null;
        }

        public JToken CaptureState()
        {
            JArray state = new JArray();
            IList<JToken> stateList = state;
            foreach (QuestStatus status in statuses)
            {
                stateList.Add(status.CaptureState());
            }
            return state;

        }

        public void RestoreState(JToken state)
        {
            if (state is JArray stateArray)
            {
                statuses.Clear();
                IList<JToken> stateList = stateArray;
                foreach (JToken token in stateList)
                {
                    statuses.Add(new QuestStatus(token));
                }
            }

        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch (predicate)
            {
                case "HasQuest": return HasQuest(Quest.GetByName(parameters[0]));
                case "CompletedQuest": return GetQuestStatus(Quest.GetByName(parameters[0])).IsQuestComplete();
            }
            return null;
        }
    }

}