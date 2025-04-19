using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue_Script : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    private int index;
    
    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            if(textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        
        }
    }

    void StartDialogue(){
        index = 0;
        StartCoroutine(TypeLine());
    }
   /* IEnumerator TypeLine(){
        
        foreach (char c in lines[index].ToCharArray()){
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

IEnumerator TypeLine()
    {
        string[] words = lines[index].Split(' '); // Split into words
        textComponent.text = ""; // Clear before showing words

        foreach (string word in words)
        {
            string currentText = textComponent.text; // Preserve text so far
            foreach (char c in word) 
            {
                textComponent.text += c; // Reveal character smoothly
                yield return new WaitForSeconds(textSpeed); // Delay for smooth effect
            }
            textComponent.text += " "; // Add space after each word
            yield return new WaitForSeconds(textSpeed * 2); // Slight delay before next word
        }
    }

    */


       IEnumerator TypeLine()
    {
        string currentLine = lines[index]; 
        textComponent.text = ""; 

        int currentCharacter = 0;
        while (currentCharacter < currentLine.Length)
        {
           
            string visibleText = currentLine.Substring(0, currentCharacter);
            string invisibleText = currentLine.Substring(currentCharacter, 1);
            textComponent.text = visibleText + "<color=#00000000>" + invisibleText + "</color>";

            currentCharacter++;
            yield return new WaitForSeconds(textSpeed); 
        }

        
        textComponent.text = currentLine;
    }

    


    

    void NextLine(){
        if(index < lines.Length - 1){
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else{
            gameObject.SetActive(false);
        }
    }
    }

