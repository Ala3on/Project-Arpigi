using System;
using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using UnityEngine;

namespace RPG.Quests
{

    public class QuestCompletion : MonoBehaviour
    {
        [SerializeField] Quest quest;
        [SerializeField] string objective;
        [SerializeField] Quest.Reward[] objectiveReward;

        public void CompleteObjective()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.CompleteObjective(quest, objective);
            if (objectiveReward != null)
            {
                GiveObjectiveReward();
            }
        }

        private void GiveObjectiveReward()
        {
            Inventory inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
            ItemDropper dropper = GetComponent<ItemDropper>();
            foreach (Quest.Reward reward in objectiveReward)
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
    }

}