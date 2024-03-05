using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Entry : DialogueTrigger
{
    bool complete = false;

   private void OnTriggerEnter(Collider other)
   {
      if (!complete) //will only run the dialogue the first time the player enters
      {
         if (other.gameObject.tag == "Player_Model") TriggerDialogue();
         complete = true;
      }
   }
}
