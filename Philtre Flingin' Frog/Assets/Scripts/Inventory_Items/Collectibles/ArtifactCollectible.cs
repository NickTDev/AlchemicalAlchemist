using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactCollectible : Collectible
{
    [SerializeField] GameObject upgradeMenuPrefab;
    /*
    [SerializeField] GameObject potionToUnlock;
    [SerializeField] artifactActions action;

    public enum artifactActions
    {
        UNLOCK_RECIPE,
        UPGRADE_MENU,
        NUM_ACTION_TYPES
    }
    */

    public override void Collect()
    {
        Instantiate(upgradeMenuPrefab, transform.position, transform.rotation);
        HandleCollection();
        /*
        if (action == artifactActions.UPGRADE_MENU)
        {
            Instantiate(upgradeMenuPrefab, transform.position, transform.rotation);
        }
        else if(action == artifactActions.UNLOCK_RECIPE)
        {
            RecipeManager.UnlockRecipe(potionToUnlock.GetComponent<InventoryItemPotion>().GetPotionType());
        }
        */
    }
}
