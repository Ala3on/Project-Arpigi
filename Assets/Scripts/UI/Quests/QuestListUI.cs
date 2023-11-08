using System.Collections;
using System.Collections.Generic;
using RPG.Quests;
using UnityEngine;

namespace RPG.UI.Quests
{

    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] QuestItemUI questPrefab;

        QuestList questList;

        void Awake() //You can safely cache most references here
        {
            questList = GameObject.FindWithTag("Player").GetComponent<QuestList>();
        }
        void OnEnable() //You can subscribe to events here
        {
            questList.OnQuestListUpdated += Redraw;
            Redraw();
        }
        void OnDisable() //You should always unsubscribe when the object is disabled.
        {
            questList.OnQuestListUpdated -= Redraw;
        }

        void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.OnQuestListUpdated += Redraw;
            Redraw();
        }

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            foreach (QuestStatus status in questList?.GetStatuses())
            {
                QuestItemUI UIInstance = Instantiate<QuestItemUI>(questPrefab, transform);
                UIInstance.Setup(status);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}