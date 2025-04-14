using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class MoveButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public event Action<int> OnClicked;
    public event Action<int> OnHovered;
    private int moveIndex;
    [SerializeField] TMP_Text moveText;

    private void Awake()
    {
        //moveText = GetComponentInChildren<TMP_Text>();
        // if (moveText == null)
        // {
        //     Debug.LogError("MoveButton needs a TMP_Text child!");
        // }
    }

    public void SetIndex(int index)
    {
        moveIndex = index;
    }

    public void SetMoveText(string text)
    {
        if (moveText != null)
        {
            moveText.text = text;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnHovered?.Invoke(moveIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke(moveIndex);
    }
}
