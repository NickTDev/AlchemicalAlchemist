using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgression : PlayerStateSaver
{
    [SerializeField] protected string sceneToLoad; //which scene to go to after this
    bool activated = false;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.transform.parent.tag == "Player" && !activated)
        {
            SavePlayerInventory(coll.gameObject.transform.parent.gameObject);
            ////switches to new scene
            DontDestroyOnLoad(gameObject);
            activated = true;
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    //gives the player the correct inventory
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(activated)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if(player != null) //if it loads a scene without a player, self destructs without adding inventory
            {
                LoadPlayerInventory(player);
            }
            activated = false;
            Destroy(gameObject);
            Destroy(this);
        }
    }


}
