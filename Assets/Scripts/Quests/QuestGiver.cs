using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Quests
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] Quest[] quests;

        public void GiveQuest(int index)
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            if (questList == null) return;
            if (index < 0 || index >= quests.Length)
            {
                Debug.LogError("Quest index out of range");
                return;
            }
            questList.AddQuest(quests[index]);
        }
    }
}
