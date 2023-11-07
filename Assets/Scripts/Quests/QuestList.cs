using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{

    public class QuestList : MonoBehaviour
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
            OnQuestListUpdated?.Invoke();
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
    }

}