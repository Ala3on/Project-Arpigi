using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI dialogueText;
        [SerializeField] Button nextButton;
        [SerializeField] Button quitButton;
        [SerializeField] Transform choicesRoot;
        [SerializeField] GameObject normalDialogueRoot;
        [SerializeField] GameObject choicePrefab;

        // Start is called before the first frame update
        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.onConversationUpdated += UpdateUi;
            nextButton.onClick.AddListener(Next);
            quitButton.onClick.AddListener(() => playerConversant.Quit());
            UpdateUi();
        }

        void Next()
        {
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
            choicesRoot.gameObject.SetActive(playerConversant.IsChoosing());
            normalDialogueRoot.gameObject.SetActive(!playerConversant.IsChoosing());

            if (playerConversant.IsChoosing())
            {
                BuildChoiceList();
            }
            else
            {
                dialogueText.text = playerConversant.GetText();
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
    }
}
