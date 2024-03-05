using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
[ExecuteInEditMode]
public class Quest
{

    Vector3 topPos;

    //font sizes
    int titleSize = 36; 
    int taskSize = 24;
    float offput = 36f;//offput between each line

    public string name;
    public List<Task> objectives;

    //canvas and text object that will be instantiated
    [HideInInspector] public Canvas display;
    [HideInInspector] public TextMeshProUGUI sample;

    List<TextMeshProUGUI> taskLines;
    TextMeshProUGUI title;
    // Start is called before the first frame update
    public void Load(Canvas setDisplay, TextMeshProUGUI setSample)
    {
        display = setDisplay;
        sample = setSample;
        taskLines = new List<TextMeshProUGUI>();

        title = GameObject.Instantiate(sample, sample.transform.position, Quaternion.identity) as TextMeshProUGUI;
        title.transform.SetParent(display.transform);
        title.gameObject.SetActive(true);
        title.fontSize = titleSize;
        title.fontStyle = FontStyles.Bold;
        title.text = name;

        for (int i = 0; i < objectives.Count; i++)
        {
            objectives[i].GetName();
            TextMeshProUGUI tmp = GameObject.Instantiate(sample, sample.transform.position, Quaternion.identity) as TextMeshProUGUI;
            tmp.transform.SetParent(display.transform);
            tmp.gameObject.SetActive(true);
            tmp.fontSize = taskSize;
            tmp.fontStyle = FontStyles.Bold;
            tmp.fontStyle = FontStyles.Underline;
            tmp.text = objectives[i].taskName;
            tmp.transform.position = new Vector3(tmp.transform.position.x, tmp.transform.position.y - offput * (1 + i), tmp.transform.position.z);
            taskLines.Add(tmp);
        }
    }

    public void ActiveQuest()
    {
        title.gameObject.SetActive(true);
        foreach(TextMeshProUGUI tmp in taskLines)
        {
            tmp.gameObject.SetActive(true);
        }
        foreach(Task task in objectives)
        {
            task.Load();
        }
    }

    public void InactiveQuest()
    {
        title.gameObject.SetActive(false);
        foreach (TextMeshProUGUI tmp in taskLines)
        {
            tmp.gameObject.SetActive(false);
        }
        foreach (Task task in objectives)
        {
            task.Unload();
        }
    }

    public void Display()
    {
        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i].completed)
            {
                taskLines[i].color = new Color(0.0f, 1.0f, 0.0f);
            }
            else
            {
                taskLines[i].color = new Color(1.0f, 0.0f, 0.0f);
            }
        }
    }

    public bool CheckQuestIsComplete()
    {
        bool complete = true;
        for (int i = 0; i < objectives.Count; i++)
        {
            if(!objectives[i].completed)
            {
                complete = false;
            }
        }

        return complete;
    }


    public void MarkAllComplete()
    {
        foreach(Task task in objectives)
        {
            task.completed = true;
        }
    }

    public void SetFontSizes(int title, int task, float off)
    {
        titleSize = title;
        taskSize = task;
        offput = off;
    }


}
