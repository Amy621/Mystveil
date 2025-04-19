using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;
using System;

public class DialogueManager : MonoBehaviour
{
    public TextAsset inkFile;
    public GameObject textBox;
    public GameObject optionPanel;
    public List<Button> optionButtons;

    static Story story;
    TMP_Text nametag;
    TMP_Text message;
    int indexOfChoiceSelected;
    static Choice choiceSelected;

    public float textSpeed = 0.04f;
    private bool isTyping = false;
    private string currentFullSentence = "";
    private Coroutine typeCoroutine;

    // Start is called before the first frame update
    public void Start()
    {
        story = new Story(inkFile.text);
        nametag = textBox.transform.GetChild(0).GetComponent<TMP_Text>();
        message = textBox.transform.GetChild(1).GetComponent<TMP_Text>();
        choiceSelected = null;

        AdvanceDialogue(); // Start the first line of dialogue
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                // If currently typing, finish the sentence immediately
                StopCoroutine(typeCoroutine);
                message.text = currentFullSentence.Substring(currentFullSentence.IndexOf(":", StringComparison.Ordinal) + 1);
                isTyping = false;
            }
            else if (story.canContinue)
            {
                AdvanceDialogue();

                // Are there any choices?
                if (story.currentChoices.Count != 0)
                {
                    StartCoroutine(ShowChoices());
                }
            }
            else
            {
                FinishDialogue();
            }
        }
    }

    private void FinishDialogue()
    {
        Debug.Log("End of Dialogue!");
    }

    void AdvanceDialogue()
    {
        currentFullSentence = story.Continue();
        nametag.text = currentFullSentence.Substring(0, currentFullSentence.IndexOf(":", StringComparison.Ordinal));

        StopAllCoroutines();
        typeCoroutine = StartCoroutine(TypeSentence(currentFullSentence.Substring(currentFullSentence.IndexOf(":", StringComparison.Ordinal) + 1)));
    }

    IEnumerator TypeSentence(string sentence)
    {
        message.text = "";
        isTyping = true;

        int currentCharacter = 0;
        while (currentCharacter < sentence.Length)
        {
            string visibleText = sentence.Substring(0, currentCharacter);
            string invisibleText = sentence.Substring(currentCharacter, 1);
            message.text = visibleText + "<color=#00000000>" + invisibleText + "</color>";

            currentCharacter++;
            yield return new WaitForSeconds(textSpeed);
        }

        message.text = sentence;
        isTyping = false;
        yield return null;
    }

    IEnumerator ShowChoices()
    {
        Debug.Log("There are choices that need to be made here!");
        List<Choice> _choices = story.currentChoices;

        for (int i = 0; i < _choices.Count; i++)
        {
            Button optionButton = optionButtons[i];
            optionButton.gameObject.SetActive(true);
            optionButton.GetComponentInChildren<TMP_Text>().text = _choices[i].text; // Access TMP_Text in children
            Selectable selectable = optionButton.gameObject.AddComponent<Selectable>();
            selectable.element = _choices[i];
            optionButton.onClick.AddListener(() => { selectable.Decide(); });
        }

        optionPanel.SetActive(true);
        yield return new WaitUntil(() => { return choiceSelected != null; });
        AdvanceFromDecision();
    }

    public static void SetDecision(object element)
    {
        choiceSelected = (Choice)element;
        story.ChooseChoiceIndex(choiceSelected.index);
    }

    void AdvanceFromDecision()
    {
        optionPanel.SetActive(false);
        for (int i = 0; i < optionButtons.Count; i++)
        {
            Button optionButton = optionButtons[i];
            optionButton.gameObject.SetActive(false);
            Selectable script = optionButton.GetComponent<Selectable>();
            if (script != null)
            {
                Destroy(script);
            }
            optionButton.onClick.RemoveAllListeners(); // Clean up listeners
        }

        choiceSelected = null;
        AdvanceDialogue();
    }
}