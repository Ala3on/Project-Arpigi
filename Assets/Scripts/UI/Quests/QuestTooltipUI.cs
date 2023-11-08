using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPG.Quests;
using TMPro;
using UnityEngine;

namespace RPG.UI.Quests
{

    public class QuestTooltipUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] Transform objectiveContainer;
        [SerializeField] GameObject objectivePrefab;
        [SerializeField] GameObject objectiveIncompletePrefab;
        [SerializeField] TextMeshProUGUI reward;

        public void Setup(QuestStatus status)
        {
            Quest quest = status.GetQuest();
            title.text = quest.GetTitle();
            foreach (Transform child in objectiveContainer)
            {
                Destroy(child.gameObject);
            }
            foreach (Quest.Objective objective in quest.GetObjectives())
            {
                GameObject prefab = status.IsObjectiveComplete(objective.reference) ? objectivePrefab : objectiveIncompletePrefab;
                GameObject objectiveInstance = Instantiate(prefab, objectiveContainer);

                objectiveInstance.GetComponentInChildren<TextMeshProUGUI>().text = objective.description;
            }
            reward.text = GetRewardText(quest);
        }

        private string GetRewardText(Quest quest)
        {
            List<string> rewards = new List<string>();
            foreach (Quest.Reward reward in quest.GetRewards())
            {
                // string amount = reward.amount > 1 ? $"x{reward.amount}" : "";
                rewards.Add($"{reward.amount}x {reward.item.GetDisplayName()}");
            }
            return rewards.Any() ? $"{string.Join("\n", rewards)}" : "No Reward";
        }
    }
}