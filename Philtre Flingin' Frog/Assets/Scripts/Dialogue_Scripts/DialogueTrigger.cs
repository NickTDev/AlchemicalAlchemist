using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    protected GameObject player;
    public UnityEvent toCall;
    [HideInInspector] public bool called = false;


    void Start()
    {
		FindPlayer();
        dialogue.Load();
    }
	
	void FindPlayer()
	{
        player = GameObject.FindWithTag("Player");
	}

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(this);
    }
}
