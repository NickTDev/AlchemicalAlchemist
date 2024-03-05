using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientInventoryScript : InventoryScript
{
    public override void InitTypeVariables()//sets up several variables based on the inventory type
    {
        invType = InventoryType.INGREDIENT;
        X_size = (int)RecipeManager.primaryElement.NUM_PRIMARIES;
        Y_size = (int)RecipeManager.secondaryElement.NUM_SECONDARIES; 
    }
    public override void SpawnSlotItem(GameObject slot, int x, int y)//creates the manipulatable inventory item object in the slot
    {
        GameObject item = Instantiate(GameObject.FindWithTag("Player").GetComponent<RecipeManager>().GetIngredient((RecipeManager.primaryElement)x, (RecipeManager.secondaryElement)y), slot.transform.position, slot.transform.rotation) as GameObject;
        item.transform.SetParent(GameObject.Find(hierarchyParentName).transform, true);
        item.GetComponent<InventoryItemIngredient>().SetScale();
    }
}
