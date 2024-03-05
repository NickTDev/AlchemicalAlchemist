using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//this will be put on the NPC that is speaking

//hasn't been updated for new dialogue system but we don't use it so i won't bother
public class HoverDialogue : MonoBehaviour
{
    /*
    public TextMeshProUGUI dialogueText;

    private Queue<string> sentences;
    [SerializeField] GameObject textBox;

    public float delay;

    bool inDialogue = false;
    GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        sentences = new Queue<string>();
    }

    void Update()
    {
        if (dialogueText)
        {
            dialogueText.transform.rotation = Quaternion.LookRotation(this.transform.position - player.transform.position);
        }
    }


    public void StartDialogue(Dialogue dialogue)
    {
        if(inDialogue)
        {
            return;
        }

        textBox.SetActive(true);
        inDialogue = true;

        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }


        DisplayNextSentence();


    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0) //end of queue
        {
            EndDialogue();
            return;
        }
        else
        {
            //make it scroll
            string sentence = sentences.Dequeue();
            StopAllCoroutines();
            StartCoroutine(WriteSentence(sentence));
        }
    }


    IEnumerator WriteSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
            yield return null;
        }
        yield return new WaitForSeconds(delay);
        DisplayNextSentence();

    }


    void EndDialogue()
    {
        Debug.Log("End of conversation");
        textBox.SetActive(false);
        inDialogue = false;

    }


    */

}
