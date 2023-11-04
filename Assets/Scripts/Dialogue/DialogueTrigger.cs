using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Dialogue
{

    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] DialogueAction action;
        [SerializeField] UnityEvent onTrigger = null;

        public void Trigger(DialogueAction actionToTrigger)
        {
            if (actionToTrigger == action)
            {
                onTrigger.Invoke();
            }
        }

    }

}