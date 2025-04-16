using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class MoveSelectionUI : MonoBehaviour
{
    [SerializeField] List<Button> moveButtons;
    [SerializeField] Color highlightedColor;
    [SerializeField] Color normalColor = Color.black;

    public event Action<int> OnMoveToDeleteSelected;
    private List<PlayerSpells> currentMoves; // to store the current list of moves

    int currentSelection = 0;

    public void SetMoveData(List<PlayerSpells> moves, PlayerSpells newMove)
    {
        currentMoves = moves;
        for (int i = 0; i < moveButtons.Count; ++i)
        {
            if (i < moves.Count)
            {
                moveButtons[i].gameObject.SetActive(true);
                TMP_Text buttonText = moveButtons[i].GetComponentInChildren<TMP_Text>();
                if(buttonText != null)
                    buttonText.text = moves[i].Name;

                int index = i;
                moveButtons[i].onClick.RemoveAllListeners();
                moveButtons[i].onClick.AddListener(() => HandleMoveSelectionClick(index));
            }
            else
            {
                moveButtons[i].gameObject.SetActive(false);
            }
        }
        UpdateHighlight();
    }

    public void HandleMoveHover(int moveIndex)
    {
        currentSelection = moveIndex;
        UpdateHighlight();
        
    }

    public void HandleMoveSelectionClick(int moveIndex)
    {
        Debug.Log($"Move at index {moveIndex} clicked for deletion.");
        OnMoveToDeleteSelected?.Invoke(moveIndex);
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < moveButtons.Count; ++i)
        {
            TMP_Text buttonText = moveButtons[i].GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                if (i < currentMoves.Count && i == currentSelection && moveButtons[i].gameObject.activeSelf)
                {
                    buttonText.color = highlightedColor;
                }
                else if (i < currentMoves.Count && moveButtons[i].gameObject.activeSelf)
                {
                    buttonText.color = normalColor;
                }
                else if (buttonText != null)
                {
                    buttonText.color = normalColor; // Ensure inactive buttons have normal color
                }
            }
        }
    }
}
