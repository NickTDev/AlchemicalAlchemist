using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionInventoryScript : InventoryScript
{

    public override void InitTypeVariables()//sets up several variables based on the inventory type
    {
        invType = InventoryType.POTION;
        X_size = (int)playerInventory.GetPotionInventoryDimensions().x;// (int)RecipeManager.potionType.NUM_POTIONS;
        Y_size = (int)playerInventory.GetPotionInventoryDimensions().y;
    }
    public override void SpawnSlotItem(GameObject slot, int x, int y)//creates the manipulatable inventory item object in the slot
    {
        int pos = y * X_size + x;
        if (playerInventory.GetNumPotionInSlot(pos) > 0) //doesn't spawn most of the inventory without this check
        {
            RecipeManager.potionType type = playerInventory.GetPotionTypeInSlot(pos);
            RecipeManager.secondaryElement secondary = playerInventory.GetPotionSecondaryInSlot(pos);
            GameObject item = Instantiate(recipeManager.potionPrefabs[(int)type], slot.transform.position, slot.transform.rotation) as GameObject;
            item.transform.SetParent(GameObject.Find(hierarchyParentName).transform, true);
            InventoryItemPotion itemScript = item.GetComponent<InventoryItemPotion>();
            itemScript.SetScale();
            itemScript.SetSecondaryType(secondary);
            itemScript.SetSlot(slot);
            itemScript.ScaleUp();
        }
    }
}