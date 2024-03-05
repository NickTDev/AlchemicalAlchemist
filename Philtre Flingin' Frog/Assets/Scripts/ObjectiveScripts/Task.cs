using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.Events;

public enum TaskType
{
    Kill,
    Fetch,
    TalkTo
};

public enum EnemyType
{
    Tracker,
    Swarmer,
    Charger
}


[System.Serializable]
[ExecuteInEditMode]
public class Task : PropertyAttribute
{
    [HideInInspector] public float totalHeight = 0;
    [HideInInspector] public int variableHeight = 1;

    public bool completed = false;

    //Type enemyType;
    System.Type[] enemyTypes = { typeof(trackerEnemy), typeof(swarmerEnemy), typeof(chargerEnemy) };
    string[] enemyNames = { "Caterpurrlar", "Kriller Scormp", "Stampedeedle" };


    [HideInInspector] public string taskName;

    public TaskType taskType;

    public EnemyType enemyType;

    public Collectible collType;

    public int num; //numer of enemies to kill or ingredients to collect

    public UnityEvent toCall;

    bool called = false; //if toCall has already been called, to avoid calling it every frame


    //OnDisable should remove listener

    int killCount = 0;
    int collectCount = 0;

    public DialogueTrigger speaker; //which dialogue to interact with


    public void Load()
    {
        switch (taskType)
        {
            case TaskType.Kill:
                GameEvents.current.onEnemyKill += onKill;
                break;
            case TaskType.Fetch:
                GameEvents.current.onCollect += onCollect;
                break;
            case TaskType.TalkTo:
                GameEvents.current.onConvoEnd += doneSpeaking;
                break;
            default:
                //taskName = "default";
                break;
        }

        
    }

    public void Unload()
    {
        switch (taskType)
        {
            case TaskType.Kill:
                GameEvents.current.onEnemyKill -= onKill;
                break;
            case TaskType.Fetch:
                GameEvents.current.onCollect -= onCollect;
                break;
            case TaskType.TalkTo:
                GameEvents.current.onConvoEnd -= doneSpeaking;
                break;
            default:
                //taskName = "default";
                break;
        }

    }


    public void GetName()
    {
        switch (taskType)
        {
            case TaskType.Kill:
                taskName = "Defeat " + num.ToString() + " " + enemyNames[(int)enemyType];
                if (num != 1)
                {
                    taskName += "s";
                }
                break;
            case TaskType.Fetch:
                taskName = "Collect " + num.ToString() + " " + collType.collectibleName;
                if (num != 1)
                {
                    taskName += "s";
                }
                break;
            case TaskType.TalkTo:
                taskName = "Talk To " + speaker.gameObject.name; //should change this later
                break;
            default:
                taskName = "default";
                break;
        }
    }


    public void onKill(System.Type typeOfEnemy)
    {
        if (typeOfEnemy == enemyTypes[(int)enemyType])
        {
            killCount++;
            if(killCount >= num)
            {
                completed = true;
            }
        }
    }

    public void onCollect(Collectible collected)
    {
        if(collected.Compare(collType))
        {
            collectCount++;
            if(collectCount >= num)
            {
                completed = true;
            }
        }
    }

    public void doneSpeaking(DialogueTrigger spoke)
    {
        Debug.Log("called dialogue event");
        if(spoke == speaker)
        {
            completed = true;
        }
    }

    //called when the quest starts
    void CheckComplete()
    {
        if(taskType == TaskType.Kill)
        {
            var remainingEnemies = GameObject.FindObjectsOfType(enemyTypes[(int)enemyType]);
            if(remainingEnemies.Length > num)
            {
                completed = true; //not enough enemies in the scene to complete the objective
                if (!called && toCall != null)
                {
                    toCall.Invoke();
                }
                called = true;
            }
        }
        if(taskType == TaskType.Fetch)
        {
            //check to see if any objects with the ingredient type are in the scene
            GameObject[] ingredients = (GameObject[])GameObject.FindObjectsOfType(typeof(Collectible));
            List<Collectible> sameIngredients = new List<Collectible>();
            for(int i = 0; i < ingredients.Length; i++)
            {
                if(collType.Compare(ingredients[i].GetComponent<Collectible>()))
                {
                    sameIngredients.Add(ingredients[i].gameObject.GetComponent<Collectible>());
                }
            }
            if(sameIngredients.Count < num)
            {
                completed = true; //not enough ingredients in the scene to complete the objective
                if (!called && toCall != null)
                {
                    toCall.Invoke();
                }
                called = true;
            }


        }
    }








}