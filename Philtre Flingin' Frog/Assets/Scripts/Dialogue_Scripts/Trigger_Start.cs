using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Start : DialogueTrigger
{
    bool complete = false;
    // Update is called once per frame
    void Update() //should only run after all Start() functions are done just in case
    {
        if(!complete) //will only run once
        {
            TriggerDialogue();
            complete = true;
        }
    }
}
