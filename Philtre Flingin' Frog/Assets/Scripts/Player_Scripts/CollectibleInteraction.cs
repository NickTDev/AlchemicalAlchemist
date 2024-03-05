using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleInteraction : MonoBehaviour
{
    Collectible collect;
    List<Collectible> inRange = new List<Collectible>(); //can use to hold objects so I can sort them later
    List <Collectible> toCollect = new List <Collectible>();
    GameObject player;
    PlayerInventory inventory;

    public float range = 5f;
    float startTime;

    CollectiblesManager collManager;

    void Start()
    {
        player = this.gameObject;
        inventory = player.GetComponent<PlayerInventory>();
        collManager = GameObject.FindObjectOfType<CollectiblesManager>();
    }

    //during update, refresh collectibles within range
    //either after that or during fixedupdate have the input to grab the nearest collectible

    void Update()
    {
        float speed = this.gameObject.GetComponent<PlayerMovement>().getSpeed();
        if(speed > 0.25) //speed is never quite 0
            //should only recalc the list when the player moves (ideally would only refill when the player moves out of range of a collectible - could add that in the collectible script
        {
            Refill();
        }
        if (toCollect.Count > 0)
        {
            toCollect[0].Select();
        }
        Check();

        if(Input.GetKeyDown(KeyCode.O))
        {
            FindObjectOfType<GameManager>().Respawn();
        }

    }

    public void Refill()
    {
        if(toCollect.Count > 0)
        {
            toCollect[0].Deselect();
        }
        toCollect.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);
        foreach (var hitCollider in hitColliders)
        {
            if(hitCollider.gameObject.layer == LayerMask.NameToLayer("Collectible"))
            {
                toCollect.Add(hitCollider.gameObject.GetComponent<Collectible>());
            }

        }

    }

    void Collect(Collectible coll)
    {
        collManager.Popup(coll);
        //Debug.Log("Collected an ingredient");
        if(coll.GetCollectibleType() == Collectible.collectibleType.INGREDIENT)
        {
            RecipeManager.primaryElement prmElm = coll.prmElm;
            RecipeManager.secondaryElement scdElm = coll.scdElm;
            int primary = (int)(prmElm);
            int secondary = (int)(scdElm);
            inventory.AddIngredient(primary, secondary, coll.count);
        }
        GameEvents.current.itemCollected(coll);
        coll.Collect();
    }

    void Check()
    {
        if (toCollect.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                startTime = Time.time;
            }

            if(Input.GetKeyUp(KeyCode.E))
            {
                float t = Time.time - startTime;
                //Debug.Log(t.ToString());
                if (t > 1.0f)
                {
                    //Debug.Log("Collecting multiple");
                    foreach(Collectible coll in toCollect)
                    {
                        Collect(coll);
                        //Destroy(coll.gameObject); //happens in the collectible itself
                    }
                    toCollect.Clear();
                }
                else
                {
                    Collect(toCollect[0]);
                    //Destroy(toCollect[0].gameObject); //happens in the collectible itself
                    toCollect.RemoveAt(0);
                }
            }

            //system to scroll through the objects, not really needed but if we want to reimplement it later
            /*
            if(Input.mouseScrollDelta.y > 0f) //scroll up, move [0] to the end of the list
            {
                toCollect[0].Deselect();
                Collectible temp = toCollect[0];
                toCollect.RemoveAt(0);
                toCollect.Add(temp);

            }
            else if(Input.mouseScrollDelta.y < 0f) //scroll down, move [count - 1] to the front of the list
            {
                toCollect[0].Deselect();
                Collectible temp = toCollect[toCollect.Count - 1];
                toCollect.RemoveAt(toCollect.Count - 1);
                toCollect.Insert(0, temp);

            }
            */
        }
    }
}
