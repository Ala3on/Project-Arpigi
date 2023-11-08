using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;
using System;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI dialogueText;
        [SerializeField] TextMeshProUGUI conversantName;
        [SerializeField] Button nextButton;
        [SerializeField] Button quitButton;
        [SerializeField] Transform choicesRoot;
        [SerializeField] GameObject normalDialogueRoot;
        [SerializeField] GameObject choicePrefab;
        [SerializeField] Image avatar;

        bool showingText = false;
        // Start is called before the first frame update
        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.onConversationUpdated += UpdateUi;
            nextButton.onClick.AddListener(Next);
            GetComponent<Button>().onClick.AddListener(Next);
            quitButton.onClick.AddListener(() => playerConversant.Quit());
            UpdateUi();
        }

        void Next()
        {
            if (showingText)
            {
                StopAllCoroutines();
                dialogueText.text = playerConversant.GetText();
                showingText = false;
                return;
            }
            if (playerConversant.HasNext())
            {
                playerConversant.Next();

            }
            else
            {
                playerConversant.Quit();
            }
        }

        void UpdateUi()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive())
            {
                return;
            }
            conversantName.text = playerConversant.GetCurrentConversantName();
            Sprite avatarSprite = playerConversant.GetCurrentConversantAvatar();

            if (avatarSprite != null)
            {
                avatar.enabled = true;
                avatar.sprite = avatarSprite;
            }
            else
            {
                avatar.enabled = false;
            }

            choicesRoot.gameObject.SetActive(playerConversant.IsChoosing());
            normalDialogueRoot.gameObject.SetActive(!playerConversant.IsChoosing());

            if (playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                //dialogueText.text = playerConversant.GetText();
                StartCoroutine(ShowTextSlowly(playerConversant.GetText()));
                //nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
        }

        private void BuildChoiceList()
        {
            foreach (Transform child in choicesRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choicesRoot);
                var textComp = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComp.text = choice.GetText();
                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    //playerConversant.SelectChoice(choice);
                    if (playerConversant.HasNext())
                    {
                        playerConversant.SelectChoice(choice);
                    }
                });
            }
        }
        IEnumerator ShowTextSlowly(string text)
        {
            showingText = true;
            dialogueText.text = "";

            foreach (char ch in playerConversant.GetText())
            {
                dialogueText.text += ch;
                // wait between each letter
                yield return new WaitForSeconds(0.02f);
            }
            showingText = false;
        }
    }
}
