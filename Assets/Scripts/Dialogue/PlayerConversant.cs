using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPG.Control;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Dialogue
{

    public class PlayerConversant : MonoBehaviour, IAction
    {
        [SerializeField] string playerName;
        [SerializeField] string companionName;
        [SerializeField] Sprite playerSprite;
        [SerializeField] Sprite companionSprite;
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
            TriggerEnterAction();
            onConversationUpdated();

            Transform companion = GameObject.FindWithTag("Companion").transform;
            StartCoroutine(RotateCharacter(transform, targetConversant.transform));
            StartCoroutine(RotateCharacter(companion, targetConversant.transform));
            StartCoroutine(RotateCharacter(targetConversant.transform, transform));
        }

        public void Quit()
        {
            currentDialogue = null;
            TriggerExitAction();
            currentNode = null;
            isChoosing = false;
            currentConversant = null;
            onConversationUpdated();
        }


        private IEnumerator RotateCharacter(Transform character, Transform target)
        {
            Vector3 direction = target.position - character.position;
            direction.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float duration = 1.0f;
            float time = 0.0f;

            while (time < duration)
            {
                character.rotation = Quaternion.Lerp(character.rotation, targetRotation, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            character.rotation = targetRotation;
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
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }


        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }

        public void Next()
        {
            int numPlayerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if (numPlayerResponses > 1)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }

            isChoosing = false;

            DialogueNode[] children = FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).ToArray();
            int index = UnityEngine.Random.Range(0, children.Count());
            TriggerExitAction();
            currentNode = children[index];
            TriggerEnterAction();
            onConversationUpdated();
        }
        public bool HasNext()
        {
            return FilterOnCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0;
        }

        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (DialogueNode node in inputNode)
            {
                if (node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }



        private void TriggerEnterAction()
        {
            if (currentNode != null)
            {
                Debug.Log("TriggerEnterAction " + currentNode.GetOnEnterAction());
                TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                Debug.Log("TriggerExitAction " + currentNode.GetOnExitAction());
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(DialogueAction dialogueAction)
        {
            if (dialogueAction == DialogueAction.None) return;
            if (currentConversant == null) return;
            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(dialogueAction);
            }
        }

        public string GetCurrentConversantName()
        {
            if (currentNode == null)
            {
                return "No Conversation";
            }
            if (currentNode.IsPlayerSpeaking() || isChoosing)
            {
                return playerName;
            }
            if (currentNode.GetSpeaker() == Speaker.Companion)
            {
                return companionName;
            }
            if (currentConversant == null)
            {
                return "";
            }
            else
            {
                return currentConversant.GetName();
            }
        }

        public void Cancel()
        {
            Quit();
        }

        public Sprite GetCurrentConversantAvatar()
        {
            if (currentNode == null)
            {
                return null;
            }
            if (currentNode.IsPlayerSpeaking() || isChoosing)
            {
                return playerSprite;
            }
            if (currentNode.GetSpeaker() == Speaker.Companion)
            {
                return companionSprite;

            }
            if (currentConversant != null)
            {
                return currentConversant.GetAvatar();
            }
            return null;

        }
    }
}
