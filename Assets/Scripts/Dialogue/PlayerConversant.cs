using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Dialogue
{

    public class PlayerConversant : MonoBehaviour, IAction
    {
        Dialogue currentDialogue = null;
        DialogueNode currentNode = null;
        bool isChoosing = false;

        AIConversant targetConversant = null;
        AIConversant currentConversant = null;
        Dialogue targetDialogue = null;

        public event Action onConversationUpdated;

        public void StartDialogueAction(AIConversant newConversant, Dialogue newDialogue)
        {
            if (newConversant == currentConversant) return;
            Quit();  //clear any old conversant 
            GetComponent<ActionScheduler>().StartAction(this);
            targetConversant = newConversant;
            currentConversant = newConversant;
            targetDialogue = newDialogue;
        }

        void Update()
        {
            if (!targetConversant) return;
            if (Vector3.Distance(transform.position, targetConversant.transform.position) > targetConversant.GetStartingDistance())
            {
                GetComponent<Mover>().MoveTo(targetConversant.transform.position, 1.0f);
            }
            else
            {
                GetComponent<Mover>().Cancel(); //stop player movement once in range of conversant
                StartDialogue(targetConversant, targetDialogue);
                targetConversant = null;
            }
        }


        public void StartDialogue(AIConversant targetConversant, Dialogue newDialogue)
        {
            //targetConversant.transform.LookAt(transform);
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            onConversationUpdated();

            StartCoroutine(RotatePlayer(targetConversant.transform));
        }

        private IEnumerator RotatePlayer(Transform target)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float duration = 1.0f;
            float time = 0.0f;

            while (time < duration)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            transform.rotation = targetRotation;
        }

        public bool IsActive()
        {
            return currentDialogue != null;
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }

        public string GetText()
        {
            if (currentNode == null)
            {
                return "No Conversation";
            }
            return currentNode.GetText();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return currentDialogue.GetPlayerChildren(currentNode);
        }


        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int numPlayerResponses = currentDialogue.GetPlayerChildren(currentNode).Count();
            if (numPlayerResponses > 1)
            {
                isChoosing = true;
                onConversationUpdated();
                return;
            }

            isChoosing = false;

            DialogueNode[] children = currentDialogue.GetAllChildren(currentNode).ToArray();
            int index = UnityEngine.Random.Range(0, children.Count());
            currentNode = children[index];
            onConversationUpdated();
        }
        public bool HasNext()
        {
            //return currentDialogue.GetNextDialogue() != null;


            return currentDialogue.GetAllChildren(currentNode).Count() > 0;
        }

        public void Quit()
        {
            currentDialogue = null;
            currentNode = null;
            isChoosing = false;
            currentConversant = null;
            onConversationUpdated();
        }

        public void Cancel()
        {
            Quit();
        }
    }
}
