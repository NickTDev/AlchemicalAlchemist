using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_SpeakWith : DialogueTrigger
{

    public float radius = 3f;
    void Update()
    {
        float distance = Vector3.Distance(this.transform.position, player.transform.position);
        if (distance <= radius)
        {
            //player within radius
            if (Input.GetKeyDown(KeyCode.E))
            {
                TriggerDialogue();
            }

        }

    }

   public void ForceDialogue(){
      TriggerDialogue();
   }
}
