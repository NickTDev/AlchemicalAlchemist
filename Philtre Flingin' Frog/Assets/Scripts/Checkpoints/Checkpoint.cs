using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Checkpoint : PlayerStateSaver
{
    GameObject storedPlayer;
    Vector3 storedPos;
    GameManager gm;
    public bool hasBeenCrossed;
   [SerializeField] Transform respawnNode;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        hasBeenCrossed = false;
      if(respawnNode != null){
         storedPos = respawnNode.position;
      }
   }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.transform.parent.tag == "Player")
        {
            SetAsCheckpoint(coll.gameObject.transform.parent.gameObject);
        }
    }

    public void SetAsCheckpoint(GameObject player)
    {
        storedPlayer = player;
         if(respawnNode == null){
         storedPos = storedPlayer.transform.position;
         }
      GameManager.instance.currentCheckpoint = this;
        hasBeenCrossed = true;
        /////////stores player info
        SavePlayerInventory(storedPlayer);
    }

    public void Respawn()
    {
        GameObject newPlayer = Instantiate(storedPlayer, storedPos, Quaternion.identity);
        //all scripts spawn deactivated for some reason
        MonoBehaviour[] scripts = newPlayer.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = true;
        }
        //health bar doesn't start enabled either
        foreach(Transform child in newPlayer.transform)
        {
            if(child.gameObject.name == "Health_Bar_Canvas")
            {
                child.GetComponent<Canvas>().enabled = true;
                child.GetComponent<CanvasScaler>().enabled = true;
                child.GetComponent<GraphicRaycaster>().enabled = true;
            }
        }
        //gives it correct variables
        LoadPlayerInventory(newPlayer);
        //tells everyone to find the player again
        GameEvents.current.playerRespawn(true);
    }
}
