using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

//Use:
//Empty object in scene with this script attached
//Create canvas (or copy the one currently made, will make prefab of it later) with 2 text boxes and a button
//In editor, fill in public variables, the TextMeshProUGUIs are the text boxes in the canvas, player is the player
//Add event on click for the button, have it set to DisplayNextSentence()

//Each object that can be spoken with should have a DialogueTrigger attached
//just add the script, and then fill in the Dialogue object with name and however many lines of dialogue it should have


public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private Queue<Sentence> sentences;
    [SerializeField] GameObject textBox;


    //find better way to do this, but the goal is that the player should not be able to move or look around while in dialogue (could potentially pause everything else but the dialogue box, might actually
    //be easier). If we want to have the player still mobile we will need something other than a continue button
    [SerializeField] GameObject player;

    bool inDialogue = false;

    [SerializeField] int delay = 2; //how many frames between each letter

    bool writing = false;
    bool skipWriting = false;

    DialogueTrigger speaker;

    private UnityEvent endFunction;

    [Header("Menu SFX")]
    [SerializeField] FMODUnity.EventReference interactSFX;
    void Start()
    {
        sentences = new Queue<Sentence>();
    }

    void Update()
    {
        
    }

    public void StartDialogue(DialogueTrigger isTalking)
    {
        speaker = isTalking;
        Dialogue dialogue = speaker.dialogue;
        GameManager.SetPaused(true);
        endFunction = speaker.toCall;
        if(inDialogue)
        {
            return;
        }
        //disables player movement and attacking so they aren't moving around or throwing stuff while interacting with NPCs
        //player.GetComponent<AttackScript>().enabled = false;
        //player.GetComponent<PlayerMovement>().enabled = false;

        //enabling the canvas w/ text, setting the name to have the NPC's name
        
        textBox.SetActive(true);
        inDialogue = true;


        //clearing and then filling the queue of lines of dialogue
        sentences.Clear();

        foreach(Sentence sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }


        DisplayNextSentence();


    }

    public void DisplayNextSentence()//Advances dialog
    {
        FMOD.Studio.EventInstance e = AudioEngineManager.CreateSound(interactSFX, 1f);
        e.start();
        if (!writing)
        {
            if (sentences.Count == 0) //end of queue
            {
                EndDialogue();
                return;
            }
            else
            {
                //make it scroll
                writing = true;
                Sentence sentence = sentences.Dequeue();
                StopAllCoroutines();
                StartCoroutine(WriteSentence(sentence));
                
            }
        }
        else
        {
            skipWriting = true;
        }
    }

    IEnumerator WriteSentence(Sentence sentence)
    {
        nameText.SetText(sentence.name);
        dialogueText.text = "";

        string[] lines = sentence.text.Split(' ');
        foreach (string word in lines)
        {
            dialogueText.text += word + " ";
            for (int i = 0; i < delay; i++)
            {
                yield return null;
            }
            if (writing && skipWriting)
            {
                dialogueText.text = sentence.text;
                skipWriting = false;
                break;
            }
        }

        /*
        foreach (char letter in sentence.text.ToCharArray())
        {
            dialogueText.text += letter;
            for(int i = 0; i < delay; i++)
            {
                yield return null;
            }
            if(writing && skipWriting)
            {
                dialogueText.text = sentence.text;
                skipWriting = false;
                break;
            }
        }
        */


        writing = false;
    }

    void EndDialogue()
    {
        GameManager.SetPaused(false);
        Debug.Log("End of conversation");
        textBox.SetActive(false);
        inDialogue = false;
        GameEvents.current.speakerEnds(speaker);
        if(endFunction != null && !speaker.called)
        {
            endFunction.Invoke();
            speaker.called = true;
        }
    }

    public void Test()
    {
        Debug.Log("successfully called function");
    }

}
