using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DialogueController : MonoBehaviour
{
    public TextMeshProUGUI DialogueText;
    public string[] sentences;
    private int index = 0;
    public float dialogueSpeed;
    private bool nextText = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            if (nextText == true)
            {
                NextSentence();
                nextText = false;
            }
        }
    }

    IEnumerator WriteSentence()
    {
        foreach (char Character in sentences[index].ToCharArray())
        {
            DialogueText.text += Character;
            yield return new WaitForSeconds(dialogueSpeed);
        }

        index++;
        nextText = true;
    }

    void NextSentence()
    { 
        if (index <= sentences.Length-1) 
        {
            DialogueText.text = "";
            StartCoroutine(WriteSentence());
        }
    }
}
