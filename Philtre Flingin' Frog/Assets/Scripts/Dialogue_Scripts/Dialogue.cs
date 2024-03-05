using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//to get a text asset, create a .txt file in the explorer in the Assets/DialogueScripts folder

[System.Serializable]
public class Dialogue
{
    [HideInInspector] public List<Sentence> sentences;
    public TextAsset dialogueScript;
    public void Load()
    {
        if(!dialogueScript)
        {
            Debug.Log("No file given");
            return; //no file name set
        }
                
        string text = dialogueScript.text;
        string[] lines = text.Split('\n');
        foreach(string line in lines)
        {
            //Debug.Log(line);
            string[] sep = line.Split(new[] { ':' }, 2);
            Sentence sentence = new Sentence();
            sentence.name = sep[0];
            sentence.text = sep[1];
            sentences.Add(sentence);
        }
    }


}
