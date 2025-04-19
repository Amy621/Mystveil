using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursor;
    public Vector2 cursorOffset;

    void Start()
    {
        cursorOffset = new Vector2(0, 0);
        Cursor.SetCursor(cursor, cursorOffset, CursorMode.Auto);
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        
    }
}