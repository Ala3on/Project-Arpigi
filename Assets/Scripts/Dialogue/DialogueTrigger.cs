using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Dialogue
{

    public class DialogueTrigger : MonoBehaviour
    {

        [SerializeField] private Trigger[] triggers;
        public IEnumerable<Trigger> Triggers => triggers;

        [System.Serializable]
        public class Trigger
        {
            [SerializeField] DialogueAction action;
            [SerializeField] UnityEvent onTrigger = null;

            public void TriggerAction(DialogueAction actionToTrigger)
            {

                if (actionToTrigger == action)
                {
                    onTrigger.Invoke();
                }
            }

        }


    }

}