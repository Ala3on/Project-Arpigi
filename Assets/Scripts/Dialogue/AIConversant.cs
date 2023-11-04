using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{

    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Dialogue dialogue = null;
        [SerializeField] float startingDistance = 3.0f;

        public float GetStartingDistance()
        {
            return startingDistance;
        }
        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (dialogue == null)
            {
                return false;
            }
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>().StartDialogueAction(this, dialogue);
            }
            return true;
        }
    }

}