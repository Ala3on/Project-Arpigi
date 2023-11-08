using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Quests
{

    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI title;
        [SerializeField] TextMeshProUGUI progress;

        QuestStatus status;

        public void Setup(QuestStatus _status)
        {
            status = _status;
            title.text = _status.GetQuest().GetTitle();
            progress.text = status.GetCompletedCounts() + "/" + _status.GetQuest().GetObjectiveCount().ToString();
        }

        public QuestStatus GetQuestStatus()
        {
            return status;
        }
    }

}