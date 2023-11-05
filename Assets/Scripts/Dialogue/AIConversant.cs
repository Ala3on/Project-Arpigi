using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Dialogue
{

    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] Dialogue dialogue = null;
        [SerializeField] float startingDistance = 3.0f;
        [SerializeField] string conversantName = "NPC";
        [SerializeField] Sprite conversantAvatar;


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
            Health health = GetComponent<Health>();
            if (dialogue == null || health.IsDead)
            {
                return false;
            }
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>().StartDialogueAction(this, dialogue);
            }
            return true;
        }

        public string GetName()
        {
            return conversantName;
        }

        public Sprite GetAvatar()
        {
            return conversantAvatar;
        }
    }

}