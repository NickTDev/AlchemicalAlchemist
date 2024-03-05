using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeTrigger : MonoBehaviour
{

    [SerializeField] GameObject potionToUnlock;
    bool activated = false;

    void Start()
    {
        GameEvents.current.onPlayerRespawn += PlayerRespawn;
    }

    void OnTriggerEnter(Collider coll)
    {
        if(!activated && coll.gameObject.tag == "Player_Model")
        {
            RecipeManager.UnlockRecipe(potionToUnlock.GetComponent<InventoryItemPotion>().GetPotionType());
            activated = true;
        }
    }
    public void PlayerRespawn(bool yes = true) //to ensure player doesn't get locked out of recipe
    {
        activated = false;
    }
}
