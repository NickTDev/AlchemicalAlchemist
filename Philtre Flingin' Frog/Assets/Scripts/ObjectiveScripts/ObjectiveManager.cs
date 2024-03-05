using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * To use:
 * In editor, add a Quest to the Objectives manager. Set the name of the Quest to whatever you want the name to be (will be shown at the top of the objectives)
 * Add however many individual tasks you want to be done in the quest. The order does not affect players' ability to complete them, they can do them in any order
 * For each task, set the Type to whichever type of task it should be, either killing enemies, collecting ingredients, or interacting with dialogue
 * For enemies, select the type of enemy, tracker = caterpurrlar, swarmer = kriller scormp, charger = stampedeetle. Then, put in how many you want to have them kill
 * For ingredients, enter a collectible from the scene into the area, then put in how many you want to have them collect (note, the UI will use the name given in the Collectible object)
 * For dialogue, drag in the DialogueTrigger you want them to interact with (note, the UI will show the name of the actual object that the Trigger is attached to)
 * 
 * Repeat for however many Quests you want to have them complete in the level
 */

public class ObjectiveManager : MonoBehaviour
{
    public List<Quest> quests;
    Quest currentQuest;

    public Canvas objCanvas;
    public TextMeshProUGUI sample;

    int index = 0;

    //font sizes
    [SerializeField] int titleSize = 36;
    [SerializeField] int taskSize = 24;
    [SerializeField] float offput = 36f;//offput between each line
    bool wasPaused = false; //whether the game was paused last frame


    // Start is called before the first frame update
    void Start()
    {
        if (quests.Count == 0)
        {
            Debug.Log("No Quests Added");
            this.enabled = false;
            return;
        }
        foreach(Quest quest in quests)
        {
            quest.SetFontSizes(titleSize, taskSize, offput);
            quest.Load(objCanvas, sample);
            quest.InactiveQuest();
        }
        SetCurrent();
    }

    void SetCurrent()
    {
        if(index >= quests.Count)
        {
            return;
        }
        if (index != 0)
        {
            currentQuest.InactiveQuest();
        }
        currentQuest = quests[index];
        currentQuest.ActiveQuest();
        index++;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("Marking Complete");
            //currentQuest.MarkAllComplete();
        }

        currentQuest.Display();
        if(currentQuest.CheckQuestIsComplete()) //if quest is complete, go to next one
        {
            SetCurrent();
        }
        HandlePause();
    }


    void HandlePause() //UI disappears during pause
    {
        if(GameManager.GetPaused() && !wasPaused)
        {
            foreach(Transform display in objCanvas.transform)
            {
                display.GetComponent<TMP_Text>().enabled = false;
            }
        }
        else if (!GameManager.GetPaused() && wasPaused)
        {
            foreach (Transform display in objCanvas.transform)
            {
                display.GetComponent<TMP_Text>().enabled = true;
            }
        }

        wasPaused = GameManager.GetPaused();
    }
}
