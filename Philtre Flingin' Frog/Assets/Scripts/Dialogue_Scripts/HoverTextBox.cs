using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//this script will be put on the NPC that is speaking

public class HoverTextBox : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject player;
    public TextMeshProUGUI dialogueText;

    void Start()
    {
        FindPlayer();
    }
	void FindPlayer()
	{
        player = GameObject.FindWithTag("Player");
	}

    void Update()
    {
        if (dialogueText)
        {
            dialogueText.transform.LookAt(player.transform);
        }
    }
}
