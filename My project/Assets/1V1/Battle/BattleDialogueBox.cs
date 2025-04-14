using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BattleDialogueBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;
    [SerializeField] TMP_Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;

    [SerializeField] List<TMP_Text> actionTexts;
    [SerializeField] List<MoveButton> moveButtons;
    [SerializeField] List<TMP_Text> powTexts;
    [SerializeField] List<TMP_Text> mpTexts;

    public event Action<int> OnActionSelected;
    public event Action<int> OnMoveSelected;
    public event Action<int> OnMoveHovered;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond);
        }
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
        if (enabled)
        {
            // Ensure buttons are interactable when enabled
            for (int i = 0; i < actionTexts.Count; i++)
            {
                int index = i; // Capture the index for the lambda
                var button = actionTexts[i].GetComponentInParent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => OnActionSelected?.Invoke(index));
                }
            }
        }
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        if (enabled)
        {
            // Ensure move buttons are interactable and their events are set up
            for (int i = 0; i < moveButtons.Count; i++)
            {
                int index = i; // Capture the index for the lambda
                moveButtons[i].SetIndex(index); // Set the index on the MoveButton script
                moveButtons[i].OnClicked -= InvokeMoveSelected; // Remove any previous listeners
                moveButtons[i].OnClicked += InvokeMoveSelected;
                moveButtons[i].OnHovered -= InvokeMoveHovered;
                moveButtons[i].OnHovered += InvokeMoveHovered;
            }
        }
        else
        {
            // Optionally clear listeners when disabled
            foreach (var button in moveButtons)
            {
                button.OnClicked -= InvokeMoveSelected;
                button.OnHovered -= InvokeMoveHovered;
            }
        }
    }

    void InvokeMoveSelected(int moveIndex)
    {
        OnMoveSelected?.Invoke(moveIndex);
    }

    void InvokeMoveHovered(int moveIndex)
    {
        OnMoveHovered?.Invoke(moveIndex);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for(int i = 0; i < actionTexts.Count; ++i)
        {
            if(i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateSpellSelection(int selectedSpell)
    {
        // for (int i = 0; i < moveTexts.Count; ++i)
        // {
        //     if (i == selectedSpell)
        //         moveTexts[i].color = highlightedColor;
        //     else
        //         moveTexts[i].color = Color.black;
        // }

        for (int i = 0; i < moveButtons.Count; ++i)
        {
            if (i == selectedSpell)
                moveButtons[i].GetComponentInChildren<TMP_Text>().color = highlightedColor;
            else
                moveButtons[i].GetComponentInChildren<TMP_Text>().color = Color.black;
        }
    }

    public void SetSpellNames(List<PlayerSpell> spells)
    {
        // for (int i = 0; i < moveTexts.Count; ++i)
        // {
        //     if (i < spells.Count)
        //     {
        //         moveTexts[i].text = spells[i].Base.Name;
        //         powTexts[i].text = "POW: " + spells[i].Base.Power;
        //         mpTexts[i].text = "MP: " + spells[i].Base.ManaPoints;
        //     }
        //     else
        //     {
        //         moveTexts[i].text = "-";
        //         powTexts[i].text = "POW: -";
        //         mpTexts[i].text = "MP: -";
        //     }
        // }
       
        for (int i = 0; i < moveButtons.Count; ++i)
        {
            if (i < spells.Count)
            {
                moveButtons[i].gameObject.SetActive(true);
                moveButtons[i].SetMoveText(spells[i].Base.Name);
                powTexts[i].text = "POW: " + spells[i].Base.Power;
                mpTexts[i].text = "MP: " + spells[i].Base.ManaPoints;
            }
            else
            {
                moveButtons[i].gameObject.SetActive(true);
                moveButtons[i].SetMoveText("-");
                powTexts[i].text = "POW: -";
                mpTexts[i].text = "MP: -";
            }
        }
    }
}