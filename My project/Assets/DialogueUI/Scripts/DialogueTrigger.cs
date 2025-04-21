using UnityEngine;
using TMPro;
using Ink.Runtime;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text nametagText;
    public TMP_Text messageText;
    public GameObject choicesPanel;
    public Button[] choiceButtons = new Button[3];

    private Story currentStory;
    private DialogueManager dialogueManager;
    private bool isTyping;
    private Coroutine typeCoroutine;
    private float typingSpeed = 0.04f;
    private bool waitingForInput = false;

    void Start()
    {
        dialoguePanel.SetActive(false);
        choicesPanel.SetActive(false);
        dialogueManager = FindObjectOfType<DialogueManager>();

        if (dialogueManager != null)
        {
            dialogueManager.OnDialogueStarted += StartDialogueInternal;
            dialogueManager.OnDialogueEnded += EndDialogueInternal;
        }
        else
        {
            Debug.LogError("DialogueManager not found in the scene!");
        }

        // Ensure the choices panel is initially inactive
        if (choicesPanel != null)
        {
            choicesPanel.SetActive(false);
        }

        // Initially hide all assigned choice buttons
        foreach (var button in choiceButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    public void TriggerDialogue(string interactionName)
    {
        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(interactionName);
        }
    }

    private bool canAdvanceDialogue = false;
    public void StartDialogueInternal(Story story)
    {
        Debug.Log("DialogueTrigger: StartDialogueInternal called");
        currentStory = story;
        dialoguePanel.SetActive(true);
        waitingForInput = true;
        // Introduce a small delay or set a flag after a frame
        StartCoroutine(SetCanAdvanceDialogue());
        DisplayNextLine();
    }

    System.Collections.IEnumerator SetCanAdvanceDialogue()
    {
        yield return null; // Wait for one frame
        canAdvanceDialogue = true;
    }

    void EndDialogueInternal()
    {
        Debug.Log("Dialogue Trigger: EndDialogueInteral called");
        currentStory = null;
        dialoguePanel.SetActive(false);
        choicesPanel.SetActive(false);
        // Hide all assigned choice buttons on dialogue end
        foreach (var button in choiceButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    public void Update()
    {
        if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping && typeCoroutine != null)
            {
                // Auto-complete the current line
                StopCoroutine(typeCoroutine);
                messageText.text = currentFullStoryLine;
                isTyping = false;
                DisplayChoices();
            }
            else if (currentStory != null && !isTyping)
            {
                waitingForInput = false;
                if (currentStory.canContinue && currentStory.currentChoices.Count == 0)
                {
                    DisplayNextLine();
                }
                else if (currentStory.currentChoices.Count > 0)
                {
                    // Do nothing here, choices are being displayed
                }
                else if (!currentStory.canContinue && currentStory.currentChoices.Count == 0)
                {
                    // Dialogue has ended
                    dialogueManager.EndDialogue();
                }
            }
        }
    }

    private string currentFullStoryLine;

    void DisplayNextLine()
    {
        Debug.Log("In display next line");
        currentFullStoryLine = currentStory.Continue();
        Debug.Log(currentFullStoryLine);
        string speaker = "";
        int colonIndex = currentFullStoryLine.IndexOf(':');
        if (colonIndex > 0)
        {
            speaker = currentFullStoryLine.Substring(0, colonIndex).Trim();
            currentFullStoryLine = currentFullStoryLine.Substring(colonIndex + 1).Trim();
        }

        nametagText.text = speaker;
        StopAllCoroutines();
        messageText.text = ""; // Clear previous text before starting to type
        typeCoroutine = StartCoroutine(TypeText(currentFullStoryLine));
        choicesPanel.SetActive(false);
        if (choicesPanel != null)
        {
            choicesPanel.SetActive(false);
        }
        foreach (var button in choiceButtons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
                button.onClick.RemoveAllListeners();
            }
        }
        //choiceButtons.Clear();
    }

    System.Collections.IEnumerator TypeText(string line)
    {
        messageText.text = "";
        isTyping = true;
        foreach (char character in line.ToCharArray())
        {
            messageText.text += character;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
        DisplayChoices();
    }

    void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;
        if (currentChoices.Count > 0 && choicesPanel != null && choiceButtons.Length == 3)
        {
            choicesPanel.SetActive(true);

            for (int i = 0; i < choiceButtons.Length; i++)
            {
                if (i < currentChoices.Count && choiceButtons[i] != null)
                {
                    Choice choice = currentChoices[i];
                    TMP_Text buttonText = choiceButtons[i].GetComponentInChildren<TMP_Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = choice.text;
                    }
                    int choiceIndex = i; // Capture the index for the listener
                    choiceButtons[i].onClick.AddListener(() => MakeChoice(choiceIndex));
                    choiceButtons[i].gameObject.SetActive(true);
                }
                else if (choiceButtons[i] != null)
                {
                    choiceButtons[i].gameObject.SetActive(false); // Hide unused buttons
                    choiceButtons[i].onClick.RemoveAllListeners(); // Clear listeners for unused buttons
                }
            }
        }
        else if (choicesPanel != null)
        {
            choicesPanel.SetActive(false); // Hide the panel if no choices or not enough buttons
            // Ensure all buttons are hidden if no choices
            foreach (var button in choiceButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                    button.onClick.RemoveAllListeners();
                }
            }
        }
    }

    public void MakeChoice(object choiceIndex)
    {
        currentStory.ChooseChoiceIndex((int)choiceIndex);
        DisplayNextLine();
    }
}